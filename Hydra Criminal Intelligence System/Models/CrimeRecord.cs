// ============================================================
// Models/CrimeRecord.cs
// Represents one crime entry in a criminal's history
// ============================================================
namespace CriminalRecordMS.Models
{
    public class CrimeRecord
    {
        public string CrimeId { get; set; }
        public string CrimeType { get; set; }
        public string Description { get; set; }
        public DateTime DateCommitted { get; set; }
        public CrimeSeverity Severity { get; set; }
        public string CaseId { get; set; }

        public CrimeRecord(string crimeId, string crimeType, string description,
                           DateTime dateCommitted, CrimeSeverity severity, string caseId)
        {
            CrimeId = crimeId;
            CrimeType = crimeType;
            Description = description;
            DateCommitted = dateCommitted;
            Severity = severity;
            CaseId = caseId;
        }

        public override string ToString()
            => $"  [{CrimeId}] {CrimeType} | {Severity} | Date: {DateCommitted:yyyy-MM-dd} | Case: {CaseId}";
    }
}