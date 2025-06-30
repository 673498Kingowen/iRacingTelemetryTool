using System;

namespace IracingTelemetry.MVVM.Models
{
    /// <summary>
    /// Represents an iRacing license with its properties and behaviors
    /// </summary>
    public class License
    {
        // License levels in iRacing
        private enum LicenseLevel
        {
            Rookie,
            D,
            C,
            B,
            A,
            Pro,
            ProWC
        }

        // License categories in iRacing
        private enum LicenseCategory
        {
            Oval,
            Road,
            DirtOval,
            DirtRoad
        }

        // Properties
        private LicenseLevel Level { get; set; }
        private LicenseCategory Category { get; set; }
        private float SafetyRating { get; set; }
        private int IRating { get; set; }
        private float SubLevel { get; set; } 
        public DateTime LastUpdated { get; set; }
        private bool MprCompleted { get; set; } 

        // Constructor
        public License()
        {
            Level = LicenseLevel.Rookie;
            Category = LicenseCategory.Road;
            SafetyRating = 2.50f;
            IRating = 1350;
            SubLevel = 2.5f;
            LastUpdated = DateTime.Now;
            MprCompleted = false;
        }

        // Methods
        /// <summary>
        /// Gets the full license string representation
        /// </summary>
        public string GetLicenseString()
        {
            return $"{Level} {SubLevel:F2}";
        }

        /// <summary>
        /// Checks if the driver is eligible for promotion
        /// </summary>
        public bool IsEligibleForPromotion()
        {
            return SafetyRating >= 3.0f && MprCompleted && Level != LicenseLevel.ProWC;
        }

        /// <summary>
        /// Updates the safety rating with a new value
        /// </summary>
        public void UpdateSafetyRating(float newSafetyRating)
        {
            SafetyRating = Math.Max(0.0f, Math.Min(newSafetyRating, 4.99f));
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Returns license color based on level
        /// </summary>
        public string GetLicenseColor()
        {
            switch (Level)
            {
                case LicenseLevel.Rookie: return "#FF0000"; // Red
                case LicenseLevel.D: return "#FFA500"; // Orange
                case LicenseLevel.C: return "#FFFF00"; // Yellow
                case LicenseLevel.B: return "#008000"; // Green
                case LicenseLevel.A: return "#0000FF"; // Blue
                case LicenseLevel.Pro: return "#800080"; // Purple
                case LicenseLevel.ProWC: return "#000000"; // Black
                default: return "#FFFFFF";
            }
        }

        /// <summary>
        /// Override ToString to display license information
        /// </summary>
        public override string ToString()
        {
            return $"{Category} {Level} {SubLevel:F2} SR:{SafetyRating:F2} iR:{IRating}";
        }
    }
}