using System;

namespace VueCore
{    
    public static class Extensions
    {

        public static double ToEpoch(this DateTime dateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = dateTime.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        public static double ToEpoch(this DateTimeOffset dateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = dateTime.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }        
    }
}