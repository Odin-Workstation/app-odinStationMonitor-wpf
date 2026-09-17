using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacoStationMonitor.ViewModels
{
    public partial class StationDashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _partID;

        [ObservableProperty]
        private int _nextPartID;

        [ObservableProperty]
        private string? _childPartNo;

        [ObservableProperty]
        private string? _statusString;

        [ObservableProperty]
        private string? _matNo;

        [ObservableProperty]
        private string? _station;
    }
}
