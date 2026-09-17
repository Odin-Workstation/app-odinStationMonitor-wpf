using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Jendamark.Messaging;
using Jendamark.ODINWorkStationV2.Library.Payloads;
using Jendamark.ODINWorkStationV2.LocalInformationCache.Models;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using TacoStationMonitor.Config;
using TacoStationMonitor.Models;
using TacoStationMonitor.Service;

namespace TacoStationMonitor.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, IDisposable
    {
        private readonly IDashboardService _dashBoardDataservice;
        private bool _disposed = false;
        private readonly string _stationName = string.Empty;

        private readonly DispatcherTimer timer;
        private readonly Jendamark.Messaging.IMessenger _messenger;
        private readonly ILogger<DashboardViewModel> _logger;
        private readonly AppSettings _appSettings;
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

        public DashboardViewModel(AppSettings appSettings, Jendamark.Messaging.IMessenger messenger,IDashboardService dashboardService, ILogger<DashboardViewModel> logger)
        {
            _messenger = messenger;
            _dashBoardDataservice = dashboardService;
            _logger = logger;
            _appSettings = appSettings;

            BindMessages(); 
        }

        private void BindMessages()
        {
            _messenger.Subscribe(new SubscriptionTopic(this, "Station-StateMachine-PartValidInStation", PartValidated, TargetIDCreator.Station(_appSettings.StationId)));
            _messenger.Subscribe(new SubscriptionTopic(this, "Station-StateMachine-StationComplete", StationComplete, TargetIDCreator.Station(_appSettings.StationId)));
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

        /// <summary>
        /// Called when a part is validated in the station
        /// </summary>
        /// <param name="partID"></param>
        /// <returns></returns>
        private async Task BindPartAsync(int partID)
        {
            DashboardState data = await _dashBoardDataservice.GetDashboardDataAsync(partID);
            
            if (data == null)
                return;

            PartID = data.PartID;
            NextPartID = data.NextPartID;
            ChildPartNo = data.ChildPartNo;
            StatusString = data.StatusString;
            MatNo = data.MatNo;
            Station = data.StationName;
        }

        /// <summary>
        /// Called when the Part exists in the station and is completed, or the part is removed from the station
        /// </summary>
        /// <param name="subscriberID"></param>
        /// <param name="message"></param>

        private void StationComplete(int subscriberID, IMessage message)
        {
            var payloadValidator = new PayloadValidator<StationPayload>(message);
            try
            {
                var payload = payloadValidator.Validate();                
                int stationId = payload.StationID;

                if (stationId == _appSettings.StationId)
                {
                    DashboardState data = new DashboardState()
                    {
                        PartID = 0,
                        NextPartID = 0,
                        ChildPartNo = string.Empty,
                        StatusString = string.Empty,
                        MatNo = string.Empty,
                        StationName = _stationName
                    };
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating payload for StationComplete message.");
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _messenger.Unsubscribe(this);

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}