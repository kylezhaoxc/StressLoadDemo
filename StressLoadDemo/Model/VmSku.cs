namespace StressLoadDemo.Model
{
    public enum VmSize
    {
        Small,
        Medium,
        Large,
        Extralarge
    }
    public class VmSku
    {
        public VmSize Size;
        public int VmCount;
    }
}
