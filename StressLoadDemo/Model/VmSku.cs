using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressLoadDemo.Model
{
    public enum VMSize
    {
        Small,
        Medium,
        Large,
        Extralarge
    }
    public class VmSku
    {
        public VMSize Size;
        public int VmCount;
    }
}
