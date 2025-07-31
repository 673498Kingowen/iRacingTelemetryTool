using IracingTelemetry.Core;
using IracingTelemetry.MVVM.ViewModels;

namespace IracingTelemetry.MVVM.Views
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow() {
            DataContext = new MainViewModel();
            InitializeComponent();
        }
    }
}