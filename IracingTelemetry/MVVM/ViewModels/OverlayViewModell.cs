using System;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IracingTelemetry.Core;
using IracingTelemetry.MVVM.Views;
using iRacingSdkWrapper;
using IracingTelemetry.MVVM.Models;
using System.Globalization;

namespace IracingTelemetry.MVVM.ViewModels
{
    public partial class OverlayViewModel : ObservableObject
    {
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
        
        public IRelayCommand ShowSpeedOverlayCommand { get; }
        public IRelayCommand HideSpeedOverlayCommand { get; }
        public IRelayCommand ShowRelativeOverlayCommand { get; }
        public IRelayCommand HideRelativeOverlayCommand { get; }
        
        public OverlayViewModel()
        {
            RacingService.Instance.TelemetryUpdated += UpdateTelemetry;
            RacingService.Instance.SessionInfoUpdated += OnSessionInfoUpdated;
            
            ShowOverlayCommand = new RelayCommand(ShowOverlay);
            HideOverlayCommand = new RelayCommand(HideOverlay);
            
            ShowOverlayCommand = new RelayCommand(ShowOverlay);
            HideOverlayCommand = new RelayCommand(HideOverlay);
            ShowSpeedOverlayCommand = new RelayCommand(ShowSpeedOverlay);
            HideSpeedOverlayCommand = new RelayCommand(HideSpeedOverlay);
            ShowRelativeOverlayCommand = new RelayCommand(ShowRelativeOverlay);
            HideRelativeOverlayCommand = new RelayCommand(HideRelativeOverlay);

            // Start the service
            RacingService.Instance.Start();
        }

        private void UpdateTelemetry(TelemetryInfo telemetry)
        {
            if (telemetry == null) return;

            try
            {
                Speed = (int)(telemetry.Speed.Value * 3.6);
                Gear = telemetry.Gear.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating telemetry: {ex.Message}");
            }
        }
        
        private void OnSessionInfoUpdated(SessionInfo sessionInfo)
        {
            if (sessionInfo == null) return;

            try
            {
                var driverIdx = RacingService.Instance.PlayerCarIdx;
        
                Position = sessionInfo["DriverInfo"]["Drivers"]["CarIdx", driverIdx]["Position"].GetValue();
                UserName = sessionInfo["DriverInfo"]["Drivers"]["CarIdx", driverIdx]["UserName"].GetValue();
                LicString = sessionInfo["DriverInfo"]["Drivers"]["CarIdx", driverIdx]["LicString"].GetValue();
        
                var incidentsStr = sessionInfo["SessionInfo"]["Sessions"]["ResultsPositions"]["CarIdx", driverIdx]["Incidents"].GetValue();
                if (int.TryParse(incidentsStr, out int incidentsVal))
                {
                    Incidents = incidentsVal;
                }

                var lastLapStr = sessionInfo["SessionInfo"]["Sessions"]["ResultsPositions"]["CarIdx", driverIdx]["LastTime"].GetValue();
                if (!string.IsNullOrEmpty(lastLapStr) && lastLapStr != "-1.000")
                {
                    LastLapTime = FormatLapTime(lastLapStr);
                }

                var bestLapStr = sessionInfo["SessionInfo"]["Sessions"]["ResultsPositions"]["CarIdx", driverIdx]["FastestTime"].GetValue();
                if (!string.IsNullOrEmpty(bestLapStr) && bestLapStr != "-1.000")
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
            if (double.TryParse(lapTimeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double lapTime))
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
                    DataContext = new RelativeOverlayViewModel()
                };
            }

            _overlayWindow.Show();
        }

        private void HideOverlay()
        {
            _overlayWindow?.Hide();
        }
        
        private OverlayWindow? _speedOverlayWindow;
        private OverlayWindow? _relativeOverlayWindow;

        private void ShowSpeedOverlay()
        {
            if (_speedOverlayWindow == null)
            {
                _speedOverlayWindow = new OverlayWindow();
            }
            _speedOverlayWindow.Show();
        }

        private void HideSpeedOverlay()
        {
            _speedOverlayWindow?.Hide();
        }

        private void ShowRelativeOverlay()
        {
            if (_relativeOverlayWindow == null)
            {
                _relativeOverlayWindow = new OverlayWindow
                {
                    Content = new RelativOverlay
                    {
                        DataContext = new RelativeOverlayViewModel()
                    }
                };
            }
            _relativeOverlayWindow.Show();
        }

        private void HideRelativeOverlay()
        {
            _relativeOverlayWindow?.Hide();
        }
        
        public ObservableCollection<RelativeDriverInfo> RelativeDrivers { get; } = new();
    }
}