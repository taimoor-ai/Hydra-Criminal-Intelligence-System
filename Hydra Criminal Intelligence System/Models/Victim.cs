// ============================================================
// Models/Victim.cs
// Inherits from Person. Represents a victim linked to a case.
// ============================================================
namespace CriminalRecordMS.Models
{
    public class Victim : Person
    {
        public string CaseId { get; set; }
        public string InjuryDescription { get; set; }
        public bool IsWitness { get; set; }
        public string StatementSummary { get; set; }

        public Victim(string id, string name, string cnic, string address,
                      string phone, DateTime dob, string caseId,
                      string injuryDescription, bool isWitness)
            : base(id, name, cnic, address, phone, dob)
        {
            CaseId = caseId;
            InjuryDescription = injuryDescription;
            IsWitness = isWitness;
            StatementSummary = "";
        }

        // Polymorphism: overrides abstract method from Person
        public override string GenerateReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════════╗");
            sb.AppendLine("║             VICTIM PROFILE REPORT               ║");
            sb.AppendLine("╚══════════════════════════════════════════════════╝");
            sb.AppendLine($"  ID            : {Id}");
            sb.AppendLine($"  Name          : {Name}");
            sb.AppendLine($"  CNIC          : {Cnic}");
            sb.AppendLine($"  Age           : {Age}");
            sb.AppendLine($"  Phone         : {PhoneNumber}");
            sb.AppendLine($"  Address       : {Address}");
            sb.AppendLine($"  Case ID       : {CaseId}");
            sb.AppendLine($"  Injury        : {InjuryDescription}");
            sb.AppendLine($"  Is Witness    : {(IsWitness ? "Yes" : "No")}");
            sb.AppendLine($"  Statement     : {StatementSummary}");
            return sb.ToString();
        }
    }
}