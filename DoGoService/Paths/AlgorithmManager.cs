using DoGoService.DataObjects;
using DoGoService.Paths.Models;
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
        private static DogoDbEntitiesConnection db;

        public static void init()
        {
            if (db == null)
            {
                db = new DogoDbEntitiesConnection();
                GoogleSigned.AssignAllServices(new GoogleSigned("AIzaSyATaHv7YbpgtLoWL43P8hDjrcH30H8gyNI"));
            }
        }

        public static WalkerPath DoAlgorithm(int walkerID, List<DogWalk> walks)
        {
            init();
            var homeLocation = db.DogWalkers.First(walker => walker.id == walkerID).address;
            var dogWalkDetails = new List<DogWalkDetails>();
            var dogUserIds = walks.Select(walk => walk.UserId);
            var usersWithDogs = db.DogOwners.Where(dog => dogUserIds.Contains(dog.id));
            walks.ForEach(walk =>
            {
                var userToAdd = usersWithDogs.First(user => user.id == walk.UserId);
                var times = getEarliestAndLatestPickup(userToAdd);
                dogWalkDetails.Add(new DogWalkDetails()
                {
                    Address = userToAdd.address + " " + userToAdd.city,
                    EarliestPickup = times.Item1,
                    LatestPickup = times.Item2,
                    TimeOfWalk = walk.Duration * 60
                });
            });

            List<WalkerPathLink> graphLinks;
            DogoWaypoint dogoHome;
            List<DogoWaypoint> graphNodes;
            GetAlgorithmData(homeLocation, dogWalkDetails, out graphNodes, out graphLinks, out dogoHome);
            return new SmartGreedyAlgorithm().CalculatePath(dogoHome, graphNodes, dogWalkDetails, graphLinks);
        }


        public static Tuple<TimeSpan,TimeSpan> getEarliestAndLatestPickup(DogOwner dogOwner)
        {
            TimeSpan earliest = TimeSpan.MaxValue;
            TimeSpan latest = TimeSpan.MinValue;

            if (dogOwner.isComfortable6To8)
            {
                earliest = new TimeSpan(6, 0, 0);
                latest = new TimeSpan(8, 0, 0);
            }
            if (dogOwner.isComfortable8To10)
            {
                var localEarliest = new TimeSpan(8, 0, 0);
                if (localEarliest < earliest)
                {
                    earliest = localEarliest;
                }
                var localLatest = new TimeSpan(10, 0, 0);
                if (localLatest > latest)
                {
                    latest = localLatest;
                }
            }
            if (dogOwner.isComfortable10To12)
            {
                var localEarliest = new TimeSpan(10, 0, 0);
                if (localEarliest < earliest)
                {
                    earliest = localEarliest;
                }
                var localLatest = new TimeSpan(12, 0, 0);
                if (localLatest > latest)
                {
                    latest = localLatest;
                }
            }
            if (dogOwner.isComfortable12To14)
            {
                var localEarliest = new TimeSpan(12, 0, 0);
                if (localEarliest < earliest)
                {
                    earliest = localEarliest;
                }
                var localLatest = new TimeSpan(14, 0, 0);
                if (localLatest > latest)
                {
                    latest = localLatest;
                }
            }
            if (dogOwner.isComfortable14To16)
            {
                var localEarliest = new TimeSpan(14, 0, 0);
                if (localEarliest < earliest)
                {
                    earliest = localEarliest;
                }
                var localLatest = new TimeSpan(16, 0, 0);
                if (localLatest > latest)
                {
                    latest = localLatest;
                }
            }
            if (dogOwner.isComfortable16To18)
            {
                var localEarliest = new TimeSpan(16, 0, 0);
                if (localEarliest < earliest)
                {
                    earliest = localEarliest;
                }
                var localLatest = new TimeSpan(18, 0, 0);
                if (localLatest > latest)
                {
                    latest = localLatest;
                }
            }
            if (dogOwner.isComfortable18To20)
            {
                var localEarliest = new TimeSpan(18, 0, 0);
                if (localEarliest < earliest)
                {
                    earliest = localEarliest;
                }
                var localLatest = new TimeSpan(20, 0, 0);
                if (localLatest > latest)
                {
                    latest = localLatest;
                }
            }
            if (dogOwner.isComfortable20To22)
            {
                var localEarliest = new TimeSpan(20, 0, 0);
                if (localEarliest < earliest)
                {
                    earliest = localEarliest;
                }
                var localLatest = new TimeSpan(22, 0, 0);
                if (localLatest > latest)
                {
                    latest = localLatest;
                }
            }

            return new Tuple<TimeSpan, TimeSpan>(earliest, latest);
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