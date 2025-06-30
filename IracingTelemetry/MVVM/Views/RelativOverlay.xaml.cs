using System.ComponentModel;
using System.Runtime.CompilerServices;

public class RelativeOverlayViewModel : INotifyPropertyChanged
{
    private string _position;
    
    public string Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPropertyChanged();
            }
        }
    }
    
    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
}