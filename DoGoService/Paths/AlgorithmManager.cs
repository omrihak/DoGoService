﻿using DoGoService.Paths.Models;
using DoGoService.DataObjects;
using Google.Maps;
using Google.Maps.DistanceMatrix;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DoGoService.Paths
{
    public class AlgorithmManager
    {
        private static DogoDbEntities db;

        public static void init()
        {
            if (db == null)
            {
                db = new DogoDbEntities();
                GoogleSigned.AssignAllServices(new GoogleSigned("AIzaSyATaHv7YbpgtLoWL43P8hDjrcH30H8gyNI"));
            }
        }

        public static WalkerPath DoAlgorithm(string homeLocation, List<DogWalk> walks)
        {
            init();
            var dogWalkDetails = new List<DogWalkDetails>();
            var dogUserIds = walks.Select(walk => walk.UserId);

            var availabilityTimes = (from av in db.AvailabilityTimes
                                     where dogUserIds.Contains(av.UserId)
                                     select av).ToList();
            var usersWithDogs = db.Users.Where(dog => dogUserIds.Contains(dog.Id));
            walks.ForEach(walk =>
            {
                var avTimes = availabilityTimes.Where(t => t.UserId == walk.UserId).ToList();
                avTimes.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
                var earliestPickup = avTimes.First().StartTime;
                avTimes.Sort((a, b) => a.EndTime.CompareTo(b.EndTime));
                var latestPickup = avTimes.Last().EndTime;
                var userToAdd = usersWithDogs.First(user => user.Id == walk.UserId);

                dogWalkDetails.Add(new DogWalkDetails()
                {
                    Address = userToAdd.Address + " " + userToAdd.City,
                    EarliestPickup = earliestPickup,
                    LatestPickup = latestPickup,
                    TimeOfWalk = walk.Duration * 60
                });
            });

            List<WalkerPathLink> graphLinks;
            DogoWaypoint dogoHome;
            List<DogoWaypoint> graphNodes;
            GetAlgorithmData(homeLocation, dogWalkDetails, out graphNodes, out graphLinks, out dogoHome);
            return new SmartGreedyAlgorithm().CalculatePath(dogoHome, graphNodes, dogWalkDetails, graphLinks);
        }

        public static void GetAlgorithmData(string homeLocation, List<DogWalkDetails> dogWalks, out List<DogoWaypoint> graphNodes, out List<WalkerPathLink> graphLinks, out DogoWaypoint dogoHome)
        {
            DistanceMatrixService service = new DistanceMatrixService();
            service.BaseUri = new Uri("https://maps.google.com/maps/api/distancematrix/");
            var homeWalkDetails = new DogWalkDetails() { Address = homeLocation, TimeOfWalk = 0, EarliestPickup = new TimeSpan(), LatestPickup = new TimeSpan() };
            dogWalks.Add(homeWalkDetails);
            Waypoint home = null;
            var dogWaypointsDic = new Dictionary<Waypoint, int>();
            for (int k = 0; k < dogWalks.Count; k++)
            {
                var travelOrigin = dogWalks.ElementAt(k);
                var origin = new Waypoint() { Address = travelOrigin.Address };
                dogWaypointsDic.Add(origin, travelOrigin.TimeOfWalk);
                if (travelOrigin.Address == homeLocation)
                {
                    home = origin;
                }
            }

            var tasks = new List<Task>();
            var links = new ConcurrentBag<WaypointLink>();
            for (int k = 0; k < dogWalks.Count; k++)
            {

                int index = k;
                var task = new Task(() =>
                {
                    var travelOrigin = dogWalks.ElementAt(index);
                    var origin = dogWaypointsDic.Keys.First(key => key.Address == travelOrigin.Address);

                    DistanceMatrixRequest request = new DistanceMatrixRequest();
                    request.AddOrigin(origin);
                    for (int t = index; t < dogWalks.Count; t++)
                    {
                        var travelDest = dogWalks.ElementAt(t);
                        if (travelDest.Address != travelOrigin.Address)
                        {
                            var dest = dogWaypointsDic.Keys.First(key => key.Address == travelDest.Address);
                            request.AddDestination(dest);
                        }
                    }

                    request.Sensor = false;
                    request.Mode = TravelMode.walking;
                    var response = service.GetResponse(request);
                    while (response.Status == ServiceResponseStatus.OverQueryLimit)
                    {
                        Thread.Sleep(5000);
                        response = service.GetResponse(request);
                    }

                    for (int j = 0; j < request.WaypointsDestination.Count; j++)
                    {
                        links.Add(new WaypointLink()
                        {
                            SourceWaypoint = origin.Address,
                            DestWaypoint = request.WaypointsDestination.Values[j].Address,
                            Duration = int.Parse(response.Rows[0].Elements[j].duration.Value)
                        });
                    }

                });
                tasks.Add(task);
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());

            links.ToList().ForEach(link =>
            {
                links.Add(
                    new WaypointLink()
                    {
                        SourceWaypoint = link.DestWaypoint,
                        DestWaypoint = link.SourceWaypoint,
                        Duration = link.Duration
                    });
            });

            foreach (var waypoint in dogWaypointsDic.Keys.ToList())
            {
                links.Add(new WaypointLink()
                {
                    SourceWaypoint = waypoint.Address,
                    DestWaypoint = waypoint.Address,
                    Duration = 0
                });
            }

            dogWalks.Remove(homeWalkDetails);
            graphNodes = new List<DogoWaypoint>();
            graphLinks = new List<WalkerPathLink>();
            foreach (var waypoint in dogWaypointsDic.Keys.ToList())
            {
                var first = new DogoWaypoint() { Address = waypoint.Address, IsPassed = false, IsReturnWaypoint = false, TimeOfFirstWaypointPass = new TimeSpan(0, 0, -1) };
                if (waypoint != home)
                {
                    var second = new DogoWaypoint() { Address = waypoint.Address, IsPassed = false, IsReturnWaypoint = true, TimeOfFirstWaypointPass = new TimeSpan(0, 0, -1) };
                    graphNodes.Add(second);
                }
                graphNodes.Add(first);
            }

            dogoHome = graphNodes.First(node => node.Address == homeLocation);
            var nowTime = DateTime.Now - DateTime.Today;
            foreach (var sourceWaypoint in graphNodes.ToList())
            {
                foreach (var destWaypoint in graphNodes.ToList())
                {
                    if (sourceWaypoint != destWaypoint)
                    {
                        var duration = links.FirstOrDefault(link => link.SourceWaypoint == sourceWaypoint.Address && link.DestWaypoint == destWaypoint.Address).Duration;
                        var currWalk = dogWalks.FirstOrDefault(walk => walk.Address == destWaypoint.Address);
                        if (destWaypoint.IsReturnWaypoint)
                        {
                            graphLinks.Add(new WalkerPathLink() { SourceWaypoint = sourceWaypoint, DestWaypoint = destWaypoint, Duration = int.MaxValue, RealDuration = duration });
                        }
                        else
                        {
                            graphLinks.Add(new WalkerPathLink() { SourceWaypoint = sourceWaypoint, DestWaypoint = destWaypoint, Duration = (int)Math.Max(currWalk != null ? (currWalk.EarliestPickup - nowTime).TotalSeconds : 0, duration), RealDuration = duration });
                        }
                    }
                }
            }
        }
    }
}