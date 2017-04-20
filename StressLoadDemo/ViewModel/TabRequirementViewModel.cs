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
        private int _totalDevice;
        private int _messagePerMinute;
        private int _testDuration;
        private string _iothubrecommendation;
        private string _vmRecommendation;
        private bool _buttonEnabled;
        private IStressDataProvider _dataProvider;
        private MainViewModel _mainVm;
        private VmSku _vmInfo;
        private HubSku _hubInfo;
        /// <summary>
        /// Initializes a new instance of the TabDashboardViewModel class.
        /// </summary>
        public TabRequirementViewModel(IStressDataProvider provider,MainViewModel mainVm)
        {
            _dataProvider = provider;
           
            _iothubrecommendation = "Fill in every blank to get recommendation";
            _vmRecommendation= "Fill in every blank to get recommendation";
            _buttonEnabled = false;
        }

        public RelayCommand MoveOn=>new RelayCommand(
            () =>
            {
                RequirementMessage message = new RequirementMessage()
                {
                    IoTHubSize = _hubInfo.UnitSize,
                    IoTHubUnitCount = _hubInfo.UnitCount,
                    AzureVmSize = _vmInfo.Size,
                    VmCount = _vmInfo.VmCount,
                    MessagePerMinPerDevice = _messagePerMinute,
                    NumberOfDevicePerVm = int.Parse(TotalDevice)/_vmInfo.VmCount,
                    TestDuration = _testDuration

                };

                Messenger.Default.Send<RequirementMessage>(message,"AppendRequirementParam");
                var mainvm = new ViewModelLocator().Main;
                mainvm.SelectedTabIndex = 1;
            }
            );
        public bool ButtonEnabled
        {
            get { return _buttonEnabled; }
            set
            {
                _buttonEnabled = value;
                RaisePropertyChanged();
            }
        }
        public string TotalDevice
        {
            get {return _totalDevice.ToString();}
            set
            {
                int.TryParse(value, out _totalDevice);
                TryActivateButton();
            }
        }
        public string MessageFreq
        {
            get { return _messagePerMinute.ToString(); }
            set
            {
                int.TryParse(value,out _messagePerMinute);
                TryActivateButton();
                RecommendHub(_messagePerMinute);
            }
        }
        public string TestDuration
        {
            get
            { return _testDuration.ToString(); }
            set
            {
                int.TryParse(value,out _testDuration);
                TryActivateButton();
            }
        }

        public string HubSkuRecommendation
        {
            get { return _iothubrecommendation;}
            set
            {
                _iothubrecommendation = value;
                RaisePropertyChanged();
            }
        }
        public string VmSkuRecommendation
        {
            get { return _vmRecommendation; }
            set
            {
                _vmRecommendation = value;
                RaisePropertyChanged();
            }
        }

        public void RecommendHub(int messagePerminute)
        {
            _hubInfo = SkuCalculator.CalculateHubSku(messagePerminute);
            HubSkuRecommendation = _hubInfo.UnitSize.ToString() + " x " + _hubInfo.UnitCount;
        }

        public void TryActivateButton()
        {
            if (_testDuration != 0
                && _messagePerMinute != 0
                && _totalDevice != 0)
            {
                var totalMessageCount = _testDuration*_messagePerMinute*_totalDevice;
                _vmInfo = SkuCalculator.CalculateVmSku(totalMessageCount);
                VmSkuRecommendation = _vmInfo.Size.ToString() + " x " + _vmInfo.VmCount;
                ButtonEnabled = true;
            }
            else
            {
                ButtonEnabled = false;
            }
        }
    }
}