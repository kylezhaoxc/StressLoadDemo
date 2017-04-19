using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressLoadDemo.Model
{
    public enum HubSize
    {
        S1,
        S2,
        S3
    }
    public struct HubSku
    {
        public HubSize UnitSize;
        public int UnitCount;

    }
}
