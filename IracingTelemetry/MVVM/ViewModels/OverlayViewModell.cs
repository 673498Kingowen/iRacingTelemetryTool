using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iRacingSDK;
using IracingTelemetry.Core;
using iRacingTelemetryTool.MVVM.Views;
namespace IracingTelemetry.MVVM.ViewModels;

public partial class OverlayViewModel : ObservableObject {
    public ObservableCollection<DriveInfo> DriverStandings { get; } = new();

    private OverlayWindow? _overlayWindow;
    private readonly RacingService _iRacingService;

    [ObservableProperty] private int _speed;

    [ObservableProperty] private int _gear;

    public OverlayViewModel(RacingService iRacingService) {
        _iRacingService = iRacingService ?? throw new ArgumentNullException(nameof(iRacingService));
        ShowOverlayCommand = new RelayCommand(ShowOverlay);
    }
    

    private void OnDataReceived(DataSample sample) {
        var telemetry = sample.Telemetry;
        Speed = (int)(sample.Telemetry.Speed * 3.6);
        Gear = sample.Telemetry.Gear;

        var sessionData = sample.SessionData;
        
        /* Todo
           -- Logic for updating DriverStandings List
         */
        Application.Current.Dispatcher.Invoke(() => {
            
        });
    }

    public IRelayCommand ShowOverlayCommand { get; }

    private void ShowOverlay() {

        try {
            if (_overlayWindow is { IsVisible: false }) {
                _overlayWindow = new OverlayWindow {
                    DataContext = this
                };
            }

            _overlayWindow?.Show();
        }
        catch (Exception e) {
            Debug.WriteLine($"Error showing overlay: {e.Message}");
        }
    }

    [RelayCommand]
    private void HideOverlay() {
        if (_overlayWindow is { IsVisible: true }) {
            _overlayWindow.Close();
        }
    }
}