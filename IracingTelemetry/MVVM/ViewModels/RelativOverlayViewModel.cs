using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using IracingTelemetry.Core;
using iRacingSdkWrapper;

namespace IracingTelemetry.MVVM.ViewModels
{
    public class RelativeOverlayViewModel : INotifyPropertyChanged
    {
        private readonly RacingService _racingService;
        private SessionInfo _sessionInfo;

        public ObservableCollection<RelativeDriverViewModel> RelativeDrivers { get; }

        public RelativeOverlayViewModel(RacingService racingService)
        {
            _racingService = racingService;
            RelativeDrivers = new ObservableCollection<RelativeDriverViewModel>();

            _racingService.SessionInfoUpdated += OnSessionInfoUpdated;
            _racingService.TelemetryUpdated += OnTelemetryUpdated;
        }

        private void OnSessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            _sessionInfo = e.SessionInfo;
        }

        private void OnTelemetryUpdated(TelemetryInfo telemetry)
        {
            if (_sessionInfo == null) return;
            
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateRelativeDrivers(telemetry);
            });
        }

        private void UpdateRelativeDrivers(TelemetryInfo telemetry)
        {
            var player = _sessionInfo.DriverInfo.Drivers.FirstOrDefault(d => d.CarIdx == telemetry.PlayerCarIdx);
            if (player == null) return;

            var driversOnTrack = _sessionInfo.DriverInfo.Drivers
                .Where(d => d.IsOnTrack)
                .OrderBy(d => d.Live.LapDist)
                .ToList();

            var playerIndex = driversOnTrack.FindIndex(d => d.CarIdx == player.CarIdx);
            if (playerIndex == -1) return;

            RelativeDrivers.Clear();
            
            const int carsAhead = 5;
            const int carsBehind = 5;

            int startIndex = Math.Max(0, playerIndex - carsBehind);
            int endIndex = Math.Min(driversOnTrack.Count - 1, playerIndex + carsAhead);

            for (int i = startIndex; i <= endIndex; i++)
            {
                var driver = driversOnTrack[i];
                var driverTelemetry = telemetry.Cars[driver.CarIdx];
                
                var delta = driver.Live.LapDist - player.Live.LapDist;
                
                RelativeDrivers.Add(new RelativeDriverViewModel(driver, delta));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class RelativeDriverViewModel
    {
        private readonly iRacingSdkWrapper.Drivers.Driver _driver;

        public RelativeDriverViewModel(iRacingSdkWrapper.Drivers.Driver driver, float deltaTime)
        {
            _driver = driver;
            DeltaTime = deltaTime;
        }

        public int Position => _driver.Position;
        public string DriverName => _driver.UserName;
        public string License => _driver.LicString;
        public float DeltaTime { get; }
        
        public TimeSpan LastLapTime => TimeSpan.FromSeconds(_driver.LastLapTime);
        public TimeSpan FastestLapTime => TimeSpan.FromSeconds(_driver.BestLapTime);

 
        public bool IsLastLapBest => _driver.LastLapTime > 0 && _driver.LastLapTime == _driver.BestLapTime;
    }
}
