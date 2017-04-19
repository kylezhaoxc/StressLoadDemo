using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressLoadDemo.Model
{
    public class StressDataMockProvider:IStressDataProvider
    {
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
