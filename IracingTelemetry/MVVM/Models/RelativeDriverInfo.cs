using CommunityToolkit.Mvvm.ComponentModel;
using iRacingSdkWrapper;
using iRacingSimulator.Drivers;

namespace IracingTelemetry.MVVM.Models
{
    public partial class RelativeDriverInfo : ObservableObject
    {
        [ObservableProperty] private int _carIdx;
        [ObservableProperty] private int _position;
        [ObservableProperty] private License? _license;
        [ObservableProperty] private string _driver = string.Empty; 
        [ObservableProperty] private string _deltaTime = string.Empty;
        [ObservableProperty] private string _lastLapTime = "-";
        [ObservableProperty] private string _fastestLapTime = "-";
        [ObservableProperty] private bool _isPlayer;
        [ObservableProperty] private bool _isLastLapBest;
    }
}