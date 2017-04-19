using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressLoadDemo.Model
{
    public class RequirementMessage
    {
        public int IoTHubUnitCount { get; set; }
        public HubSize IoTHubSize { get; set; }
        public int VmCount { get; set; }
        public VMSize AzureVmSize { get; set; }
        public int MessagePerMinPerDevice { get; set; }
        public int NumberOfDevicePerVm { get; set; }
        public int TestDuration { get; set; }
    }
}
