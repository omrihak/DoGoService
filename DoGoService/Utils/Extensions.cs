using DoGoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoGoService.Utils
{
    public static class Extensions
    {
        public static TimeSpan GetEarliestAvailableTime(this DogOwner owner)
        {
            var times = new[] { owner.isComfortable6To8,
                               owner.isComfortable8To10,
                               owner.isComfortable10To12,
                               owner.isComfortable12To14,
                               owner.isComfortable14To16,
                               owner.isComfortable16To18,
                               owner.isComfortable18To20,
                               owner.isComfortable20To22};

            for (int i = 0; i < times.Count(); i++)
            {
                if (times[i])
                {
                    return new TimeSpan((i * 2) + 6, 0, 0);
                }
            }

            return TimeSpan.MinValue;
        }

        public static TimeSpan GetLatestAvailableTime(this DogOwner owner)
        {
            var times = new[] { owner.isComfortable6To8,
                               owner.isComfortable8To10,
                               owner.isComfortable10To12,
                               owner.isComfortable12To14,
                               owner.isComfortable14To16,
                               owner.isComfortable16To18,
                               owner.isComfortable18To20,
                               owner.isComfortable20To22};

            for (int i = times.Count() - 1; i >= 0; i--)
            {
                if (times[i])
                {
                    return new TimeSpan((i * 2) + 8, 0, 0);
                }
            }

            return TimeSpan.MaxValue;
        }
    }
}