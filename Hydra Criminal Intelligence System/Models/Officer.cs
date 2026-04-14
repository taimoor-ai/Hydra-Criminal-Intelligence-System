// ============================================================
// Models/Officer.cs
// Inherits from Person. Represents a police officer.
// ============================================================
namespace CriminalRecordMS.Models
{
    public class Officer : Person
    {
        private string _badgeNumber;
        private OfficerRank _rank;
        private List<string> _assignedCaseIds;

        public string BadgeNumber
        {
            get => _badgeNumber;
            set => _badgeNumber = !string.IsNullOrWhiteSpace(value) ? value
                : throw new ArgumentException("Badge number cannot be empty.");
        }

        public OfficerRank Rank
        {
            get => _rank;
            set => _rank = value;
        }

        public string Department { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public List<string> AssignedCaseIds
        {
            get => _assignedCaseIds;
            private set => _assignedCaseIds = value ?? new List<string>();
        }

        public int CasesHandled => _assignedCaseIds.Count;

        public Officer(string id, string name, string cnic, string address,
                       string phone, DateTime dob, string badgeNumber,
                       OfficerRank rank, string department,
                       string username, string passwordHash)
            : base(id, name, cnic, address, phone, dob)
        {
            BadgeNumber = badgeNumber;
            Rank = rank;
            Department = department;
            JoinDate = DateTime.Now;
            IsActive = true;
            Username = username;
            PasswordHash = passwordHash;
            _assignedCaseIds = new List<string>();
        }

        public void AssignCase(string caseId)
        {
            if (!_assignedCaseIds.Contains(caseId))
                _assignedCaseIds.Add(caseId);
        }

        public void RemoveCase(string caseId)
        {
            _assignedCaseIds.Remove(caseId);
        }

        // Polymorphism: overrides abstract method from Person
        public override string GenerateReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════════╗");
            sb.AppendLine("║           OFFICER PROFILE REPORT                ║");
            sb.AppendLine("╚══════════════════════════════════════════════════╝");
            sb.AppendLine($"  ID            : {Id}");
            sb.AppendLine($"  Name          : {Name}");
            sb.AppendLine($"  Badge No.     : {BadgeNumber}");
            sb.AppendLine($"  Rank          : {Rank}");
            sb.AppendLine($"  Department    : {Department}");
            sb.AppendLine($"  CNIC          : {Cnic}");
            sb.AppendLine($"  Age           : {Age}");
            sb.AppendLine($"  Phone         : {PhoneNumber}");
            sb.AppendLine($"  Join Date     : {JoinDate:yyyy-MM-dd}");
            sb.AppendLine($"  Status        : {(IsActive ? "Active" : "Inactive")}");
            sb.AppendLine($"  Cases Handled : {CasesHandled}");
            sb.AppendLine();
            sb.AppendLine("  ── ASSIGNED CASES ──");
            if (_assignedCaseIds.Count == 0)
                sb.AppendLine("  No cases assigned.");
            else
                foreach (var cid in _assignedCaseIds)
                    sb.AppendLine($"  - Case ID: {cid}");
            return sb.ToString();
        }
    }
}