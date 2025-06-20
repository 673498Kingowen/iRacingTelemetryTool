using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using iRacingSdkWrapper;
using iRacingTelemetryTool.Core;
using iRacingTelemetryTool.MVVM.Views;
namespace iRacingTelemetryTool.MVVM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using iRacingSDK;
using System.Collections.ObjectModel;

public partial class OverlayViewModel : ObservableObject {
    public ObservableCollection<DriveInfo> DriverStandings { get; } = new();

    private OverlayWindow _overlayWindow;
    private readonly iRacingService _iRacingService;

    [ObservableProperty] private int _speed;

    [ObservableProperty] private int _gear;

    public OverlayViewModel(iRacingService iRacingService) {
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
    
    public void ShowOverlay() {

        try {
            if (_overlayWindow == null || !_overlayWindow.IsVisible) {
                _overlayWindow = new OverlayWindow();
                _overlayWindow.DataContext = this;
            }

            _overlayWindow.Show();
        }
        catch (Exception e) {
            Debug.WriteLine($"Error showing overlay: {e.Message}");
        }
    }

    [RelayCommand]
    private void HideOverlay() {
        if (_overlayWindow.IsVisible) {
            _overlayWindow.Close();
        }
    }
}