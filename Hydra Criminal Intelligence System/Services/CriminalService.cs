// ============================================================
// Services/CriminalService.cs
// Business logic for criminal management
// ============================================================
using CriminalRecordMS.Models;
using CriminalRecordMS.Utilities;

namespace CriminalRecordMS.Services
{
    public class CriminalService
    {
        private readonly DataStore _store;

        public CriminalService(DataStore store) => _store = store;

        // ── Add ─────────────────────────────────────────────────
        public Criminal AddCriminal(string name, string cnic, string address,
                                    string phone, DateTime dob, string nationality,
                                    string physicalDesc)
        {
            if (_store.Criminals.Values.Any(c => c.Cnic == cnic))
                throw new Exception($"Criminal with CNIC {cnic} already exists.");

            var id = IdGenerator.NewCriminalId();
            var criminal = new Criminal(id, name, cnic, address, phone, dob,
                                        nationality, physicalDesc);
            _store.Criminals[id] = criminal;
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Criminal added with ID: {id}");
            return criminal;
        }

        // ── Update ──────────────────────────────────────────────
        public void UpdateCriminal(string id, string? name, string? address,
                                   string? phone, string? physicalDesc,
                                   bool? isArrested, DateTime? arrestDate)
        {
            if (!_store.Criminals.TryGetValue(id, out var cr))
                throw new Exception($"Criminal {id} not found.");

            if (!string.IsNullOrWhiteSpace(name)) cr.Name = name;
            if (!string.IsNullOrWhiteSpace(address)) cr.Address = address;
            if (!string.IsNullOrWhiteSpace(phone)) cr.PhoneNumber = phone;
            if (!string.IsNullOrWhiteSpace(physicalDesc)) cr.PhysicalDescription = physicalDesc;
            if (isArrested.HasValue)
            {
                cr.IsArrested = isArrested.Value;
                if (isArrested.Value) cr.ArrestDate = arrestDate ?? DateTime.Now;
            }

            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Criminal {id} updated.");
        }

        // ── Delete ──────────────────────────────────────────────
        public void DeleteCriminal(string id)
        {
            if (!_store.Criminals.ContainsKey(id))
                throw new Exception($"Criminal {id} not found.");

            _store.Criminals.Remove(id);
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Criminal {id} deleted.");
        }

        // ── View All ────────────────────────────────────────────
        public List<Criminal> GetAll() => _store.Criminals.Values.ToList();

        public Criminal GetById(string id)
        {
            if (!_store.Criminals.TryGetValue(id, out var cr))
                throw new Exception($"Criminal {id} not found.");
            return cr;
        }

        // ── Search ──────────────────────────────────────────────
        public List<Criminal> SearchByName(string name)
            => _store.Criminals.Values
                .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();

        public Criminal? SearchByCnic(string cnic)
            => _store.Criminals.Values.FirstOrDefault(c => c.Cnic == cnic);

        // ── Add Crime to Criminal ───────────────────────────────
        public void AddCrimeToCriminal(string criminalId, string crimeType,
                                       string description, DateTime date,
                                       CrimeSeverity severity, string caseId)
        {
            var criminal = GetById(criminalId);
            var crimeId = IdGenerator.NewCrimeId();
            var record = new CrimeRecord(crimeId, crimeType, description, date, severity, caseId);
            criminal.AddCrime(record);
            _store.SaveAll();
            ConsoleUI.PrintSuccess($"Crime {crimeId} added to criminal {criminalId}.");
        }

        // ── Filter ──────────────────────────────────────────────
        public List<Criminal> FilterByCrimeType(string crimeType)
            => _store.Criminals.Values
                .Where(c => c.CrimeHistory.Any(cr =>
                    cr.CrimeType.Contains(crimeType, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        // ── Most Wanted ─────────────────────────────────────────
        public List<Criminal> GetMostWanted()
            => _store.Criminals.Values
                .Where(c => c.IsMostWanted && !c.IsArrested)
                .OrderByDescending(c => c.CrimeHistory.Count)
                .ToList();
    }
}