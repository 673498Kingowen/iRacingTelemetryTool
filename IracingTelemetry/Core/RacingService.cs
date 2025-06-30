using System;
using System.ComponentModel;
using System.Diagnostics;
using iRacingSdkWrapper;
using System.Globalization;

namespace IracingTelemetry.Core
{
    public class RacingService : INotifyPropertyChanged
    {
        private readonly SdkWrapper _wrapper = new();
        private bool _isConnected;

        public event Action<TelemetryInfo> TelemetryUpdated = null!;
        public event Action<string> ConnectionStatusChanged = null!;

        // Define a single session info event with delegate
        public delegate void SessionInfoEventHandler(object? sender, SdkWrapper.SessionInfoUpdatedEventArgs e);
        public event SessionInfoEventHandler? SessionInfoUpdated;

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    ConnectionStatusChanged?.Invoke(value ? "Connected" : "Disconnected");
                }
            }
        }

        public RacingService()
        {
            // 1. Instantiate the wrapper.
            _wrapper.TelemetryUpdateFrequency = 60;

            // 2. Subscribe to the events.
            _wrapper.Connected += OnConnected;
            _wrapper.Disconnected += OnDisconnected;
            _wrapper.TelemetryUpdated += OnTelemetryUpdated;
            _wrapper.SessionInfoUpdated += OnSessionInfoUpdated;
        }

        // 3. Start the connection.
        public void Start()
        {
            if (!IsIRacingRunning())
            {
                Log($"iRacing is not running. Please start iRacing.");
                return;
            }

            Log($"iRacing detected. Starting connection...");
            if (!_wrapper.IsRunning)
            {
                _wrapper.Start();
            }
        }

        public void Stop()
        {
            if (_wrapper.IsRunning)
            {
                _wrapper.Stop();
            }
            Log($"Service stopped.");
        }

        // 4. Handle the "Connected" event.
        private void OnConnected(object? sender, EventArgs e)
        {
            IsConnected = true;
            Log($"Successfully connected to iRacing! Waiting for data...");
        }

        // 5. Handle the "Disconnected" event.
        private void OnDisconnected(object? sender, EventArgs e)
        {
            IsConnected = false;
            Log($"Disconnected from iRacing.");
        }

        // 6. Handle new telemetry data when it arrives.
        private void OnTelemetryUpdated(object? sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            // The e.TelemetryInfo object contains all the live data.
            DisplayTelemetry(e.TelemetryInfo);
            TelemetryUpdated?.Invoke(e.TelemetryInfo);
        }

        // Forward session info updated events
        private void OnSessionInfoUpdated(object? sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            // Forward the event for session info (driver details, etc.)
            SessionInfoUpdated?.Invoke(this, e);
        }

        // Helper method to display telemetry data.
        private void DisplayTelemetry(TelemetryInfo telemetry) {
            try {
                var sessionTime = telemetry.SessionTime;
                var speed = telemetry.Speed;
                var rpm = telemetry.RPM;
                var gear = telemetry.Gear;
                var fuel = telemetry.FuelLevel;

                // This logging is for debugging and can be removed in production
                // Console.WriteLine($"\n--- TELEMETRY DATA ---");
                // Console.WriteLine($"Session time: {sessionTime:F1}s");
                // Console.WriteLine($"Speed: {speed.Value * 3.6:F1} km/h");
                // Console.WriteLine($"RPM: {rpm:F0}");
                // Console.WriteLine($"Gear: {gear}");
                // Console.WriteLine($"Fuel: {fuel:F1}L");
                // Console.WriteLine($"------------------------------");
            }
            catch (Exception ex) {
                Log($"Error displaying telemetry: {ex.Message}");
            }
        }

        // Helper method to log messages.
        private void Log(FormattableString message) {
            string formattedMessage = message.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine(formattedMessage);
            ConnectionStatusChanged?.Invoke(formattedMessage);
        }

        // Helper method to check if iRacing is running.
        private bool IsIRacingRunning() {
            return Process.GetProcessesByName("iRacingSim64DX11").Length > 0;
        }

        // Property changed implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public int GetDriverId() {
            return _wrapper.DriverId;
        }
    }
}