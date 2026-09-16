using TacoStationMonitor.Models;
using TacoStationMonitor.Service;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace TacoStationMonitor.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly DashboardService service = new();

        private readonly DispatcherTimer timer;

        public DashboardViewModel()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");

            DashboardState data = await service.GetDashboardData();

            if (data == null)
                return;

            PartID = data.PartID;
            NextPartID = data.NextPartID;
            ChildPartNo = data.ChildPartNo;
            StatusString = data.StatusString;
            MatNo = data.MatNo;
            Station = data.StationName;
        }

        private int partID;
        public int PartID
        {
            get => partID;
            set
            {
                partID = value;
                OnPropertyChanged();
            }
        }
        private int nextPartID;
        public int NextPartID
        {
            get => nextPartID;
            set
            {
                nextPartID = value;
                OnPropertyChanged();
            }
        }

        private string childPartNo;
        public string ChildPartNo
        {
            get => childPartNo;
            set
            {
                childPartNo = value;
                OnPropertyChanged();
            }
        }

        private string statusString;
        public string StatusString
        {
            get => statusString;
            set
            {
                statusString = value;
                OnPropertyChanged();
            }
        }

        private string matNo;
        public string MatNo
        {
            get => matNo;
            set
            {
                matNo = value;
                OnPropertyChanged();
            }
        }


        private string station;
        public string Station
        {
            get => station;
            set
            {
                station = value;
                OnPropertyChanged();
            }
        }


        private string currentTime;
        public string CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(name));
        }
    }
}