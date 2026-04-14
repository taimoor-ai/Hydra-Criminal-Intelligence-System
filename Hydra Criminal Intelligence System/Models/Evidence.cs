// ============================================================
// Models/Evidence.cs
// Base class for evidence. Demonstrates: Inheritance, Polymorphism
// Subtypes: DocumentEvidence, WeaponEvidence, DigitalEvidence
// ============================================================
using CriminalRecordMS.Interfaces;

namespace CriminalRecordMS.Models
{
    // ── Abstract base ──────────────────────────────────────────
    public abstract class Evidence : IReportable
    {
        public string EvidenceId { get; protected set; }
        public string CaseId { get; set; }
        public string Description { get; set; }
        public DateTime CollectedOn { get; set; }
        public string CollectedBy { get; set; }  // Officer ID
        public EvidenceType Type { get; protected set; }
        public string StorageLocation { get; set; }

        protected Evidence(string evidenceId, string caseId, string description,
                           DateTime collectedOn, string collectedBy, EvidenceType type)
        {
            EvidenceId = evidenceId;
            CaseId = caseId;
            Description = description;
            CollectedOn = collectedOn;
            CollectedBy = collectedBy;
            Type = type;
            StorageLocation = "Evidence Room A";
        }

        // Abstract - each subtype overrides to display type-specific info
        public abstract string GetTypeSpecificDetails();

        // Polymorphism: overridden in subclasses
        public virtual string GenerateReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"  ┌─ Evidence ID  : {EvidenceId}");
            sb.AppendLine($"  │  Type         : {Type}");
            sb.AppendLine($"  │  Case ID      : {CaseId}");
            sb.AppendLine($"  │  Description  : {Description}");
            sb.AppendLine($"  │  Collected On : {CollectedOn:yyyy-MM-dd}");
            sb.AppendLine($"  │  Collected By : {CollectedBy}");
            sb.AppendLine($"  │  Storage      : {StorageLocation}");
            sb.AppendLine($"  └─ Details      : {GetTypeSpecificDetails()}");
            return sb.ToString();
        }

        public void PrintReport() => Console.WriteLine(GenerateReport());

        public override string ToString() => $"[{EvidenceId}] {Type} - {Description}";
    }

    // ── Document Evidence ─────────────────────────────────────
    public class DocumentEvidence : Evidence
    {
        public string DocumentTitle { get; set; }
        public string DocumentType { get; set; }   // e.g., "Contract", "Letter", "ID"
        public bool IsForensicallyCertified { get; set; }

        public DocumentEvidence(string evidenceId, string caseId, string description,
                                DateTime collectedOn, string collectedBy,
                                string documentTitle, string documentType)
            : base(evidenceId, caseId, description, collectedOn, collectedBy, EvidenceType.Document)
        {
            DocumentTitle = documentTitle;
            DocumentType = documentType;
            IsForensicallyCertified = false;
        }

        public override string GetTypeSpecificDetails()
            => $"Title: {DocumentTitle} | Doc Type: {DocumentType} | Certified: {IsForensicallyCertified}";

        public override string GenerateReport()
        {
            var base_ = base.GenerateReport();
            return base_ + $"     Forensic Cert : {IsForensicallyCertified}\n";
        }
    }

    // ── Weapon Evidence ───────────────────────────────────────
    public class WeaponEvidence : Evidence
    {
        public string WeaponType { get; set; }   // Firearm, Knife, Blunt, etc.
        public string SerialNumber { get; set; }
        public bool IsRegistered { get; set; }
        public string Manufacturer { get; set; }

        public WeaponEvidence(string evidenceId, string caseId, string description,
                              DateTime collectedOn, string collectedBy,
                              string weaponType, string serialNumber, bool isRegistered)
            : base(evidenceId, caseId, description, collectedOn, collectedBy, EvidenceType.Weapon)
        {
            WeaponType = weaponType;
            SerialNumber = serialNumber;
            IsRegistered = isRegistered;
            Manufacturer = "Unknown";
        }

        public override string GetTypeSpecificDetails()
            => $"Weapon: {WeaponType} | Serial: {SerialNumber} | Registered: {IsRegistered} | Maker: {Manufacturer}";
    }

    // ── Digital Evidence ──────────────────────────────────────
    public class DigitalEvidence : Evidence
    {
        public string DeviceType { get; set; }   // Laptop, Phone, USB, etc.
        public string HashValue { get; set; }    // MD5/SHA-256 for integrity
        public string FileFormat { get; set; }
        public bool IsEncrypted { get; set; }
        public long FileSizeKb { get; set; }

        public DigitalEvidence(string evidenceId, string caseId, string description,
                               DateTime collectedOn, string collectedBy,
                               string deviceType, string hashValue, string fileFormat)
            : base(evidenceId, caseId, description, collectedOn, collectedBy, EvidenceType.Digital)
        {
            DeviceType = deviceType;
            HashValue = hashValue;
            FileFormat = fileFormat;
            IsEncrypted = false;
            FileSizeKb = 0;
        }

        public override string GetTypeSpecificDetails()
            => $"Device: {DeviceType} | Format: {FileFormat} | Hash: {HashValue} | Encrypted: {IsEncrypted} | Size: {FileSizeKb}KB";
    }
}