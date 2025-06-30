using System;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IracingTelemetry.Core;
using IracingTelemetry.MVVM.Views;
using iRacingSdkWrapper;
using IracingTelemetry.MVVM.Models;

namespace IracingTelemetry.MVVM.ViewModels
{
    public partial class OverlayViewModel : ObservableObject
    {
        private readonly RacingService _racingService;
        private OverlayWindow? _overlayWindow;

        [ObservableProperty]
        private int _speed;

        [ObservableProperty]
        private int _gear;

        [ObservableProperty] 
        private string _position = string.Empty;
    
        [ObservableProperty]
        private string _userName = string.Empty;
    
        [ObservableProperty]
        private string _licString = string.Empty;
    
        [ObservableProperty]
        private int _incidents;
    
        [ObservableProperty]
        private string _lastLapTime = string.Empty;
    
        [ObservableProperty]
        private string _bestLapTime = string.Empty;

        public IRelayCommand ShowOverlayCommand { get; }
        public IRelayCommand HideOverlayCommand { get; }

        public OverlayViewModel(RacingService racingService)
        {
            _racingService = racingService;
            _racingService.TelemetryUpdated += UpdateTelemetry;
            _racingService.SessionInfoUpdated += UpdateSessionInfo;
            
            ShowOverlayCommand = new RelayCommand(ShowOverlay);
            HideOverlayCommand = new RelayCommand(HideOverlay);
        }

        private void UpdateTelemetry(TelemetryInfo? telemetry)
        {
            if (telemetry == null) return;

            try
            {
                // Access .Value property for TelemetryValue<T> types
                Speed = (int)(telemetry.Speed.Value * 3.6);
                Gear = telemetry.Gear.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating telemetry: {ex.Message}");
            }
        }

        private void UpdateSessionInfo(object? sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            if (e.SessionInfo == null) return;

            try
            {
                // Access session info using the YAML query system
                var driverIdx = _racingService.IsConnected ? _racingService.GetDriverId() : 0;
        
                // Get driver information using the YAML path
                Position = e.SessionInfo.TryGetValue($"DriverInfo:Drivers:CarIdx:{driverIdx}:Position");
                UserName = e.SessionInfo.TryGetValue($"DriverInfo:Drivers:CarIdx:{driverIdx}:UserName");
                LicString = e.SessionInfo.TryGetValue($"DriverInfo:Drivers:CarIdx:{driverIdx}:LicString");
        
                var incidentsStr = e.SessionInfo.TryGetValue($"DriverInfo:Drivers:CarIdx:{driverIdx}:CurDriverIncidentCount");
                if (int.TryParse(incidentsStr, out int incidentsVal))
                {
                    Incidents = incidentsVal;
                }

                // Get lap time information
                var lastLapStr = e.SessionInfo.TryGetValue($"DriverInfo:Drivers:CarIdx:{driverIdx}:LastLapTime");
                if (!string.IsNullOrEmpty(lastLapStr) && lastLapStr != "0")
                {
                    LastLapTime = FormatLapTime(lastLapStr);
                }

                var bestLapStr = e.SessionInfo.TryGetValue($"DriverInfo:Drivers:CarIdx:{driverIdx}:BestLapTime");
                if (!string.IsNullOrEmpty(bestLapStr) && bestLapStr != "0")
                {
                    BestLapTime = FormatLapTime(bestLapStr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating session info: {ex.Message}");
            }
        }

        private string FormatLapTime(string lapTimeStr)
        {
            if (double.TryParse(lapTimeStr, out double lapTime))
            {
                TimeSpan ts = TimeSpan.FromSeconds(lapTime);
                return ts.Minutes > 0 
                    ? $"{ts.Minutes}:{ts.Seconds:D2}.{ts.Milliseconds:D3}" 
                    : $"{ts.Seconds}.{ts.Milliseconds:D3}";
            }
            return lapTimeStr;
        }

        private void ShowOverlay()
        {
            if (_overlayWindow == null)
            {
                _overlayWindow = new OverlayWindow
                {
                    DataContext = this
                };
            }

            _overlayWindow.Show();
            
            _overlayWindow.DataContext = new RelativeOverlayViewModel(); // Set window DataContext
            _overlayWindow.Show();
            _overlayWindow.Loaded += (sender, e) => {
                if (_overlayWindow.FindName("myView") is FrameworkElement myView)
                    myView.DataContext = new RelativeOverlayViewModel();
            };
        }

        private void HideOverlay()
        {
            _overlayWindow?.Hide();
        }
        
        // Add to OverlayViewModel.cs
        public ObservableCollection<RelativeDriverInfo> RelativeDrivers { get; } = new();
    }
}