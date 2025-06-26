using CommunityToolkit.Mvvm.ComponentModel;
using IracingTelemetry.Core;
namespace IracingTelemetry.MVVM.ViewModels;

public partial class MainViewModel(RacingService iRacingService) : ObservableObject {
    
    /// <summary>
    /// Overlay tabs.
    /// </summary>
    public IracingTelemetry.MVVM.ViewModels.OverlayViewModel OverlayVm { get; } = new(iRacingService) {
        Speed = 0,
        Gear = 0
    };

}