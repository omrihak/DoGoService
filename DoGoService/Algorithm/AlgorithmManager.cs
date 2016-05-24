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

namespace DoGoService.Algorithm
{
    public class AlgorithmManager
    {
        private static DogoDbEntities db;

        public static void init()
        {
            if (true)
            {
                db = new DogoDbEntities();
                GoogleSigned.AssignAllServices(new GoogleSigned("AIzaSyATaHv7YbpgtLoWL43P8hDjrcH30H8gyNI"));
            }
        }

        public static AlgorithmAnswer DoAlgorithm(string homeLocation, List<DogWalk> walks)
        {
            var dogWalkDetails = new List<DogWalkDetails>();
            var dogUserIds = walks.Select(walk => walk.DogUserId);
            var availabilityTimes = db.AvailabilityTimes.Where(availabilityTime => dogUserIds.Contains(availabilityTime.UserId)).Select(av=> new Tuple<int, TimeSpan>(av.UserId, av.StartTime));
            var usersWithDogs = db.Users.Where(dog => dogUserIds.Contains(dog.Id));
            walks.ForEach(walk =>
            {
                var avTimes = availabilityTimes.Where(t => t.Item1 == walk.DogUserId).ToList();
                avTimes.Sort((a, b) => a.Item2.CompareTo(b.Item2));

                dogWalkDetails.Add(new DogWalkDetails()
                {
                    Address = usersWithDogs.First(user => user.Id == walk.DogUserId).Address,
                    EarliestPickup = avTimes.First().Item2,
                    TimeOfWalk = walk.TimeOfWalk
                });
            });

            List<DogoLink> graphLinks;
            DogoWaypoint dogoHome;
            List<DogoWaypoint> graphNodes;
            GetAlgorithmData(homeLocation, dogWalkDetails, out graphNodes, out graphLinks, out dogoHome);
            return new SmartGreedyAlgorithm().DoAlgorithm(dogoHome, graphNodes, dogWalkDetails, graphLinks);
        }


        public static void GetAlgorithmData(string homeLocation, List<DogWalkDetails> dogWalks, out List<DogoWaypoint> graphNodes,  out List<DogoLink> graphLinks, out DogoWaypoint dogoHome)
        {
            DistanceMatrixService service = new DistanceMatrixService();
            service.BaseUri = new Uri("https://maps.google.com/maps/api/distancematrix/");
            var homeWalkDetails = new DogWalkDetails() { Address = homeLocation, TimeOfWalk = 0, EarliestPickup = new TimeSpan() };
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
                            sourceWaypoint = origin.Address,
                            destWaypoint = request.WaypointsDestination.Values[j].Address,
                            duration = int.Parse(response.Rows[0].Elements[j].duration.Value)
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
                        sourceWaypoint = link.destWaypoint,
                        destWaypoint = link.sourceWaypoint,
                        duration = link.duration
                    });
            });

            foreach (var waypoint in dogWaypointsDic.Keys.ToList())
            {
                links.Add(new WaypointLink()
                {
                    sourceWaypoint = waypoint.Address,
                    destWaypoint = waypoint.Address,
                    duration = 0
                });
            }

            dogWalks.Remove(homeWalkDetails);
            graphNodes = new List<DogoWaypoint>();
            graphLinks = new List<DogoLink>();
            foreach (var waypoint in dogWaypointsDic.Keys.ToList())
            {
                var first = new DogoWaypoint() { address = waypoint.Address, isPassed = false, isReturnWaypoint = false, timeOfFirstWaypointPass = new TimeSpan(0,0,-1) };
                if (waypoint != home)
                {
                    var second = new DogoWaypoint() { address = waypoint.Address, isPassed = false, isReturnWaypoint = true, timeOfFirstWaypointPass = new TimeSpan(0, 0, -1) };
                    graphNodes.Add(second);
                }
                graphNodes.Add(first);
            }

            dogoHome = graphNodes.First(node => node.address == homeLocation);

            foreach (var sourceWaypoint in graphNodes.ToList())
            {
                foreach (var destWaypoint in graphNodes.ToList())
                {
                    if (sourceWaypoint != destWaypoint)
                    {
                        var duration = links.FirstOrDefault(link => link.sourceWaypoint == sourceWaypoint.address && link.destWaypoint == destWaypoint.address).duration;

                        if (destWaypoint.isReturnWaypoint)
                        {
                            graphLinks.Add(new DogoLink() { sourceWaypoint = sourceWaypoint, destWaypoint = destWaypoint, duration = int.MaxValue, realDuration = duration });
                        }
                        else
                        {
                            graphLinks.Add(new DogoLink() { sourceWaypoint = sourceWaypoint, destWaypoint = destWaypoint, duration = duration, realDuration = duration });
                        }
                    }
                }
            }
        }
    }
}