// ============================================================
// Utilities/FileHandler.cs
// Handles saving and loading all data to/from JSON files
// ============================================================
using System.Text.Json;
using System.Text.Json.Serialization;
using CriminalRecordMS.Models;

namespace CriminalRecordMS.Utilities
{
    public static class FileHandler
    {
        private static readonly string DataDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data");

        private static readonly JsonSerializerOptions _opts = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        static FileHandler()
        {
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);
        }

        private static string Path_(string name) => Path.Combine(DataDir, name + ".json");

        // ── Generic Save/Load ─────────────────────────────────

        public static void Save<T>(string fileName, List<T> data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _opts);
                File.WriteAllText(Path_(fileName), json);
            }
            catch (Exception ex)
            {
                ConsoleUI.PrintError($"Failed to save {fileName}: {ex.Message}");
            }
        }

        public static List<T> Load<T>(string fileName)
        {
            try
            {
                var path = Path_(fileName);
                if (!File.Exists(path)) return new List<T>();
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<T>>(json, _opts) ?? new List<T>();
            }
            catch (Exception ex)
            {
                ConsoleUI.PrintError($"Failed to load {fileName}: {ex.Message}");
                return new List<T>();
            }
        }

        // ── Evidence requires polymorphic handling ─────────────

        public static void SaveCases(List<Case> cases)
        {
            try
            {
                var dto = cases.Select(c => new CaseDTO(c)).ToList();
                var json = JsonSerializer.Serialize(dto, _opts);
                File.WriteAllText(Path_("cases"), json);
            }
            catch (Exception ex)
            {
                ConsoleUI.PrintError($"Save cases error: {ex.Message}");
            }
        }

        public static List<Case> LoadCases()
        {
            try
            {
                var path = Path_("cases");
                if (!File.Exists(path)) return new List<Case>();
                var json = File.ReadAllText(path);
                var dtos = JsonSerializer.Deserialize<List<CaseDTO>>(json, _opts)
                           ?? new List<CaseDTO>();
                return dtos.Select(d => d.ToCase()).ToList();
            }
            catch (Exception ex)
            {
                ConsoleUI.PrintError($"Load cases error: {ex.Message}");
                return new List<Case>();
            }
        }
    }

    // ── Data Transfer Objects for JSON serialization ──────────

    public class CrimeRecordDTO
    {
        public string CrimeId { get; set; } = "";
        public string CrimeType { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DateCommitted { get; set; }
        public CrimeSeverity Severity { get; set; }
        public string CaseId { get; set; } = "";

        public CrimeRecordDTO() { }
        public CrimeRecordDTO(CrimeRecord r)
        {
            CrimeId = r.CrimeId; CrimeType = r.CrimeType;
            Description = r.Description; DateCommitted = r.DateCommitted;
            Severity = r.Severity; CaseId = r.CaseId;
        }
        public CrimeRecord ToRecord() => new(CrimeId, CrimeType, Description,
            DateCommitted, Severity, CaseId);
    }

    public class CriminalDTO
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Cnic { get; set; } = "";
        public string Address { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; } = "";
        public string PhysicalDescription { get; set; } = "";
        public bool IsArrested { get; set; }
        public DateTime? ArrestDate { get; set; }
        public List<CrimeRecordDTO> CrimeHistory { get; set; } = new();

        public CriminalDTO() { }
        public CriminalDTO(Criminal c)
        {
            Id = c.Id; Name = c.Name; Cnic = c.Cnic; Address = c.Address;
            PhoneNumber = c.PhoneNumber; DateOfBirth = c.DateOfBirth;
            Nationality = c.Nationality; PhysicalDescription = c.PhysicalDescription;
            IsArrested = c.IsArrested; ArrestDate = c.ArrestDate;
            CrimeHistory = c.CrimeHistory.Select(r => new CrimeRecordDTO(r)).ToList();
        }

        public Criminal ToCriminal()
        {
            var cr = new Criminal(Id, Name, Cnic, Address, PhoneNumber,
                DateOfBirth, Nationality, PhysicalDescription)
            { IsArrested = IsArrested, ArrestDate = ArrestDate };
            foreach (var r in CrimeHistory) cr.AddCrime(r.ToRecord());
            return cr;
        }
    }

    public class OfficerDTO
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Cnic { get; set; } = "";
        public string Address { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string BadgeNumber { get; set; } = "";
        public OfficerRank Rank { get; set; }
        public string Department { get; set; } = "";
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool IsActive { get; set; }
        public List<string> AssignedCaseIds { get; set; } = new();

        public OfficerDTO() { }
        public OfficerDTO(Officer o)
        {
            Id = o.Id; Name = o.Name; Cnic = o.Cnic; Address = o.Address;
            PhoneNumber = o.PhoneNumber; DateOfBirth = o.DateOfBirth;
            BadgeNumber = o.BadgeNumber; Rank = o.Rank; Department = o.Department;
            Username = o.Username; PasswordHash = o.PasswordHash;
            IsActive = o.IsActive;
            AssignedCaseIds = new List<string>(o.AssignedCaseIds);
        }

        public Officer ToOfficer()
        {
            var of = new Officer(Id, Name, Cnic, Address, PhoneNumber, DateOfBirth,
                BadgeNumber, Rank, Department, Username, PasswordHash)
            { IsActive = IsActive };
            foreach (var cid in AssignedCaseIds) of.AssignCase(cid);
            return of;
        }
    }

    public class EvidenceDTO
    {
        public string EvidenceId { get; set; } = "";
        public string CaseId { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CollectedOn { get; set; }
        public string CollectedBy { get; set; } = "";
        public EvidenceType Type { get; set; }
        public string StorageLocation { get; set; } = "";
        // Document
        public string DocumentTitle { get; set; } = "";
        public string DocumentType { get; set; } = "";
        public bool IsForensicallyCertified { get; set; }
        // Weapon
        public string WeaponType { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public bool IsRegistered { get; set; }
        public string Manufacturer { get; set; } = "";
        // Digital
        public string DeviceType { get; set; } = "";
        public string HashValue { get; set; } = "";
        public string FileFormat { get; set; } = "";
        public bool IsEncrypted { get; set; }
        public long FileSizeKb { get; set; }

        public EvidenceDTO() { }
        public EvidenceDTO(Evidence e)
        {
            EvidenceId = e.EvidenceId; CaseId = e.CaseId;
            Description = e.Description; CollectedOn = e.CollectedOn;
            CollectedBy = e.CollectedBy; Type = e.Type;
            StorageLocation = e.StorageLocation;
            if (e is DocumentEvidence d)
            {
                DocumentTitle = d.DocumentTitle; DocumentType = d.DocumentType;
                IsForensicallyCertified = d.IsForensicallyCertified;
            }
            else if (e is WeaponEvidence w)
            {
                WeaponType = w.WeaponType; SerialNumber = w.SerialNumber;
                IsRegistered = w.IsRegistered; Manufacturer = w.Manufacturer;
            }
            else if (e is DigitalEvidence dig)
            {
                DeviceType = dig.DeviceType; HashValue = dig.HashValue;
                FileFormat = dig.FileFormat; IsEncrypted = dig.IsEncrypted;
                FileSizeKb = dig.FileSizeKb;
            }
        }

        public Evidence ToEvidence()
        {
            return Type switch
            {
                EvidenceType.Document => new DocumentEvidence(EvidenceId, CaseId, Description,
                    CollectedOn, CollectedBy, DocumentTitle, DocumentType)
                { StorageLocation = StorageLocation, IsForensicallyCertified = IsForensicallyCertified },
                EvidenceType.Weapon => new WeaponEvidence(EvidenceId, CaseId, Description,
                    CollectedOn, CollectedBy, WeaponType, SerialNumber, IsRegistered)
                { StorageLocation = StorageLocation, Manufacturer = Manufacturer },
                EvidenceType.Digital => new DigitalEvidence(EvidenceId, CaseId, Description,
                    CollectedOn, CollectedBy, DeviceType, HashValue, FileFormat)
                { StorageLocation = StorageLocation, IsEncrypted = IsEncrypted, FileSizeKb = FileSizeKb },
                _ => throw new InvalidOperationException("Unknown evidence type")
            };
        }
    }

    public class CaseDTO
    {
        public string CaseId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public CrimeSeverity Severity { get; set; }
        public CaseStatus Status { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public string PrimaryOfficerId { get; set; } = "";
        public List<string> CriminalIds { get; set; } = new();
        public List<string> OfficerIds { get; set; } = new();
        public List<string> VictimIds { get; set; } = new();
        public List<EvidenceDTO> Evidence { get; set; } = new();

        public CaseDTO() { }
        public CaseDTO(Case c)
        {
            CaseId = c.CaseId; Title = c.Title; Description = c.Description;
            Location = c.Location; Severity = c.Severity; Status = c.Status;
            DateOpened = c.DateOpened; DateClosed = c.DateClosed;
            PrimaryOfficerId = c.PrimaryOfficerId ?? "";
            CriminalIds = new List<string>(c.CriminalIds);
            OfficerIds = new List<string>(c.OfficerIds);
            VictimIds = new List<string>(c.VictimIds);
            Evidence = c.EvidenceList.Select(e => new EvidenceDTO(e)).ToList();
        }

        public Case ToCase()
        {
            var cs = new Case(CaseId, Title, Description, Location, Severity)
            {
                Status = Status,
                DateClosed = DateClosed,
                PrimaryOfficerId = PrimaryOfficerId
            };
            foreach (var cid in CriminalIds) cs.AddCriminal(cid);
            foreach (var oid in OfficerIds) cs.AssignOfficer(oid);
            foreach (var vid in VictimIds) cs.AddVictim(vid);
            foreach (var e in Evidence) cs.AddEvidence(e.ToEvidence());
            return cs;
        }
    }
}