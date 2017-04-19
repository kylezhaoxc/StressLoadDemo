using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StressLoadDemo.Model;

namespace StressLoadDemo.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private readonly IStressDataProvider dataProvider;

        private int selectedTabIndex;
        private bool testStart;
        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                selectedTabIndex = value;
                RaisePropertyChanged();
            }
        }

        public bool TestStart
        {
            get { return testStart;}
            set
            {
                if (value)
                {
                    Messenger.Default.Send<IStressDataProvider>(dataProvider, "StartTest");
                }
            }
        }
        
        public MainViewModel(IStressDataProvider provider)
        {
            dataProvider = provider;
            Messenger.Default.Register<RequirementMessage>(
                this,
                "AppendRequirementParam",
                message => AppendToProvider(message)
                );


        }

        public void AppendToProvider(RequirementMessage message)
        {
            dataProvider.NumOfVm = message.VmCount.ToString();
            dataProvider.DevicePerVm = message.NumberOfDevicePerVm.ToString();
            dataProvider.ExpectTestDuration = message.TestDuration.ToString();
            dataProvider.MessagePerMinute = message.MessagePerMinPerDevice;
            dataProvider.VmSize = message.AzureVmSize.ToString();
        }
    }
}