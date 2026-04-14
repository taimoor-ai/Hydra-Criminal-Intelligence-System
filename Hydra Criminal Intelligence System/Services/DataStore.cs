// ============================================================
// Services/DataStore.cs
// Central in-memory data repository - singleton pattern
// Holds all Lists and Dictionaries of application data
// ============================================================
using CriminalRecordMS.Models;
using CriminalRecordMS.Utilities;

namespace CriminalRecordMS.Services
{
    public class DataStore
    {
        // Collections using Dictionary for O(1) lookup by ID
        public Dictionary<string, Criminal> Criminals { get; private set; } = new();
        public Dictionary<string, Officer> Officers { get; private set; } = new();
        public Dictionary<string, Case> Cases { get; private set; } = new();
        public Dictionary<string, User> Users { get; private set; } = new();
        public Dictionary<string, Victim> Victims { get; private set; } = new();

        public User? CurrentUser { get; set; }
        public bool IsLoggedIn => CurrentUser != null;

        // ── Load from files ─────────────────────────────────────
        public void LoadAll()
        {
            // Load Criminals
            var criminalDtos = FileHandler.Load<CriminalDTO>("criminals");
            Criminals = criminalDtos.ToDictionary(c => c.Id, c => c.ToCriminal());

            // Load Officers
            var officerDtos = FileHandler.Load<OfficerDTO>("officers");
            Officers = officerDtos.ToDictionary(o => o.Id, o => o.ToOfficer());

            // Load Cases (includes embedded evidence)
            var cases = FileHandler.LoadCases();
            Cases = cases.ToDictionary(c => c.CaseId);

            // Load Users
            var users = FileHandler.Load<User>("users");
            Users = users.ToDictionary(u => u.UserId);

            // Load Victims
            var victimDtos = FileHandler.Load<VictimDTO>("victims");
            Victims = victimDtos.ToDictionary(v => v.Id, v => v.ToVictim());

            // Ensure default admin exists
            if (!Users.Values.Any(u => u.Role == UserRole.Admin))
                SeedDefaultUsers();

            ConsoleUI.PrintSuccess("Data loaded successfully.");
        }

        // ── Save to files ───────────────────────────────────────
        public void SaveAll()
        {
            FileHandler.Save("criminals", Criminals.Values.Select(c => new CriminalDTO(c)).ToList());
            FileHandler.Save("officers", Officers.Values.Select(o => new OfficerDTO(o)).ToList());
            FileHandler.SaveCases(Cases.Values.ToList());
            FileHandler.Save("users", Users.Values.ToList());
            FileHandler.Save("victims", Victims.Values.Select(v => new VictimDTO(v)).ToList());
        }

        // ── Seeding ─────────────────────────────────────────────
        public void SeedDefaultUsers()
        {
            var admin = new User("USR-001", "admin",
                PasswordHelper.Hash("admin123"), UserRole.Admin);
            Users[admin.UserId] = admin;
        }

