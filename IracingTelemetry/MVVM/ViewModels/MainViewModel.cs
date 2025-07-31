using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IracingTelemetry.Core;
using System.Windows; // Required for Dispatcher

namespace IracingTelemetry.MVVM.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _connectionStatus = "Disconnected";

        public IRelayCommand StartConnectionCommand { get; }

        /// <summary>
        /// Overlay tab viewmodel.
        /// </summary>
        public OverlayViewModel OverlayVm { get; }

        /// <summary>
        /// Relative Overlay tab viewmodel.
        /// </summary>
        public RelativeOverlayViewModel RelativeVm { get; }
        
        public MainViewModel()
        {
            OverlayVm = new OverlayViewModel();
            RelativeVm = new RelativeOverlayViewModel();
            StartConnectionCommand = new RelayCommand(() => RacingService.Instance.Start());
            RacingService.Instance.ConnectionStatusChanged += UpdateConnectionStatus;
            RacingService.Instance.Start();
        }
        
        private void UpdateConnectionStatus(string status) {
            Application.Current?.Dispatcher.Invoke(() => {
                ConnectionStatus = status;
            });
        }
    }
}