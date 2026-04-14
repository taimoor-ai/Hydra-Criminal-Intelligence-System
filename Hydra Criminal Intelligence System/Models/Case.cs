// ============================================================
// Models/Case.cs
// Represents a criminal case. Implements IReportable.
// ============================================================
using CriminalRecordMS.Interfaces;

namespace CriminalRecordMS.Models
{
    public class Case : IReportable
    {
        private string _caseId;
        private string _title;
        private CaseStatus _status;
        private List<string> _criminalIds;
        private List<string> _officerIds;
        private List<Evidence> _evidenceList;
        private List<string> _victimIds;

        public string CaseId
        {
            get => _caseId;
            private set => _caseId = !string.IsNullOrWhiteSpace(value) ? value
                : throw new ArgumentException("Case ID cannot be empty.");
        }

        public string Title
        {
            get => _title;
            set => _title = !string.IsNullOrWhiteSpace(value) ? value
                : throw new ArgumentException("Case title cannot be empty.");
        }

        public string Description { get; set; }
        public CaseStatus Status { get => _status; set => _status = value; }
        public DateTime DateOpened { get; private set; }
        public DateTime? DateClosed { get; set; }
        public string Location { get; set; }
        public CrimeSeverity Severity { get; set; }
        public string PrimaryOfficerId { get; set; }

        public List<string> CriminalIds => _criminalIds;
        public List<string> OfficerIds => _officerIds;
        public List<Evidence> EvidenceList => _evidenceList;
        public List<string> VictimIds => _victimIds;

        public Case(string caseId, string title, string description,
                    string location, CrimeSeverity severity)
        {
            CaseId = caseId;
            Title = title;
            Description = description;
            Location = location;
            Severity = severity;
            _status = CaseStatus.Open;
            DateOpened = DateTime.Now;
            _criminalIds = new List<string>();
            _officerIds = new List<string>();
            _evidenceList = new List<Evidence>();
            _victimIds = new List<string>();
        }

        public void AddCriminal(string criminalId)
        {
            if (!_criminalIds.Contains(criminalId))
                _criminalIds.Add(criminalId);
        }

        public void RemoveCriminal(string criminalId) => _criminalIds.Remove(criminalId);

        public void AssignOfficer(string officerId)
        {
            if (!_officerIds.Contains(officerId))
                _officerIds.Add(officerId);
        }

        public void RemoveOfficer(string officerId) => _officerIds.Remove(officerId);

        public void AddEvidence(Evidence evidence)
        {
            if (evidence == null) throw new ArgumentNullException(nameof(evidence));
            _evidenceList.Add(evidence);
        }

        public void AddVictim(string victimId)
        {
            if (!_victimIds.Contains(victimId))
                _victimIds.Add(victimId);
        }

        public void CloseCase()
        {
            _status = CaseStatus.Closed;
            DateClosed = DateTime.Now;
        }

        public string GenerateReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════════╗");
            sb.AppendLine("║              CASE SUMMARY REPORT                ║");
            sb.AppendLine("╚══════════════════════════════════════════════════╝");
            sb.AppendLine($"  Case ID       : {CaseId}");
            sb.AppendLine($"  Title         : {Title}");
            sb.AppendLine($"  Description   : {Description}");
            sb.AppendLine($"  Location      : {Location}");
            sb.AppendLine($"  Severity      : {Severity}");
            sb.AppendLine($"  Status        : {Status}");
            sb.AppendLine($"  Date Opened   : {DateOpened:yyyy-MM-dd}");
            sb.AppendLine($"  Date Closed   : {(DateClosed.HasValue ? DateClosed.Value.ToString("yyyy-MM-dd") : "Ongoing")}");
            sb.AppendLine($"  Criminals     : {string.Join(", ", _criminalIds)}");
            sb.AppendLine($"  Officers      : {string.Join(", ", _officerIds)}");
            sb.AppendLine($"  Evidence Count: {_evidenceList.Count}");
            sb.AppendLine($"  Victim Count  : {_victimIds.Count}");
            sb.AppendLine();
            sb.AppendLine("  ── EVIDENCE ──");
            if (_evidenceList.Count == 0)
                sb.AppendLine("  No evidence recorded.");
            else
                foreach (var e in _evidenceList)
                    sb.AppendLine(e.ToString());
            return sb.ToString();
        }

        public void PrintReport() => Console.WriteLine(GenerateReport());

        public override string ToString()
            => $"[{CaseId}] {Title} | Status: {Status} | Severity: {Severity}";
    }
}