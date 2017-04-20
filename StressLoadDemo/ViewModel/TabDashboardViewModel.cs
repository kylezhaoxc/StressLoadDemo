using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
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
        //max length of data buffered for drawing graph.
        //Hardcoded to the width of the canvas.
        private const double CanvasWidth = 265;
        private const double CanvasHeight = 111;

        private IStressDataProvider dataProvider;

        private string hubOwnerConnectionString;
        private string eventHubConnectionString;
        private string batchServiceUrl;
        private string batchAccountKey;
        private string storageAccountConnectionString;
        private Visibility summaryVisibility;
        private bool canStartTest;
        private System.Timers.Timer refreshDataTimer;

        private double deviceRealTimeNumber, messageRealTimeNumber;
        private ObservableCollection<MyLine> deviceLines,messageLines;
        private List<MyLine> deviceLineBuffer,messageLineBuffer;
        private Queue<double> deviceNumberBuffer,messageNumberBuffer;
        /// <summary>
        /// Initializes a new instance of the TabDashboardViewModel class.
        /// </summary>
        public TabDashboardViewModel(IStressDataProvider provider)
        {
            Messenger.Default.Register<IStressDataProvider>(
                this,
                "StartTest",
                data=> processRunConfigValue(data)
                );
            dataProvider = provider;
            summaryVisibility=Visibility.Hidden;
            canStartTest = false;
            refreshDataTimer = new System.Timers.Timer();
            refreshDataTimer.Elapsed += observeData;
            refreshDataTimer.AutoReset = true;
            //fetch data and refresh UI 1 time/sec
            refreshDataTimer.Interval = 300;
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

        public ObservableCollection<MyLine> MessageLines
        {
            get { return messageLines; }
            set
            {
                messageLines = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<MyLine> DeviceLines
        {
            get
            {
                return deviceLines;
            }
            set
            {
                deviceLines = value;
                RaisePropertyChanged();
            }
        }

        public double MessageRealTimeNumber
        {
            get { return messageRealTimeNumber; }
            set
            {
                messageRealTimeNumber = value;
                RaisePropertyChanged();
            }
        }

        public double DeviceRealTimeNumber
        {
            get { return deviceRealTimeNumber; }
            set
            {
                deviceRealTimeNumber = value;
                RaisePropertyChanged();
            }
        }
        void observeData(Object source, System.Timers.ElapsedEventArgs e)
        {
            DeviceRealTimeNumber = dataProvider.GetDeviceNumber();
            MessageRealTimeNumber = dataProvider.GetMessageNumber();
            messageNumberBuffer.Enqueue(messageRealTimeNumber);
            deviceNumberBuffer.Enqueue(DeviceRealTimeNumber);
            if (deviceNumberBuffer.Count > CanvasWidth)
            {
                deviceNumberBuffer.Dequeue();
            }
            if (messageNumberBuffer.Count > CanvasWidth)
            {
                messageNumberBuffer.Dequeue();
            }
            transformDataToLines(deviceNumberBuffer.ToList(),ref deviceLineBuffer);
            transformDataToLines(messageNumberBuffer.ToList(), ref messageLineBuffer);

            DeviceLines = new ObservableCollection<MyLine>(deviceLineBuffer);
            MessageLines = new ObservableCollection<MyLine>(messageLineBuffer);
        }

        void transformDataToLines(List<double> data,ref List<MyLine> targetLines)
        {
            targetLines = new List<MyLine>();
            var maxY = data.Max();
            var rangeY = maxY - data.Min();
            var scaleY = CanvasHeight / rangeY;
            var verticalShift = maxY > 0 ? scaleY * maxY : -scaleY * maxY;
            var xUnit = CanvasWidth / (data.Count - 1);
            double prevX = 0, prevY = data[0];
            var temp = new List<MyLine>();
            data.ForEach(p =>
            {
                p = verticalShift - p * scaleY;
                if (data.Count > 1) {
                    temp.Add(new MyLine() { X1 = prevX, Y1 = prevY, X2 = prevX + xUnit, Y2 = p });
                }
                prevX += xUnit;
                prevY = p;
            });
            targetLines = temp;
        }
        void processRunConfigValue(IStressDataProvider provider)
        {
            provider.BatchKey = batchAccountKey;
            provider.HubOwnerConectionString = hubOwnerConnectionString;
            provider.EventHubConectionString = eventHubConnectionString;
            provider.BatchUrl = batchServiceUrl;
            provider.StorageAccountConectionString = storageAccountConnectionString;
            provider.Run();

            DeviceLines=new ObservableCollection<MyLine>();
            MessageLines= new ObservableCollection<MyLine>();
            deviceNumberBuffer = new Queue<double>();
            messageNumberBuffer = new Queue<double>();
            refreshDataTimer.Enabled = true;
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