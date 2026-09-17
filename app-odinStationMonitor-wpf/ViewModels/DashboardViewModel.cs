using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Jendamark.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using TacoStationMonitor.Models;
using TacoStationMonitor.Service;

namespace TacoStationMonitor.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DashboardService service = new();

        private readonly DispatcherTimer timer;
        private readonly Jendamark.Messaging.IMessenger _messenger;
        private readonly ILogger<DashboardViewModel> _logger;

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

        [ObservableProperty]
        private string? _currentTime;

        public DashboardViewModel(Jendamark.Messaging.IMessenger messenger, ILogger<DashboardViewModel> logger)
        {
            _messenger = messenger;
            _logger = logger;

            BindMessages();

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void BindMessages()
        {
            _messenger.Subscribe(new SubscriptionTopic(this, "Station-StateMachine-PartValidInStation", PartValidated, TargetIDCreator.Station(_stationID)));
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

        private void PartValidated(int subscriberID, IMessage message)
        {
            var payloadValidator = new PayloadValidator<PartValidationPayload>(message);
            try
            {
                var payload = payloadValidator.Validate();
                _ = BindPartAsync(payload.PartValidationOutput.PartIDOut ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating payload for PartValidated message.");
            }
        }

       
        
    }
}