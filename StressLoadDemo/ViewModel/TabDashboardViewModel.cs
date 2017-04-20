using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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

        private readonly IStressDataProvider _dataProvider;

        private string _hubOwnerConnectionString;
        private string _eventHubConnectionString;
        private string _batchServiceUrl;
        private string _batchAccountKey;
        private string _storageAccountConnectionString;
        private Visibility _summaryVisibility;
        private bool _canStartTest;
        private readonly System.Timers.Timer _refreshDataTimer;

        private double _deviceRealTimeNumber, _messageRealTimeNumber;
        private ObservableCollection<MyLine> _deviceLines,_messageLines;
        private List<MyLine> _deviceLineBuffer,_messageLineBuffer;
        private Queue<double> _deviceNumberBuffer,_messageNumberBuffer;
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
            _dataProvider = provider;
            _summaryVisibility=Visibility.Hidden;
            _canStartTest = false;
            _refreshDataTimer = new System.Timers.Timer();
            _refreshDataTimer.Elapsed += ObserveData;
            _refreshDataTimer.AutoReset = true;
            //fetch data and refresh UI 1 time/sec
            _refreshDataTimer.Interval = 300;
        }

        #region BindingProperties

        public Visibility SummaryVisibility
        {
            get { return _summaryVisibility; }
            set
            {
                _summaryVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool CanStartTest
        {
            get
            {
                return _canStartTest;
            }
            set
            {
                _canStartTest = value;
                RaisePropertyChanged();
            }
        }

        public string HubOwnerConnectionString
        {
            get { return _hubOwnerConnectionString;}
            set
            {
                _hubOwnerConnectionString = value;
                RaisePropertyChanged();
                TryActivateButton();
            }
        }

        public string EventHubConnectionString
        {
            get
            {
                return _eventHubConnectionString;
            }
            set
            {
                _eventHubConnectionString = value;
                RaisePropertyChanged();
                TryActivateButton();

            }
        }

        public string BatchServiceUrl
        {
            get { return _batchServiceUrl;}
            set
            {
                _batchServiceUrl = value;
                RaisePropertyChanged();
                TryActivateButton();
            }
        }

        public string BatchAccountKey
        {
            get { return _batchAccountKey;}
            set
            {
                _batchAccountKey = value;
                RaisePropertyChanged();
                TryActivateButton();
            }
        }

        public string StorageAccountConnectionString
        {
            get { return _storageAccountConnectionString;}
            set
            {
                _storageAccountConnectionString = value;
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
            get { return _messageLines; }
            set
            {
                _messageLines = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<MyLine> DeviceLines
        {
            get
            {
                return _deviceLines;
            }
            set
            {
                _deviceLines = value;
                RaisePropertyChanged();
            }
        }

        public double MessageRealTimeNumber
        {
            get { return _messageRealTimeNumber; }
            set
            {
                _messageRealTimeNumber = value;
                RaisePropertyChanged();
            }
        }

        public double DeviceRealTimeNumber
        {
            get { return _deviceRealTimeNumber; }
            set
            {
                _deviceRealTimeNumber = value;
                RaisePropertyChanged();
            }
        }
        void ObserveData(Object source, System.Timers.ElapsedEventArgs e)
        {
            DeviceRealTimeNumber = _dataProvider.GetDeviceNumber();
            MessageRealTimeNumber = _dataProvider.GetMessageNumber();
            _messageNumberBuffer.Enqueue(_messageRealTimeNumber);
            _deviceNumberBuffer.Enqueue(DeviceRealTimeNumber);
            if (_deviceNumberBuffer.Count > CanvasWidth)
            {
                _deviceNumberBuffer.Dequeue();
            }
            if (_messageNumberBuffer.Count > CanvasWidth)
            {
                _messageNumberBuffer.Dequeue();
            }
            TransformDataToLines(_deviceNumberBuffer.ToList(),ref _deviceLineBuffer);
            TransformDataToLines(_messageNumberBuffer.ToList(), ref _messageLineBuffer);

            DeviceLines = new ObservableCollection<MyLine>(_deviceLineBuffer);
            MessageLines = new ObservableCollection<MyLine>(_messageLineBuffer);
        }

        void TransformDataToLines(List<double> data,ref List<MyLine> targetLines)
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
        void ProcessRunConfigValue(IStressDataProvider provider)
        {
            provider.BatchKey = _batchAccountKey;
            provider.HubOwnerConectionString = _hubOwnerConnectionString;
            provider.EventHubConectionString = _eventHubConnectionString;
            provider.BatchUrl = _batchServiceUrl;
            provider.StorageAccountConectionString = _storageAccountConnectionString;
            provider.Run();

            DeviceLines=new ObservableCollection<MyLine>();
            MessageLines= new ObservableCollection<MyLine>();
            _deviceNumberBuffer = new Queue<double>();
            _messageNumberBuffer = new Queue<double>();
            _refreshDataTimer.Enabled = true;
        }

        void TryActivateButton()
        {
            if (!(string.IsNullOrEmpty(_hubOwnerConnectionString)||
                string.IsNullOrEmpty(_eventHubConnectionString)||
                string.IsNullOrEmpty(_batchAccountKey)||
                string.IsNullOrEmpty(_batchServiceUrl)||
                string.IsNullOrEmpty(_storageAccountConnectionString))
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