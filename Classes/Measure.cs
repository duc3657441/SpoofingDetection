using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoofingDetectionWinformApp.Classes
{
    // Measure spent time as nano seconds.
    public class Measure
    {
        private long __time_ns;
        private long __spent_time_ns;

        public Measure() 
        {
            __time_ns = 0;
            __spent_time_ns = 0;
        }

        public void Begin()
        {
            __time_ns = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000000;
            __spent_time_ns = 0;
        }

        public void End()
        {
            __spent_time_ns = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000000) - __time_ns;
        }

        public long GetSpentTimeNs() 
        {
            return __spent_time_ns;
        }
    }
}
