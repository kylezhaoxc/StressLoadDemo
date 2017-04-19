using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressLoadDemo.Model
{
    public interface IStressDataProvider
    {
        Task<int> GetDeviceNumber();
        Task<int> GetMessageNumber();

    }
}
