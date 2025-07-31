using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IracingTelemetry.Core;
using IracingTelemetry.MVVM.Models;
using iRacingSdkWrapper;
using iRacingSimulator.Drivers;

namespace IracingTelemetry.MVVM.ViewModels
{
    public partial class RelativeOverlayViewModel : ObservableObject
    {
        public ObservableCollection<RelativeDriverInfo> RelativeDrivers { get; } = new();
        private SessionInfo? _latestSessionInfo;

        public RelativeOverlayViewModel()
        {
            RacingService.Instance.SessionInfoUpdated += OnSessionInfoUpdated;
            RacingService.Instance.TelemetryUpdated += OnTelemetryUpdated;
        }

        private void OnSessionInfoUpdated(SessionInfo sessionInfo)
        {
            _latestSessionInfo = sessionInfo;
        }

        private void OnTelemetryUpdated(TelemetryInfo telemetry)
        {
            if (_latestSessionInfo == null) return;

            var playerCarIdx = RacingService.Instance.PlayerCarIdx;

            var allDrivers = Enumerable.Range(0, telemetry.CarIdxLapDistPct.Value.Length)
                .Where(i => telemetry.CarIdxTrackSurface.Value[i] != TrackSurfaces.NotInWorld)
                .Select(i => new
                {
                    CarIdx = i,
                    LapDistPct = telemetry.CarIdxLapDistPct.Value[i]
                })
                .OrderByDescending(d => d.LapDistPct)
                .ToList();

            var playerIndex = allDrivers.FindIndex(d => d.CarIdx == playerCarIdx);
            if (playerIndex == -1) return;

            var driversToShow = allDrivers
                .Skip(Math.Max(0, playerIndex - 3))
                .Take(7)
                .ToList();

            Application.Current?.Dispatcher.Invoke(new Action(() =>
            {
                if (_latestSessionInfo == null) return;

                while (RelativeDrivers.Count < driversToShow.Count)
                {
                    RelativeDrivers.Add(new RelativeDriverInfo());
                }
                while (RelativeDrivers.Count > driversToShow.Count)
                {
                    RelativeDrivers.RemoveAt(RelativeDrivers.Count - 1);
                }

                for (int i = 0; i < driversToShow.Count; i++)
                {
                    var driverData = driversToShow[i];
                    var itemToUpdate = RelativeDrivers[i];
                    var carIdx = driverData.CarIdx;

                    var driverQuery = _latestSessionInfo["DriverInfo"]["Drivers"]["CarIdx", carIdx];
                    
                    var currentSessionNum = telemetry.SessionNum.Value;
                    var resultsQuery = _latestSessionInfo["SessionInfo"]["Sessions"]["SessionNum", currentSessionNum]["ResultsPositions"]["CarIdx", carIdx];

                    itemToUpdate.CarIdx = carIdx;
                    itemToUpdate.Position = int.TryParse(resultsQuery["Position"].GetValue(), out var pos) ? pos : 0;
                    itemToUpdate.Driver = driverQuery["UserName"].GetValue() ?? "N/A";

                    int.TryParse(driverQuery["LicLevel"].GetValue(), out var licLevel);
                    int.TryParse(driverQuery["LicSubLevel"].GetValue(), out var licSubLevel);
                    
                    var licColorString = driverQuery["LicColor"].GetValue() ?? "0xFFFFFF";
                    var color = (Color)(ColorConverter.ConvertFromString(licColorString) ?? Colors.White);
                    itemToUpdate.License = new License(licLevel, licSubLevel, color);

                    float.TryParse(resultsQuery["LastTime"].GetValue(), out var lastTime);
                    float.TryParse(resultsQuery["FastestTime"].GetValue(), out var fastestTime);
                    
                    itemToUpdate.LastLapTime = FormatLapTime(lastTime > 0 ? lastTime : -1);
                    itemToUpdate.FastestLapTime = FormatLapTime(fastestTime > 0 ? fastestTime : -1);
                    
                    itemToUpdate.IsLastLapBest = (lastTime > 0 && lastTime == fastestTime);

                    itemToUpdate.IsPlayer = carIdx == playerCarIdx;
                    
                    var playerTime = telemetry.CarIdxEstTime.Value[playerCarIdx];
                    var driverTime = telemetry.CarIdxEstTime.Value[carIdx];
                    var delta = driverTime - playerTime;
                    itemToUpdate.DeltaTime = itemToUpdate.IsPlayer ? "0.0" : delta.ToString("F1", CultureInfo.InvariantCulture);
                }
            }));
        }

        private string FormatLapTime(float lapTime)
        {
            if (lapTime <= 0) return "-";
            return TimeSpan.FromSeconds(lapTime).ToString(@"m\:ss\.fff");
        }
    }
}