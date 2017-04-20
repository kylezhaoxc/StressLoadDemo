namespace StressLoadDemo.Model
{
    public interface IStressDataProvider
    {
        //required parameters for running stressload.exe
        string HubOwnerConectionString { get; set; }
        string EventHubConectionString { get; set; }
        string StorageAccountConectionString { get; set; }
        string BatchUrl { get; set; }
        string BatchKey { get; set; }
        string DevicePerVm { get; set; }
        string NumOfVm { get; set; }
        string ExpectTestDuration { get; set; }
        int MessagePerMinute { get; set; }
        string VmSize { get; set; }
        
        double GetDeviceNumber();
        double GetMessageNumber();
        void Run();
    }
}
