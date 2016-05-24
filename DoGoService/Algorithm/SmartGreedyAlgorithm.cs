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

        public AlgorithmAnswer DoAlgorithm(DogoWaypoint dogoHome, List<DogoWaypoint> graphNodes, List<DogWalkDetails> dogWalkDetails, List<DogoLink> graphLinks)
        {
            startTime = DateTime.Now - DateTime.Today;
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
                        link.duration = Math.Max(link.realDuration,  getTimeOfWalk(dogWalkDetails, node.address) + (secondNode.timeOfFirstWaypointPass - GlobalTime).Seconds);
                    });
                }
            }

            var links = graphLinks.Where(link => link.sourceWaypoint == node && !link.destWaypoint.isPassed && link.duration != int.MaxValue &&
                    dogWalkDetails.First(walk => walk.Address == link.destWaypoint.address).EarliestPickup > GlobalTime.Add(new TimeSpan(0, 0, link.realDuration)));
            var addDelayToStart = false;
            if (links.Count() == 0)
            {
                var linksWithDelay = graphLinks.Where(link => link.sourceWaypoint == node && !link.destWaypoint.isPassed && link.duration != int.MaxValue);
                if (linksWithDelay.Count() > 0)
                {
                    addDelayToStart = true;
                    links = linksWithDelay;
                }
            }

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
                if (addDelayToStart)
                {
                    startTime = startTime.Add(dogWalkDetails.First(walk => walk.Address == minLink.destWaypoint.address).EarliestPickup - GlobalTime);
                }

                if (minLink.realDuration == minLink.duration)
                {
                    trail.Enqueue(new DogoMission() { waypoint = minLink.destWaypoint.address, length = minLink.realDuration, type = minLink.destWaypoint.isReturnWaypoint ? MissionType.WalkReturn : MissionType.WalkPickup });
                }
                else
                {
                    trail.Enqueue(new DogoMission() { waypoint = minLink.destWaypoint.address, length = minLink.duration - minLink.realDuration, type = MissionType.WaitReturn });

                }
                node.isPassed = true;


                graphLinks.Where(link => link.destWaypoint.isReturnWaypoint).ToList().ForEach(link => {

                    if (link.destWaypoint.timeOfFirstWaypointPass.Seconds != -1)
                    {
                        link.duration = Math.Max(link.realDuration,  getTimeOfWalk(dogWalkDetails, link.destWaypoint.address) + (link.destWaypoint.timeOfFirstWaypointPass - GlobalTime).Seconds);
                    }
                    else
                    {
                        link.duration = int.MaxValue;
                    }
                });

                DoRecursive(minLink.destWaypoint, graphNodes, dogWalkDetails, graphLinks);
            }
        }
    }
}
