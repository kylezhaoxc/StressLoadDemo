using System.ComponentModel;
using System.Dynamic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StressLoadDemo.Helpers;
using StressLoadDemo.Model;

namespace StressLoadDemo.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    
    
    public class TabRequirementViewModel : ViewModelBase
    {
        private int totalDevice;
        private int messagePerMinute;
        private int testDuration;
        private string iothubrecommendation;
        private string vmRecommendation;
        private bool buttonEnabled;
        private IStressDataProvider dataProvider;
        private MainViewModel MainVM;
        private VmSku vmInfo;
        private HubSku hubInfo;
        /// <summary>
        /// Initializes a new instance of the TabDashboardViewModel class.
        /// </summary>
        public TabRequirementViewModel(IStressDataProvider provider,MainViewModel mainVM)
        {
            dataProvider = provider;
           
            iothubrecommendation = "Fill in every blank to get recommendation";
            vmRecommendation= "Fill in every blank to get recommendation";
            buttonEnabled = false;
        }

        public RelayCommand MoveOn=>new RelayCommand(
            () =>
            {
                RequirementMessage message = new RequirementMessage()
                {
                    IoTHubSize = hubInfo.UnitSize,
                    IoTHubUnitCount = hubInfo.UnitCount,
                    AzureVmSize = vmInfo.Size,
                    VmCount = vmInfo.VmCount,
                    MessagePerMinPerDevice = messagePerMinute,
                    NumberOfDevicePerVm = int.Parse(TotalDevice)/vmInfo.VmCount,
                    TestDuration = testDuration

                };

                Messenger.Default.Send<RequirementMessage>(message,"AppendRequirementParam");
                var mainvm = new ViewModelLocator().Main;
                mainvm.SelectedTabIndex = 1;
            }
            );
        public bool ButtonEnabled
        {
            get { return buttonEnabled; }
            set
            {
                buttonEnabled = value;
                RaisePropertyChanged();
            }
        }
        public string TotalDevice
        {
            get {return totalDevice.ToString();}
            set
            {
                int.TryParse(value, out totalDevice);
                TryActivateButton();
            }
        }
        public string MessageFreq
        {
            get { return messagePerMinute.ToString(); }
            set
            {
                int.TryParse(value,out messagePerMinute);
                TryActivateButton();
                RecommendHub(messagePerMinute);
            }
        }
        public string TestDuration
        {
            get
            { return testDuration.ToString(); }
            set
            {
                int.TryParse(value,out testDuration);
                TryActivateButton();
            }
        }

        public string HubSkuRecommendation
        {
            get { return iothubrecommendation;}
            set
            {
                iothubrecommendation = value;
                RaisePropertyChanged();
            }
        }
        public string VmSkuRecommendation
        {
            get { return vmRecommendation; }
            set
            {
                vmRecommendation = value;
                RaisePropertyChanged();
            }
        }

        public void RecommendHub(int messagePerminute)
        {
            hubInfo = SKUCalculator.CalculateHubSku(messagePerminute);
            HubSkuRecommendation = hubInfo.UnitSize.ToString() + " x " + hubInfo.UnitCount;
        }

        public void TryActivateButton()
        {
            if (testDuration != 0
                && messagePerMinute != 0
                && totalDevice != 0)
            {
                var totalMessageCount = testDuration*messagePerMinute*totalDevice;
                vmInfo = SKUCalculator.CalculateVmSku(totalMessageCount);
                VmSkuRecommendation = vmInfo.Size.ToString() + " x " + vmInfo.VmCount;
                ButtonEnabled = true;
            }
            else
            {
                ButtonEnabled = false;
            }
        }
    }
}