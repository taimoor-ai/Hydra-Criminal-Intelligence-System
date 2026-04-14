// ============================================================
// Services/CaseService.cs
// Business logic for case management
// ============================================================
using CriminalRecordMS.Models;
using CriminalRecordMS.Utilities;

namespace CriminalRecordMS.Services
{
    public class CaseService
    {
        private readonly DataStore _store;

        public CaseService(DataStore store) => _store = store;

        public Case CreateCase(string title, string description, string location,
                               CrimeSeverity severity)
        {
            var id = IdGenerator.NewCaseId();
            var cs = new Case(id, title, description, location, severity);
            _store.Cases[id] = cs;
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Case created with ID: {id}");
            return cs;
        }

        public void AssignOfficer(string caseId, string officerId)
        {
            var cs = GetById(caseId);
            if (!_store.Officers.ContainsKey(officerId))
                throw new Exception($"Officer {officerId} not found.");
            cs.AssignOfficer(officerId);
            _store.Officers[officerId].AssignCase(caseId);
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Officer {officerId} assigned to case {caseId}.");
        }

        public void RemoveOfficer(string caseId, string officerId)
        {
            var cs = GetById(caseId);
            cs.RemoveOfficer(officerId);
            if (_store.Officers.TryGetValue(officerId, out var of))
                of.RemoveCase(caseId);
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Officer {officerId} removed from case {caseId}.");
        }

        public void LinkCriminal(string caseId, string criminalId)
        {
            var cs = GetById(caseId);
            if (!_store.Criminals.ContainsKey(criminalId))
                throw new Exception($"Criminal {criminalId} not found.");
            cs.AddCriminal(criminalId);
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Criminal {criminalId} linked to case {caseId}.");
        }

        public void UpdateStatus(string caseId, CaseStatus status)
        {
            var cs = GetById(caseId);
            cs.Status = status;
            if (status == CaseStatus.Closed) cs.CloseCase();
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Case {caseId} status updated to {status}.");
        }

        public Case GetById(string id)
        {
            if (!_store.Cases.TryGetValue(id, out var cs))
                throw new Exception($"Case {id} not found.");
            return cs;
        }

        public List<Case> GetAll() => _store.Cases.Values.ToList();

        public List<Case> GetByStatus(CaseStatus status)
            => _store.Cases.Values.Where(c => c.Status == status).ToList();

        public List<Case> GetByOfficer(string officerId)
            => _store.Cases.Values.Where(c => c.OfficerIds.Contains(officerId)).ToList();
    }

    // ============================================================
    // Services/OfficerService.cs
    // ============================================================
    public class OfficerService
    {
        private readonly DataStore _store;

        public OfficerService(DataStore store) => _store = store;

        public Officer AddOfficer(string name, string cnic, string address, string phone,
                                  DateTime dob, string badgeNumber, OfficerRank rank,
                                  string department, string username, string password)
        {
            if (_store.Officers.Values.Any(o => o.Username == username))
                throw new Exception($"Username '{username}' is already taken.");

            var id = IdGenerator.NewOfficerId();
            var hash = PasswordHelper.Hash(password);
            var officer = new Officer(id, name, cnic, address, phone, dob,
                                      badgeNumber, rank, department, username, hash);
            _store.Officers[id] = officer;

            // Create user account for officer
            var userId = $"USR-{_store.Users.Count + 100}";
            var user = new User(userId, username, hash, UserRole.Officer, id);
            _store.Users[userId] = user;

            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Officer added with ID: {id}");
            return officer;
        }

        public Officer GetById(string id)
        {
            if (!_store.Officers.TryGetValue(id, out var of))
                throw new Exception($"Officer {id} not found.");
            return of;
        }

        public List<Officer> GetAll() => _store.Officers.Values.ToList();

        public List<Officer> GetActive() => _store.Officers.Values.Where(o => o.IsActive).ToList();

        public void DeactivateOfficer(string id)
        {
            var of = GetById(id);
            of.IsActive = false;
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Officer {id} deactivated.");
        }

        public List<Case> GetOfficerCases(string officerId)
        {
            var of = GetById(officerId);
            return of.AssignedCaseIds
                .Where(cid => _store.Cases.ContainsKey(cid))
                .Select(cid => _store.Cases[cid])
                .ToList();
        }
    }

    // ============================================================
    // Services/EvidenceService.cs
    // ============================================================
    public class EvidenceService
    {
        private readonly DataStore _store;

        public EvidenceService(DataStore store) => _store = store;

        public DocumentEvidence AddDocumentEvidence(string caseId, string description,
            string collectedBy, string docTitle, string docType)
        {
            var cs = GetCase(caseId);
            var id = IdGenerator.NewEvidenceId();
            var ev = new DocumentEvidence(id, caseId, description, DateTime.Now,
                                          collectedBy, docTitle, docType);
            cs.AddEvidence(ev);
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Document evidence {id} added.");
            return ev;
        }

