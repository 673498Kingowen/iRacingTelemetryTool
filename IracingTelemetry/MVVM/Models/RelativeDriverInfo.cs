namespace IracingTelemetry.MVVM.Models;

public class RelativeDriverInfo {
    public string Position { get; set; } = "";
    public string UserName { get; set; } = "";
    public string LicString { get; set; } = "";
    public string Gap { get; set; } = "";
    public bool IsCurrentDriver { get; set; }
}