namespace iRacingTelemetryTool.MVVM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using iRacingTelemetryTool.Core;

public partial class MainViewModel : ObservableObject{
    
    /// <summary>
    /// Overlay tabs.
    /// </summary>
    public IracingTelemetry.MVVM.ViewModels.OverlayViewModel OverlayVM { get; }
    
    public MainViewModel(iRacingService iRacingService) {
        OverlayVM = new IracingTelemetry.MVVM.ViewModels.OverlayViewModel(iRacingService);
    }
}