using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using StressLoadDemo.Model;

namespace StressLoadDemo.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TabDashboardViewModel : ViewModelBase
    {
        private IStressDataProvider dataProvider;
        private string hubOwnerConnectionString;
        private string eventHubConnectionString;
        private string batchServiceUrl;
        private string batchAccountKey;
        private string storageAccountConnectionString;
        private Visibility summaryVisibility;
        private bool canStartTest;

        /// <summary>
        /// Initializes a new instance of the TabDashboardViewModel class.
        /// </summary>
        public TabDashboardViewModel(IStressDataProvider provider)
        {
            Messenger.Default.Register<IStressDataProvider>(
                this,
                "StartTest",
                data=> ProcessRunConfigValue(data)
                );
            dataProvider = provider;
            summaryVisibility=Visibility.Hidden;
            canStartTest = false;
        }

        #region BindingProperties

        public Visibility SummaryVisibility
        {
            get { return summaryVisibility; }
            set
            {
                summaryVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool CanStartTest
        {
            get
            {
                return canStartTest;
            }
            set
            {
                canStartTest = value;
                RaisePropertyChanged();
            }
        }

        public string HubOwnerConnectionString
        {
            get { return hubOwnerConnectionString;}
            set
            {
                hubOwnerConnectionString = value;
                RaisePropertyChanged();
                TryActivateButton();
            }
        }

        public string EventHubConnectionString
        {
            get
            {
                return eventHubConnectionString;
            }
            set
            {
                eventHubConnectionString = value;
                RaisePropertyChanged();
                TryActivateButton();

            }
        }

        public string BatchServiceUrl
        {
            get { return batchServiceUrl;}
            set
            {
                batchServiceUrl = value;
                RaisePropertyChanged();
                TryActivateButton();
            }
        }

        public string BatchAccountKey
        {
            get { return batchAccountKey;}
            set
            {
                batchAccountKey = value;
                RaisePropertyChanged();
                TryActivateButton();
            }
        }

        public string StorageAccountConnectionString
        {
            get { return storageAccountConnectionString;}
            set
            {
                storageAccountConnectionString = value;
                RaisePropertyChanged();
                TryActivateButton();
            }
        }
        #endregion
        public RelayCommand StartTest => new RelayCommand(RunTest);

        void RunTest()
        {
            new ViewModelLocator().Main.TestStart = true;
            SummaryVisibility = Visibility.Visible; 
        }

        void ProcessRunConfigValue(IStressDataProvider provider)
        {
            provider.BatchKey = batchAccountKey;
            provider.HubOwnerConectionString = hubOwnerConnectionString;
            provider.EventHubConectionString = eventHubConnectionString;
            provider.BatchUrl = batchServiceUrl;
            provider.StorageAccountConectionString = storageAccountConnectionString;
        }
        void TryActivateButton()
        {
            if (!(string.IsNullOrEmpty(hubOwnerConnectionString)||
                string.IsNullOrEmpty(eventHubConnectionString)||
                string.IsNullOrEmpty(batchAccountKey)||
                string.IsNullOrEmpty(batchServiceUrl)||
                string.IsNullOrEmpty(storageAccountConnectionString))
                )
            {
                CanStartTest=true;
            }
            else
            {
               CanStartTest=false;
            }
        }
    }
}