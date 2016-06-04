using DoGoService.Paths.Enums;
using DoGoService.Paths.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoGoService.Paths
{
    public class SmartGreedyAlgorithm
    {
        private TimeSpan GlobalTime;
        private Queue<PathNode> trail;
        private double averageDuration;
        private TimeSpan startTime;
        private TimeSpan latestStart;

        public WalkerPath CalculatePath(DogoWaypoint dogoHome, List<DogoWaypoint> graphNodes, List<DogWalkDetails> dogWalkDetails, List<WalkerPathLink> graphLinks)
        {
            startTime = DateTime.Now - DateTime.Today;
            latestStart = TimeSpan.MaxValue;
            GlobalTime = startTime;
            trail = new Queue<PathNode>();
            trail.Enqueue(new PathNode(dogoHome.Address, 0, NodeAction.Start));
            dogoHome.IsPassed = true;
            averageDuration = dogWalkDetails.Select(walk=>walk.TimeOfWalk).Average();

            DoRecursive(dogoHome, graphNodes, dogWalkDetails, graphLinks);


            var lastLink = graphLinks.First(link => link.DestWaypoint.Address == dogoHome.Address && link.SourceWaypoint.Address == trail.Last().Waypoint);
            trail.Enqueue(new PathNode(dogoHome.Address, lastLink.Duration, NodeAction.Walk));
            return new WalkerPath() { Path = trail, StartTime = startTime };
        }

        private int GetWalkDuration(List<DogWalkDetails> dogWalkDetails, string address)
        {
            return dogWalkDetails.First(walk => walk.Address == address).TimeOfWalk;
        }

        private void DoRecursive(DogoWaypoint node, List<DogoWaypoint> graphNodes, List<DogWalkDetails> dogWalkDetails, List<WalkerPathLink> graphLinks)
        {
            if (!node.IsReturnWaypoint)
            {
                var secondNode = graphNodes.FirstOrDefault(waypoint => waypoint.IsReturnWaypoint && waypoint.Address == node.Address);
                if (secondNode != null)
                {
                    secondNode.TimeOfFirstWaypointPass = GlobalTime;
                    graphLinks.Where(link => link.DestWaypoint == secondNode).ToList().ForEach(link => {
                        link.Duration = (int)Math.Max(link.RealDuration,  GetWalkDuration(dogWalkDetails, node.Address) + (secondNode.TimeOfFirstWaypointPass - GlobalTime).TotalSeconds);
                    });
                }
            }

            var links = graphLinks.Where(link => link.SourceWaypoint == node && !link.DestWaypoint.IsPassed && link.Duration != int.MaxValue);

            if (links.Count() > 0)
            {
                WalkerPathLink minLink = links.First();
                double minLinkValue = minLink.DestWaypoint.IsReturnWaypoint ? 
                                            minLink.Duration : 
                                            minLink.Duration - Math.Max(((GetWalkDuration(dogWalkDetails, minLink.DestWaypoint.Address) - averageDuration) / 2), 0);

                foreach (var link in links)
                {
                    double value = link.DestWaypoint.IsReturnWaypoint ? link.Duration : link.Duration - Math.Max(((GetWalkDuration(dogWalkDetails, link.DestWaypoint.Address) - averageDuration) / 2), 0);
                    if (value < minLinkValue)
                    {
                        minLinkValue = value;
                        minLink = link;
                    }
                }

                GlobalTime = GlobalTime.Add(new TimeSpan(0, 0, minLink.Duration));
                var currentWalk = dogWalkDetails.First(walk => walk.Address == minLink.DestWaypoint.Address);
                if (minLink.RealDuration != minLink.Duration)
                {
                    var isTooEarly = currentWalk.EarliestPickup > GlobalTime;

                    if (isTooEarly && !minLink.DestWaypoint.IsReturnWaypoint)
                    {
                        GlobalTime = GlobalTime.Add(new TimeSpan(0, 0, -minLink.Duration));
                        GlobalTime = GlobalTime.Add(new TimeSpan(0, 0, minLink.RealDuration));
                        if (startTime + currentWalk.EarliestPickup - GlobalTime <= latestStart)
                        {
                            latestStart += currentWalk.EarliestPickup - GlobalTime;
                            startTime += currentWalk.EarliestPickup - GlobalTime;
                            GlobalTime += currentWalk.EarliestPickup - GlobalTime;
                        }
                        else
                        {
                            var waitTime = startTime + currentWalk.EarliestPickup - GlobalTime - latestStart;
                            GlobalTime += latestStart - startTime;
                            startTime = latestStart;
                            trail.Enqueue(new PathNode((int)waitTime.TotalSeconds,NodeAction.Wait));
                            GlobalTime = GlobalTime.Add(waitTime);
                        }
                    }
                    else
                    {
                        trail.Enqueue(new PathNode(minLink.Duration - minLink.RealDuration, NodeAction.Wait));
                    }
                }

                var missionType = minLink.DestWaypoint.IsReturnWaypoint ? NodeAction.WalkReturn : NodeAction.WalkPickup;
                trail.Enqueue(new PathNode(minLink.DestWaypoint.Address, minLink.RealDuration, missionType));

                if(!minLink.DestWaypoint.IsReturnWaypoint)
                {
                    var latestStartLocal = startTime + (currentWalk.LatestPickup - GlobalTime);
                    if (latestStart > latestStartLocal)
                    {
                        latestStart = latestStartLocal;
                    }
                }
               
                node.IsPassed = true;
                graphLinks.Where(link => link.DestWaypoint.IsReturnWaypoint).ToList().ForEach(link => {

                    if (link.DestWaypoint.TimeOfFirstWaypointPass.TotalSeconds != -1)
                    {
                        link.Duration = (int)Math.Max(link.RealDuration,  GetWalkDuration(dogWalkDetails, link.DestWaypoint.Address) + (link.DestWaypoint.TimeOfFirstWaypointPass - GlobalTime).TotalSeconds);
                    }
                    else
                    {
                        link.Duration = int.MaxValue;
                    }
                });

                graphLinks.Where(link => !link.DestWaypoint.IsReturnWaypoint).ToList().ForEach(link =>
                {
                    var currWalk = dogWalkDetails.FirstOrDefault(walk => walk.Address == link.DestWaypoint.Address);
                    link.Duration = (int)Math.Max(link.RealDuration, currWalk != null ? ( currWalk.EarliestPickup - GlobalTime).TotalSeconds : 0);
                });

                DoRecursive(minLink.DestWaypoint, graphNodes, dogWalkDetails, graphLinks);
            }
        }
    }
}
