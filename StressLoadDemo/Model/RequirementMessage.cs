namespace StressLoadDemo.Model
{
    public class RequirementMessage
    {
        public int IoTHubUnitCount { get; set; }
        public HubSize IoTHubSize { get; set; }
        public int VmCount { get; set; }
        public VmSize AzureVmSize { get; set; }
        public int MessagePerMinPerDevice { get; set; }
        public int NumberOfDevicePerVm { get; set; }
        public int TestDuration { get; set; }
    }
}
