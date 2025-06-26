using System.Diagnostics;
using iRacingSdkWrapper;
namespace IracingTelemetry.Core
{
    public class RacingService
    {
        private readonly SdkWrapper _wrapper = new();

        public event Action<TelemetryInfo> TelemetryUpdated = null!;
        public event Action<string> ConnectionStatusChanged = null!;

        public bool IsConnected => _wrapper.IsConnected;

        public RacingService()
        {
            // 1. Instantiate the wrapper.
            _wrapper.TelemetryUpdateFrequency = 60;

            // 2. Subscribe to the events.
            _wrapper.Connected += OnConnected;
            _wrapper.Disconnected += OnDisconnected;
            _wrapper.TelemetryUpdated += OnTelemetryUpdated;
        }

        // 3. Start the connection.
        public void Start()
        {
            if (!IsIRacingRunning())
            {
                Log("iRacing is not running. Please start iRacing.");
                return;
            }

            Log("iRacing detected. Starting connection...");
            _wrapper.Start();
        }

        public void Stop()
        {
            _wrapper.Stop();
            Log("Service stopped.");
        }

        // 4. Handle the "Connected" event.
        private void OnConnected(object? sender, EventArgs e)
        {
            Log("Successfully connected to iRacing! Waiting for data...");
        }

        // 5. Handle the "Disconnected" event.
        private void OnDisconnected(object? sender, EventArgs e)
        {
            Log("Disconnected from iRacing.");
        }

        // 6. Handle new telemetry data when it arrives.
        private void OnTelemetryUpdated(object? sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            // The e.TelemetryInfo object contains all the live data.
            DisplayTelemetry(e.TelemetryInfo);
            TelemetryUpdated(e.TelemetryInfo);
        }
        
        // Helper method to display telemetry data.
        private void DisplayTelemetry(TelemetryInfo telemetry)
        {
            try
            {
                var sessionTime = telemetry.SessionTime;
                var speed = telemetry.Speed;
                var rpm = telemetry.RPM;
                var gear = telemetry.Gear;
                var fuel = telemetry.FuelLevel;

                Log($"\n--- TELEMETRY DATA ---");
                Log($"Session time: {sessionTime:F1}s");
                Log($"Speed: {speed.Value * 3.6:F1} km/h");
                Log($"RPM: {rpm:F0}");
                Log($"Gear: {gear}");
                Log($"Fuel: {fuel:F1}L");
                Log("------------------------------");
            }
            catch (Exception ex)
            {
                Log($"Error displaying telemetry: {ex.Message}");
            }
        }
        
        // Helper method to log messages.
        private void Log(string message)
        {
            Console.WriteLine(message);
            ConnectionStatusChanged(message);
        }
        
        // Helper method to check if iRacing is running.
        private bool IsIRacingRunning()
        {
            return Process.GetProcessesByName("iRacingSim64DX11").Length > 0;
        }
    }
}