using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using iRacingSdkWrapper;

namespace IracingTelemetry.Core
{
    public class RacingService : INotifyPropertyChanged
    {
        // Static instance for Singleton pattern
        private static readonly Lazy<RacingService> _instance = new(() => new RacingService());
        public static RacingService Instance => _instance.Value;

        private readonly SdkWrapper _wrapper;
        private bool _isConnected;

        // --- Public events for ViewModels to subscribe to ---
        public event Action<TelemetryInfo> TelemetryUpdated = delegate { };
        public event Action<SessionInfo> SessionInfoUpdated = delegate { };
        
        // -- ADD THIS EVENT BACK IN --
        public event Action<string> ConnectionStatusChanged = delegate { };

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                    // Also fire the status changed event for logging/display
                    ConnectionStatusChanged?.Invoke(value ? "Connected" : "Disconnected");
                }
            }
        }
        
        public int PlayerCarIdx => _wrapper.DriverId;

        private RacingService()
        {
            _wrapper = new SdkWrapper();
            _wrapper.TelemetryUpdateFrequency = 60;

            _wrapper.Connected += (s, e) => { IsConnected = true; };
            _wrapper.Disconnected += (s, e) => { IsConnected = false; };
            
            _wrapper.TelemetryUpdated += (s, e) =>
            {
                TelemetryUpdated?.Invoke(e.TelemetryInfo);
            };

            _wrapper.SessionInfoUpdated += (s, e) =>
            {
                SessionInfoUpdated?.Invoke(e.SessionInfo);
            };
        }

        public void Start()
        {
            if (IsIRacingRunning())
            {
                if (!_wrapper.IsRunning)
                {
                    Log("iRacing detected. Starting connection...");
                    _wrapper.Start();
                }
            }
            else
            {
                Log("iRacing is not running.");
            }
        }

        public void Stop()
        {
            if (_wrapper.IsRunning)
            {
                _wrapper.Stop();
                Log("Service stopped.");
            }
        }
        
        // Helper method to log messages and fire the event
        private void Log(string message) {
            Console.WriteLine(message);
            ConnectionStatusChanged?.Invoke(message);
        }

        private bool IsIRacingRunning()
        {
            return Process.GetProcessesByName("iRacingSim64DX11").Length > 0;
        }
        

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}