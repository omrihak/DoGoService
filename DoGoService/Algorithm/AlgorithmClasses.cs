using Google.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoGoService.Algorithm
{
    public class DogoMission
    {
        public string waypoint { get; set; }
        public int length { get; set; }
        public MissionType type { get; set; }
    }

    public class DogWalk
    {
        public int TimeOfWalk { get; set; }
        public int DogUserId { get; set; }
    }

    public class AlgorithmData
    {
        public List<DogWalk> DogWalks { get; set; }
        public string HomeLocation { get; set; }
    }

    public class DogWalkDetails
    {
        public int TimeOfWalk { get; set; }
        public string Address { get; set; }
        public TimeSpan EarliestPickup { get; set; }
    }

    public enum MissionType
    {
        WalkPickup,
        WalkReturn,
        Walk,
        WaitReturn,
        Start
    }

    public enum AlgorithmType
    {
        SmartGreedy,
        Random,
        BruteForce,
        Greedy,
        Greedy2,
        Genetic
    }

    public class Mission
    {
        public Queue<DogoMission> trail { get; set; }
        public int GlobalTime { get; set; }
    }

    public class DogoWaypoint : ICloneable
    {
        public string address { get; set; }
        public bool isReturnWaypoint { get; set; }
        public bool isPassed { get; set; }
        public TimeSpan timeOfFirstWaypointPass { get; set; }

        public object Clone()
        {
            var dogoWaypoint = new DogoWaypoint();
            dogoWaypoint.address = address;
            dogoWaypoint.isReturnWaypoint = isReturnWaypoint;
            dogoWaypoint.isPassed = isPassed;
            dogoWaypoint.timeOfFirstWaypointPass = timeOfFirstWaypointPass;
            return dogoWaypoint;
        }
    }

    public class WaypointLink
    {
        public string sourceWaypoint { get; set; }
        public string destWaypoint { get; set; }
        public int duration { get; set; }
    }

    public class AlgorithmAnswer
    {
        public Queue<DogoMission> trail { get; set; }
        public TimeSpan startTime { get; set; }
    }

    public class DogoLink : ICloneable
    {
        public DogoWaypoint sourceWaypoint { get; set; }
        public DogoWaypoint destWaypoint { get; set; }
        public int duration { get; set; }
        public int realDuration { get; set; }

        public object Clone()
        {
            var dogoLink = new DogoLink();
            dogoLink.sourceWaypoint = sourceWaypoint;
            dogoLink.destWaypoint = destWaypoint;
            dogoLink.duration = duration;
            dogoLink.realDuration = realDuration;
            return dogoLink;
        }
    }
}