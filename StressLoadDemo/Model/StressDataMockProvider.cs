using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Converters;
using GalaSoft.MvvmLight.Messaging;

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

        private double deviceNumber,messageNumber;
        private Thread hubDeviceThread, hubMsgThread;


        public double GetDeviceNumber()
        {
            return deviceNumber;
        }


        public double GetMessageNumber()
        {
            return messageNumber;
        }

        public void Run()
        {
            hubDeviceThread = new Thread(()=> calculatesin());
            hubMsgThread = new Thread(()=>calculateln());
            hubDeviceThread.Start();
            hubMsgThread.Start();
            
            
        }

        void calculatesin()
        {
            while (true)
            {
                for (double i = -10;; i += 0.1)
                {
                    deviceNumber = 20*Math.Sin(i);
                    Thread.Sleep(100);
                }
            }
        }

        void calculateln()
        {
            while (true)
            {
                for (double i = 0; ; i += 0.1)
                {
                    messageNumber = Math.Tan(i);
                    Thread.Sleep(100);
                }
            }
        }
    }
}
