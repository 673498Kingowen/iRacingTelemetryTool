using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IracingTelemetry.Core;
using IracingTelemetry.MVVM.Models;
using iRacingSdkWrapper;
using iRacingSimulator.Drivers; 
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace IracingTelemetry.MVVM.ViewModels
{
    public partial class RelativeOverlayViewModel : ObservableObject
    {
        public ObservableCollection<RelativeDriverInfo> RelativeDrivers { get; } = new();
        
        private SessionData? _sessionData;
        private readonly IDeserializer _deserializer;

        public RelativeOverlayViewModel()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            RacingService.Instance.SessionInfoUpdated += OnSessionInfoUpdated;
            RacingService.Instance.TelemetryUpdated += OnTelemetryUpdated;
        }

        private void OnSessionInfoUpdated(SessionInfo sessionInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionInfo.RawYaml)) return;
                _sessionData = _deserializer.Deserialize<SessionData>(sessionInfo.RawYaml);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[YAML Deserialization Error] {ex.Message}");
                _sessionData = null;
            }
        }
        
        private Color GetStandardLicenseColor(int licLevel)
        {

            if (licLevel >= 0 && licLevel <= 4) return Color.FromRgb(221, 51, 51); 
            if (licLevel >= 5 && licLevel <= 8) return Color.FromRgb(255, 193, 5);  
            if (licLevel >= 9 && licLevel <= 12) return Color.FromRgb(0, 168, 224); 
            if (licLevel >= 13 && licLevel <= 16) return Color.FromRgb(51, 153, 51); 
            if (licLevel >= 17 && licLevel <= 20) return Color.FromRgb(28, 64, 140);  
            if (licLevel >= 21 && licLevel <= 28) return Colors.Black;
            return Colors.DarkGray;
        }

        private void OnTelemetryUpdated(TelemetryInfo telemetry)
        {
            if (_sessionData?.DriverInfo?.Drivers == null) return;

            var playerCarIdx = RacingService.Instance.PlayerCarIdx;

            var activeDrivers = Enumerable.Range(0, telemetry.CarIdxLapDistPct.Value.Length)
                .Where(i => telemetry.CarIdxTrackSurface.Value[i] != TrackSurfaces.NotInWorld)
                .Select(i => new
                {
                    CarIdx = i,
                    LapDistPct = telemetry.CarIdxLapDistPct.Value[i],
                    Lap = telemetry.CarIdxLap.Value[i]
                })
                .OrderByDescending(d => d.Lap + d.LapDistPct)
                .ToList();

            var playerIndex = activeDrivers.FindIndex(d => d.CarIdx == playerCarIdx);
            if (playerIndex == -1) return;

            var driversToShow = activeDrivers
                .Skip(Math.Max(0, playerIndex - 3))
                .Take(7)
                .ToList();

            Application.Current?.Dispatcher.Invoke((Action)(() =>
            {
                while (RelativeDrivers.Count > driversToShow.Count) RelativeDrivers.RemoveAt(RelativeDrivers.Count - 1);
                while (RelativeDrivers.Count < driversToShow.Count) RelativeDrivers.Add(new RelativeDriverInfo());

                for (int i = 0; i < driversToShow.Count; i++)
                {
                    var driverTelemetry = driversToShow[i];
                    var itemToUpdate = RelativeDrivers[i];
                    var carIdx = driverTelemetry.CarIdx;

                    var driverInfo = _sessionData.DriverInfo.Drivers.FirstOrDefault(d => d.CarIdx == carIdx);
                    if (driverInfo == null) continue;

                    var currentSessionNum = telemetry.SessionNum.Value;
                    ResultPosition? resultsInfo = null;
                    if (currentSessionNum < _sessionData.SessionInfo.Sessions.Count)
                    {
                        resultsInfo = _sessionData.SessionInfo.Sessions[currentSessionNum].ResultsPositions?.FirstOrDefault(p => p.CarIdx == carIdx);
                    }
                    
                    var licColor = GetStandardLicenseColor(driverInfo.LicLevel);
                    itemToUpdate.License = new License(driverInfo.LicLevel, driverInfo.LicSubLevel, licColor);
                    
                    itemToUpdate.CarIdx = carIdx;
                    itemToUpdate.Position = resultsInfo?.Position ?? 0;
                    itemToUpdate.Driver = driverInfo.UserName;
                    itemToUpdate.IsPlayer = carIdx == playerCarIdx;
                    
                    // Lap times
                    var lastTime = resultsInfo?.LastTime ?? -1;
                    var fastestTime = resultsInfo?.FastestTime ?? -1;
                    itemToUpdate.LastLapTime = FormatLapTime(lastTime);
                    itemToUpdate.FastestLapTime = FormatLapTime(fastestTime);
                    itemToUpdate.IsLastLapBest = (lastTime > 0 && fastestTime > 0 && Math.Abs(lastTime - fastestTime) < 0.001f);

                    if (itemToUpdate.IsPlayer)
                    {
                        itemToUpdate.DeltaTime = "0.0";
                    }
                    else
                    {
                        var playerTotalDistance = telemetry.CarIdxLap.Value[playerCarIdx] + telemetry.CarIdxLapDistPct.Value[playerCarIdx];
                        var driverTotalDistance = telemetry.CarIdxLap.Value[carIdx] + telemetry.CarIdxLapDistPct.Value[carIdx];
                        var distanceDelta = driverTotalDistance - playerTotalDistance;
                        var estimatedTimeDelta = distanceDelta * 60.0;
                        
                        if (Math.Abs(distanceDelta) >= 1.0)
                        {
                            itemToUpdate.DeltaTime = $"{(int)Math.Floor(Math.Abs(distanceDelta))}L";
                        }
                        else
                        {
                            itemToUpdate.DeltaTime = estimatedTimeDelta.ToString("F1", CultureInfo.InvariantCulture);
                        }
                    }
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