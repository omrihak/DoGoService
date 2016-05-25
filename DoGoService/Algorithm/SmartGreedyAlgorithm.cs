using Google.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoGoService.Algorithm
{
    public class SmartGreedyAlgorithm
    {
        private TimeSpan GlobalTime;
        private Queue<DogoMission> trail;
        private double averageDuration;
        private TimeSpan startTime;
        private TimeSpan latestStart;

        public AlgorithmAnswer DoAlgorithm(DogoWaypoint dogoHome, List<DogoWaypoint> graphNodes, List<DogWalkDetails> dogWalkDetails, List<DogoLink> graphLinks)
        {
            startTime = DateTime.Now - DateTime.Today;
            latestStart = TimeSpan.MaxValue;
            GlobalTime = startTime;
            trail = new Queue<DogoMission>();
            trail.Enqueue(new DogoMission() { waypoint = dogoHome.address, length = 0, type = MissionType.Start });
            dogoHome.isPassed = true;
            averageDuration = dogWalkDetails.Select(walk=>walk.TimeOfWalk).Average();

            DoRecursive(dogoHome, graphNodes, dogWalkDetails, graphLinks);

            return new AlgorithmAnswer() { trail = trail, startTime = startTime };
        }

        private int getTimeOfWalk(List<DogWalkDetails> dogWalkDetails, string address)
        {
            return dogWalkDetails.First(walk => walk.Address == address).TimeOfWalk;
        }

        public void DoRecursive(DogoWaypoint node, List<DogoWaypoint> graphNodes, List<DogWalkDetails> dogWalkDetails, List<DogoLink> graphLinks)
        {
            if (!node.isReturnWaypoint)
            {
                var secondNode = graphNodes.FirstOrDefault(waypoint => waypoint.isReturnWaypoint && waypoint.address == node.address);
                if (secondNode != null)
                {
                    secondNode.timeOfFirstWaypointPass = GlobalTime;
                    graphLinks.Where(link => link.destWaypoint == secondNode).ToList().ForEach(link => {
                        link.duration = (int)Math.Max(link.realDuration,  getTimeOfWalk(dogWalkDetails, node.address) + (secondNode.timeOfFirstWaypointPass - GlobalTime).TotalSeconds);
                    });
                }
            }

            var links = graphLinks.Where(link => link.sourceWaypoint == node && !link.destWaypoint.isPassed && link.duration != int.MaxValue);

            if (links.Count() > 0)
            {
                DogoLink minLink = links.First();
                double minLinkValue = minLink.destWaypoint.isReturnWaypoint ? 
                                            minLink.duration : 
                                            minLink.duration - Math.Max(((getTimeOfWalk(dogWalkDetails, minLink.destWaypoint.address) - averageDuration) / 2), 0);

                foreach (var link in links)
                {
                    double value = link.destWaypoint.isReturnWaypoint ? link.duration : link.duration - Math.Max(((getTimeOfWalk(dogWalkDetails, link.destWaypoint.address) - averageDuration) / 2), 0);
                    if (value < minLinkValue)
                    {
                        minLinkValue = value;
                        minLink = link;
                    }
                }

                GlobalTime = GlobalTime.Add(new TimeSpan(0, 0, minLink.duration));
                var currentWalk = dogWalkDetails.First(walk => walk.Address == minLink.destWaypoint.address);
                if (minLink.realDuration != minLink.duration)
                {
                    var isTooEarly = currentWalk.EarliestPickup > GlobalTime;

                    if (isTooEarly && !minLink.destWaypoint.isReturnWaypoint)
                    {
                        GlobalTime = GlobalTime.Add(new TimeSpan(0, 0, -minLink.duration));
                        GlobalTime = GlobalTime.Add(new TimeSpan(0, 0, minLink.realDuration));
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
                            trail.Enqueue(new DogoMission() { length = (int)waitTime.TotalSeconds, type = MissionType.Wait });
                            GlobalTime = GlobalTime.Add(waitTime);
                        }
                    }
                    else
                    {
                        trail.Enqueue(new DogoMission() { length = minLink.duration - minLink.realDuration, type = MissionType.Wait });
                    }
                }

                trail.Enqueue(new DogoMission() { waypoint = minLink.destWaypoint.address, length = minLink.realDuration, type = minLink.destWaypoint.isReturnWaypoint ? MissionType.WalkReturn : MissionType.WalkPickup });

                if(!minLink.destWaypoint.isReturnWaypoint)
                {
                    var latestStartLocal = startTime + (currentWalk.LatestPickup - GlobalTime);
                    if (latestStart > latestStartLocal)
                    {
                        latestStart = latestStartLocal;
                    }
                }
               
                node.isPassed = true;


                graphLinks.Where(link => link.destWaypoint.isReturnWaypoint).ToList().ForEach(link => {

                    if (link.destWaypoint.timeOfFirstWaypointPass.TotalSeconds != -1)
                    {
                        link.duration = (int)Math.Max(link.realDuration,  getTimeOfWalk(dogWalkDetails, link.destWaypoint.address) + (link.destWaypoint.timeOfFirstWaypointPass - GlobalTime).TotalSeconds);
                    }
                    else
                    {
                        link.duration = int.MaxValue;
                    }
                });

                graphLinks.Where(link => !link.destWaypoint.isReturnWaypoint).ToList().ForEach(link =>
                {
                    var currWalk = dogWalkDetails.FirstOrDefault(walk => walk.Address == link.destWaypoint.address);
                    link.duration = (int)Math.Max(link.realDuration, currWalk != null ? ( currWalk.EarliestPickup - GlobalTime).TotalSeconds : 0);
                });

                DoRecursive(minLink.destWaypoint, graphNodes, dogWalkDetails, graphLinks);
            }
        }
    }
}
