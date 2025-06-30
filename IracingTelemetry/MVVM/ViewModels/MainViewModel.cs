using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IracingTelemetry.Core;

namespace IracingTelemetry.MVVM.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly RacingService _racingService;

        [ObservableProperty]
        private string _connectionStatus = "Disconnected";

        public IRelayCommand StartConnectionCommand { get; }

        /// <summary>
        /// Overlay tab viewmodel.
        /// </summary>
        public OverlayViewModel OverlayVm { get; }

        public MainViewModel(RacingService racingService)
        {
            _racingService = racingService;
            
            // Initialize the overlay viewmodel with the racing service
            OverlayVm = new(racingService) {
                Speed = 0,
                Gear = 0
            };
            
            // Set up commands
            StartConnectionCommand = new RelayCommand(StartConnection);
            
            // Subscribe to connection status changes
            _racingService.ConnectionStatusChanged += UpdateConnectionStatus;
        }
        
        private void StartConnection()
        {
            _racingService.Start();
        }
        
        private void UpdateConnectionStatus(string status)
        {
            ConnectionStatus = status;
        }
    }
}