        public void SeedSampleData()
        {
            if (Criminals.Count > 0) return; // Don't seed if data exists

            // -- Officers --
            var o1 = new Officer("OFC-201", "Inspector Khalid Mahmood",
                "3520212345671", "123 Police Colony, Lahore", "03001234567",
                new DateTime(1980, 5, 15), "B-2041", OfficerRank.Inspector,
                "Criminal Investigation", "khalid.mahmood", PasswordHelper.Hash("officer123"));
            var o2 = new Officer("OFC-202", "Sub-Inspector Ayesha Noor",
                "3520298765432", "45 Garden Town, Lahore", "03211234567",
                new DateTime(1990, 8, 22), "B-2042", OfficerRank.SubInspector,
                "Cybercrime", "ayesha.noor", PasswordHelper.Hash("officer123"));
            var o3 = new Officer("OFC-203", "DSP Tariq Bashir",
                "3520311223344", "7 Officers Mess, Rawalpindi", "03451234567",
                new DateTime(1975, 3, 10), "B-1001", OfficerRank.DSP,
                "Homicide", "tariq.bashir", PasswordHelper.Hash("officer123"));

            Officers[o1.Id] = o1;
            Officers[o2.Id] = o2;
            Officers[o3.Id] = o3;

            // Officer users
            var u2 = new User("USR-002", "khalid.mahmood", PasswordHelper.Hash("officer123"),
                UserRole.Officer, o1.Id);
            var u3 = new User("USR-003", "ayesha.noor", PasswordHelper.Hash("officer123"),
                UserRole.Officer, o2.Id);
            Users[u2.UserId] = u2;
            Users[u3.UserId] = u3;

            // -- Criminals --
            var cr1 = new Criminal("CRM-101", "Ahmed Raza Khan", "3520211122334",
                "Unknown - Fugitive", "N/A", new DateTime(1985, 6, 20),
                "Pakistani", "5'10\", Athletic build, scar on left cheek, black hair");
            cr1.AddCrime(new CrimeRecord("CRC-601", "Armed Robbery", "Bank robbery with firearm",
                new DateTime(2021, 3, 10), CrimeSeverity.Capital, "CSE-301"));
            cr1.AddCrime(new CrimeRecord("CRC-602", "Murder", "Shot during robbery",
                new DateTime(2021, 3, 10), CrimeSeverity.Capital, "CSE-301"));
            cr1.AddCrime(new CrimeRecord("CRC-603", "Kidnapping", "Hostage taken",
                new DateTime(2023, 7, 14), CrimeSeverity.Major, "CSE-302"));

            var cr2 = new Criminal("CRM-102", "Tariq Shah", "3520244556677",
                "Flat 4B, Orangi Town, Karachi", "03122223333",
                new DateTime(1992, 11, 5), "Pakistani",
                "5'7\", Medium build, tattooed forearms, brown eyes");
            cr2.AddCrime(new CrimeRecord("CRC-604", "Drug Trafficking", "Heroin distribution",
                new DateTime(2022, 1, 20), CrimeSeverity.Major, "CSE-303"));
            cr2.IsArrested = true;
            cr2.ArrestDate = new DateTime(2023, 4, 15);

            var cr3 = new Criminal("CRM-103", "Zara Malik", "3520277889900",
                "House 12, DHA Phase 2, Islamabad", "03331234567",
                new DateTime(1995, 2, 14), "Pakistani",
                "5'5\", Slim build, glasses, short hair");
            cr3.AddCrime(new CrimeRecord("CRC-605", "Cybercrime", "Bank account fraud",
                new DateTime(2023, 9, 1), CrimeSeverity.Moderate, "CSE-304"));

            Criminals[cr1.Id] = cr1;
            Criminals[cr2.Id] = cr2;
            Criminals[cr3.Id] = cr3;

            // -- Cases --
            var cs1 = new Case("CSE-301", "HBL Bank Robbery - 2021",
                "Armed robbery at HBL Gulberg branch; one casualty",
                "HBL Gulberg Branch, Lahore", CrimeSeverity.Capital);
            cs1.Status = CaseStatus.UnderInvestigation;
            cs1.AddCriminal(cr1.Id);
            cs1.AssignOfficer(o1.Id);
            cs1.AssignOfficer(o3.Id);
            cs1.PrimaryOfficerId = o1.Id;
            o1.AssignCase(cs1.CaseId);
            o3.AssignCase(cs1.CaseId);
            // Evidence
            cs1.AddEvidence(new WeaponEvidence("EVD-401", cs1.CaseId,
                "9mm Glock recovered near exit", new DateTime(2021, 3, 11),
                o1.Id, "Firearm", "GL-19-4567", false)
            { StorageLocation = "Vault B" });
            cs1.AddEvidence(new DigitalEvidence("EVD-402", cs1.CaseId,
                "CCTV footage of robbery", new DateTime(2021, 3, 11),
                o1.Id, "CCTV Hard Drive", "a1b2c3d4e5f6", "MP4")
            { FileSizeKb = 2048000 });

            var cs2 = new Case("CSE-302", "Raza Kidnapping Case",
                "Businessman's son kidnapped for ransom",
                "Model Town, Lahore", CrimeSeverity.Major);
            cs2.Status = CaseStatus.Open;
            cs2.AddCriminal(cr1.Id);
            cs2.AssignOfficer(o1.Id);
            cs2.PrimaryOfficerId = o1.Id;
            o1.AssignCase(cs2.CaseId);
            cs2.AddEvidence(new DocumentEvidence("EVD-403", cs2.CaseId,
                "Ransom note left at scene", new DateTime(2023, 7, 15),
                o1.Id, "Ransom Note", "Letter")
            { IsForensicallyCertified = true });

            var cs3 = new Case("CSE-303", "Narcotics Operation Sweep",
                "Large heroin cache seized from distribution network",
                "Orangi Town, Karachi", CrimeSeverity.Major);
            cs3.Status = CaseStatus.Closed;
            cs3.AddCriminal(cr2.Id);
            cs3.AssignOfficer(o2.Id);
            cs3.PrimaryOfficerId = o2.Id;
            o2.AssignCase(cs3.CaseId);
            cs3.CloseCase();
            cs3.AddEvidence(new DocumentEvidence("EVD-404", cs3.CaseId,
                "Seized distribution ledger", new DateTime(2023, 4, 16),
                o2.Id, "Drug Ledger", "Financial Record"));

            var cs4 = new Case("CSE-304", "Online Banking Fraud",
                "Multiple accounts compromised via phishing",
                "Online / Islamabad", CrimeSeverity.Moderate);
            cs4.Status = CaseStatus.UnderInvestigation;
            cs4.AddCriminal(cr3.Id);
            cs4.AssignOfficer(o2.Id);
            cs4.PrimaryOfficerId = o2.Id;
            o2.AssignCase(cs4.CaseId);
            cs4.AddEvidence(new DigitalEvidence("EVD-405", cs4.CaseId,
                "Suspect's laptop with phishing toolkit", new DateTime(2023, 9, 3),
                o2.Id, "Laptop", "sha256:abc123def456", "Mixed")
            { IsEncrypted = true, FileSizeKb = 500 });

            Cases[cs1.CaseId] = cs1;
            Cases[cs2.CaseId] = cs2;
            Cases[cs3.CaseId] = cs3;
            Cases[cs4.CaseId] = cs4;

            SaveAll();
            ConsoleUI.PrintSuccess("Sample data seeded and saved.");
        }
    }

    // Victim DTO for file serialization
    public class VictimDTO
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Cnic { get; set; } = "";
        public string Address { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string CaseId { get; set; } = "";
        public string InjuryDescription { get; set; } = "";
        public bool IsWitness { get; set; }
        public string StatementSummary { get; set; } = "";

        public VictimDTO() { }
        public VictimDTO(Victim v)
        {
            Id = v.Id; Name = v.Name; Cnic = v.Cnic; Address = v.Address;
            PhoneNumber = v.PhoneNumber; DateOfBirth = v.DateOfBirth;
            CaseId = v.CaseId; InjuryDescription = v.InjuryDescription;
            IsWitness = v.IsWitness; StatementSummary = v.StatementSummary;
        }
        public Victim ToVictim()
        {
            var v = new Victim(Id, Name, Cnic, Address, PhoneNumber, DateOfBirth,
                CaseId, InjuryDescription, IsWitness)
            { StatementSummary = StatementSummary };
            return v;
        }
    }
}