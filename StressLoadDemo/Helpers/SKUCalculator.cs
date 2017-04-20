using System;
using StressLoadDemo.Model;

namespace StressLoadDemo.Helpers
{
    public static class SkuCalculator
    {
        public const double IothubS1Speed = 720;
        public const double IothubS2Speed = 7200;
        public const double IothubS3Speed = 360000;

        public const double VmSmallCapacity = 10000;
        public const double VmMediumCapacity = 20000;
        public const double VmLargeCapacity = 40000;
        public const double VmExtralargeCapacity = 80000;

        public static HubSku CalculateHubSku(int messagePerMinute)
        {
            HubSku sku = new HubSku();
            if (messagePerMinute < IothubS2Speed)
            {
                sku.UnitSize=HubSize.S1;
                sku.UnitCount = (int) Math.Ceiling(messagePerMinute/IothubS1Speed);
            }
            else if (messagePerMinute < IothubS3Speed)
            {
                sku.UnitSize = HubSize.S2;
                sku.UnitCount = (int) Math.Ceiling(messagePerMinute/IothubS2Speed);
            }
            else
            {
                sku.UnitSize=HubSize.S3;
                sku.UnitCount= (int)Math.Ceiling(messagePerMinute / IothubS3Speed);
            }
            return sku;
        }

        public static VmSku CalculateVmSku(int totalMessage)
        {
            var sku = new VmSku();
            if (totalMessage > VmExtralargeCapacity)
            {
                sku.Size=VmSize.Extralarge;
                sku.VmCount = (int) Math.Ceiling(totalMessage/VmExtralargeCapacity);
            }
            else if (totalMessage > VmLargeCapacity)
            {
                sku.Size=VmSize.Large;
                sku.VmCount= (int)Math.Ceiling(totalMessage / VmLargeCapacity);
            }
            else if (totalMessage > VmMediumCapacity)
            {
                sku.Size = VmSize.Medium;
                sku.VmCount = (int) Math.Ceiling(totalMessage/VmMediumCapacity);
            }
            else
            {
                sku.Size=VmSize.Small;
                sku.VmCount = (int) Math.Ceiling(totalMessage/VmSmallCapacity);

            }
            return sku;
        } 
    }
}
