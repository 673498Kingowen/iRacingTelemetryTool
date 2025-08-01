using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace IracingTelemetry.MVVM.Models
{
    // A class to hold the driver's info from the session YAML
    public class SessionDriver
    {
        public int CarIdx { get; set; }
        public string UserName { get; set; } = "N/A";
        public int LicLevel { get; set; }
        public int LicSubLevel { get; set; }
        public string LicColor { get; set; } = "0xFFFFFF";
    }

    // A class to hold the results for a specific driver
    public class ResultPosition
    {
        public int Position { get; set; }
        public int CarIdx { get; set; }
        public float LastTime { get; set; }
        public float FastestTime { get; set; }
    }
    
    // A class representing a single session (Practice, Qualify, Race)
    public class Session
    {
        public List<ResultPosition> ResultsPositions { get; set; } = new();
    }
    
    public class DriverInfo
    {
        public List<SessionDriver> Drivers { get; set; } = new();
    }

    public class SessionInfoNode
    {
        public List<Session> Sessions { get; set; } = new();
    }
    
    public class SessionData
    {
        public DriverInfo DriverInfo { get; set; } = new();
        public SessionInfoNode SessionInfo { get; set; } = new();
    }
}