using System;
using System.Threading;

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

        private double _deviceNumber,_messageNumber;
        private Thread _hubDeviceThread, _hubMsgThread;


        public double GetDeviceNumber()
        {
            return _deviceNumber;
        }


        public double GetMessageNumber()
        {
            return _messageNumber;
        }

        public void Run()
        {
            _hubDeviceThread = new Thread(()=> Calculatesin());
            _hubMsgThread = new Thread(()=>Calculateln());
            _hubDeviceThread.Start();
            _hubMsgThread.Start();
            
            
        }

        void Calculatesin()
        {
            while (true)
            {
                for (double i = -10;; i += 0.1)
                {
                    _deviceNumber = 20*Math.Sin(i);
                    Thread.Sleep(100);
                }
            }
        }

        void Calculateln()
        {
            while (true)
            {
                for (double i = 0; ; i += 0.1)
                {
                    _messageNumber = Math.Tan(i);
                    Thread.Sleep(100);
                }
            }
        }
    }
}
