using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressLoadDemo.Model
{
    public class StressDataMockProvider:IStressDataProvider
    {
        public string HubOwnerConectionString { get; set; }
        public string EventHubConectionString { get; set; }
        public string StorageAccountConectionString { get; set; }
        public string BatchUrl { get; set; }
        public string BatchKey { get; set; }
        public string DevicePerVm { get; set; }
        public string NumOfVm { get; set; }
        public string ExpectTestDuration { get; set; }
        public int MessagePerMinute { get; set; }
        public string VmSize { get; set; }


        private static int querycount = 0;
        public Task<int> GetDeviceNumber()
        {
            return Task.FromResult(querycount++);
        }

        public Task<int> GetMessageNumber()
        {
            return Task.FromResult(2*querycount++);
        }
    }
}