        public WeaponEvidence AddWeaponEvidence(string caseId, string description,
            string collectedBy, string weaponType, string serial, bool isRegistered)
        {
            var cs = GetCase(caseId);
            var id = IdGenerator.NewEvidenceId();
            var ev = new WeaponEvidence(id, caseId, description, DateTime.Now,
                                        collectedBy, weaponType, serial, isRegistered);
            cs.AddEvidence(ev);
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Weapon evidence {id} added.");
            return ev;
        }

        public DigitalEvidence AddDigitalEvidence(string caseId, string description,
            string collectedBy, string deviceType, string hash, string fileFormat)
        {
            var cs = GetCase(caseId);
            var id = IdGenerator.NewEvidenceId();
            var ev = new DigitalEvidence(id, caseId, description, DateTime.Now,
                                         collectedBy, deviceType, hash, fileFormat);
            cs.AddEvidence(ev);
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Digital evidence {id} added.");
            return ev;
        }

        public List<Evidence> GetEvidenceForCase(string caseId)
        {
            var cs = GetCase(caseId);
            return cs.EvidenceList;
        }

        private Case GetCase(string caseId)
        {
            if (!_store.Cases.TryGetValue(caseId, out var cs))
                throw new Exception($"Case {caseId} not found.");
            return cs;
        }
    }

    // ============================================================
    // Services/ReportService.cs
    // Implements IReportable indirectly via report generation
    // ============================================================
    public class ReportService
    {
        private readonly DataStore _store;

        public ReportService(DataStore store) => _store = store;

        // Polymorphism: Each entity's GenerateReport() is called
        public void CriminalHistoryReport(string criminalId)
        {
            if (!_store.Criminals.TryGetValue(criminalId, out var cr))
                throw new Exception($"Criminal {criminalId} not found.");
            // Polymorphic call - calls Criminal.GenerateReport()
            cr.PrintReport();
        }

        public void CaseSummaryReport(string caseId)
        {
            if (!_store.Cases.TryGetValue(caseId, out var cs))
                throw new Exception($"Case {caseId} not found.");
            cs.PrintReport();
        }

        public void OfficerPerformanceReport(string officerId)
        {
            if (!_store.Officers.TryGetValue(officerId, out var of))
                throw new Exception($"Officer {officerId} not found.");

            // Polymorphic call - calls Officer.GenerateReport()
            of.PrintReport();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ── PERFORMANCE METRICS ──");
            Console.ResetColor();

            var cases = _store.Cases.Values.Where(c => c.OfficerIds.Contains(officerId)).ToList();
            int total = cases.Count;
            int closed = cases.Count(c => c.Status == CaseStatus.Closed);
            int active = cases.Count(c => c.Status != CaseStatus.Closed);
            double rate = total > 0 ? (double)closed / total * 100 : 0;

            Console.WriteLine($"  Total Cases   : {total}");
            Console.WriteLine($"  Closed Cases  : {closed}");
            Console.WriteLine($"  Active Cases  : {active}");
            Console.WriteLine($"  Closure Rate  : {rate:F1}%");
        }

        public void SystemSummaryReport()
        {
            ConsoleUI.PrintHeader("SYSTEM SUMMARY REPORT");
            Console.WriteLine($"  Total Criminals : {_store.Criminals.Count}");
            Console.WriteLine($"  Total Officers  : {_store.Officers.Count}");
            Console.WriteLine($"  Total Cases     : {_store.Cases.Count}");
            Console.WriteLine($"    - Open               : {_store.Cases.Values.Count(c => c.Status == CaseStatus.Open)}");
            Console.WriteLine($"    - Under Investigation: {_store.Cases.Values.Count(c => c.Status == CaseStatus.UnderInvestigation)}");
            Console.WriteLine($"    - Closed             : {_store.Cases.Values.Count(c => c.Status == CaseStatus.Closed)}");
            Console.WriteLine($"  Arrested Criminals : {_store.Criminals.Values.Count(c => c.IsArrested)}");
            Console.WriteLine($"  Most Wanted        : {_store.Criminals.Values.Count(c => c.IsMostWanted && !c.IsArrested)}");

            ConsoleUI.PrintDivider();
            Console.WriteLine("  Top Criminals by Crime Count:");
            foreach (var cr in _store.Criminals.Values.OrderByDescending(c => c.CrimeHistory.Count).Take(5))
                Console.WriteLine($"    [{cr.Id}] {cr.Name} - {cr.CrimeHistory.Count} crimes | Danger: {cr.DangerLevel}");
        }
    }
}