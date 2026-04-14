// ============================================================
// Models/Criminal.cs
// Inherits from Person. Adds crime history, status, etc.
// Demonstrates: Inheritance, Encapsulation, Polymorphism
// ============================================================
namespace CriminalRecordMS.Models
{
    public class Criminal : Person
    {
        private List<CrimeRecord> _crimeHistory;
        private bool _isMostWanted;
        private string _dangerLevel;

        public List<CrimeRecord> CrimeHistory
        {
            get => _crimeHistory;
            private set => _crimeHistory = value ?? new List<CrimeRecord>();
        }

        public bool IsMostWanted
        {
            get => _isMostWanted;
            set => _isMostWanted = value;
        }

        public string DangerLevel
        {
            get => _dangerLevel;
            set => _dangerLevel = value ?? "Low";
        }

        public string Nationality { get; set; }
        public string PhysicalDescription { get; set; }
        public bool IsArrested { get; set; }
        public DateTime? ArrestDate { get; set; }

        public Criminal(string id, string name, string cnic, string address,
                        string phone, DateTime dob, string nationality,
                        string physicalDescription)
            : base(id, name, cnic, address, phone, dob)
        {
            Nationality = nationality;
            PhysicalDescription = physicalDescription;
            _crimeHistory = new List<CrimeRecord>();
            _isMostWanted = false;
            _dangerLevel = "Low";
            IsArrested = false;
        }

        public void AddCrime(CrimeRecord crime)
        {
            if (crime == null) throw new ArgumentNullException(nameof(crime));
            _crimeHistory.Add(crime);
            UpdateDangerLevel();
            UpdateMostWantedStatus();
        }

        public void RemoveCrime(string crimeId)
        {
            var crime = _crimeHistory.FirstOrDefault(c => c.CrimeId == crimeId);
            if (crime != null)
            {
                _crimeHistory.Remove(crime);
                UpdateDangerLevel();
                UpdateMostWantedStatus();
            }
        }

        private void UpdateDangerLevel()
        {
            int capitalCount = _crimeHistory.Count(c => c.Severity == CrimeSeverity.Capital);
            int majorCount = _crimeHistory.Count(c => c.Severity == CrimeSeverity.Major);

            if (capitalCount >= 2 || _crimeHistory.Count >= 5)
                _dangerLevel = "Extreme";
            else if (capitalCount == 1 || majorCount >= 2)
                _dangerLevel = "High";
            else if (majorCount == 1 || _crimeHistory.Count >= 3)
                _dangerLevel = "Medium";
            else
                _dangerLevel = "Low";
        }

        private void UpdateMostWantedStatus()
        {
            _isMostWanted = _crimeHistory.Count >= 3
                || _crimeHistory.Any(c => c.Severity == CrimeSeverity.Capital);
        }

        // Polymorphism: overrides abstract method from Person
        public override string GenerateReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════════╗");
            sb.AppendLine("║           CRIMINAL PROFILE REPORT               ║");
            sb.AppendLine("╚══════════════════════════════════════════════════╝");
            sb.AppendLine($"  ID            : {Id}");
            sb.AppendLine($"  Name          : {Name}");
            sb.AppendLine($"  CNIC          : {Cnic}");
            sb.AppendLine($"  Age           : {Age}");
            sb.AppendLine($"  DOB           : {DateOfBirth:yyyy-MM-dd}");
            sb.AppendLine($"  Address       : {Address}");
            sb.AppendLine($"  Phone         : {PhoneNumber}");
            sb.AppendLine($"  Nationality   : {Nationality}");
            sb.AppendLine($"  Description   : {PhysicalDescription}");
            sb.AppendLine($"  Danger Level  : {DangerLevel}");
            sb.AppendLine($"  Most Wanted   : {(IsMostWanted ? "YES ⚠" : "No")}");
            sb.AppendLine($"  Arrested      : {(IsArrested ? $"Yes ({ArrestDate:yyyy-MM-dd})" : "No / Fugitive")}");
            sb.AppendLine($"  Total Crimes  : {_crimeHistory.Count}");
            sb.AppendLine();
            sb.AppendLine("  ── CRIME HISTORY ──");
            if (_crimeHistory.Count == 0)
                sb.AppendLine("  No crimes recorded.");
            else
                foreach (var c in _crimeHistory)
                    sb.AppendLine(c.ToString());
            return sb.ToString();
        }
    }
}