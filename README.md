# 🚔 Criminal Record Management System (CRMS)
### C# Console Application — Final Year Project | BSCS Spring 2026 | KICSIT, Rawalpindi

---

<div align="center">
  <img src="https://img.shields.io/badge/Platform-.NET%208-512BD4?style=for-the-badge&logo=dotnet" />
  <img src="https://img.shields.io/badge/Language-C%23%2012-239120?style=for-the-badge&logo=csharp" />
  <img src="https://img.shields.io/badge/Architecture-Layered%20%2B%20Service%20Pattern-0078D4?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Storage-JSON%20File%20Persistence-F7A800?style=for-the-badge" />
  <img src="https://img.shields.io/badge/OOP-Encapsulation%20%7C%20Inheritance%20%7C%20Polymorphism%20%7C%20Abstraction-C0392B?style=for-the-badge" />
</div>
---

## 📋 Table of Contents

1. [Project Overview](#1-project-overview)
2. [System Architecture](#2-system-architecture)
3. [Folder & File Structure](#3-folder--file-structure)
4. [OOP Design — Deep Analysis](#4-oop-design--deep-analysis)
5. [Class Reference — All 18 Files](#5-class-reference--all-18-files)
6. [Features & Modules](#6-features--modules)
7. [Authentication System](#7-authentication-system)
8. [Data Persistence & DTO Pattern](#8-data-persistence--dto-pattern)
9. [Sample Pre-Seeded Data](#9-sample-pre-seeded-data)
10. [Console UI & Menu Navigation](#10-console-ui--menu-navigation)
11. [Exception Handling Strategy](#11-exception-handling-strategy)
12. [Getting Started](#12-getting-started)
13. [Default Credentials](#13-default-credentials)
14. [How to Use — Step by Step](#14-how-to-use--step-by-step)
15. [Design Decisions & Tradeoffs](#15-design-decisions--tradeoffs)
16. [Known Limitations & Future Scope](#16-known-limitations--future-scope)
17. [Academic Compliance](#17-academic-compliance)

---

## 1. Project Overview

The **Criminal Record Management System (CRMS)** is a fully functional, menu-driven console application built in **C# (.NET 8)** that simulates the internal record-keeping system of a police department. It was designed as a final-year undergraduate project to demonstrate mastery of **Object-Oriented Programming** concepts in a real-world, domain-driven context.

The system allows authenticated users (Admins and Officers) to manage:

- **Criminal profiles** with full crime history tracking
- **Case records** linking criminals, officers, victims, and evidence
- **Officer accounts** with role-based access
- **Three types of evidence** (Document, Weapon, Digital) using inheritance
- **Reports** including criminal history, case summaries, officer performance, and most-wanted lists
- **Search and filtering** by name, CNIC, crime type, and case status

All data is **persisted to JSON files** between sessions with no external database dependency.

---

## 2. System Architecture

CRMS follows a **4-layer architecture** that cleanly separates concerns:

```
┌────────────────────────────────────────────────────────────┐
│                   PRESENTATION LAYER                        │
│  MenuController.cs  —  All console screens and user flows  │
├────────────────────────────────────────────────────────────┤
│                    SERVICES LAYER                           │
│  AuthService  │  CriminalService  │  CaseService           │
│  OfficerService  │  EvidenceService  │  ReportService       │
│  DataStore  (in-memory central repository)                 │
├────────────────────────────────────────────────────────────┤
│                     MODELS LAYER                            │
│  Person → Criminal / Officer / Victim                      │
│  Evidence → DocumentEvidence / WeaponEvidence / Digital    │
│  Case  │  CrimeRecord  │  User  │  Enums                   │
├────────────────────────────────────────────────────────────┤
│                   UTILITIES LAYER                           │
│  FileHandler (JSON I/O)  │  PasswordHelper (SHA-256)       │
│  IdGenerator  │  ConsoleUI  │  Validator                   │
└────────────────────────────────────────────────────────────┘
                            ↕ JSON files
                    ┌───────────────────┐
                    │   Data/ directory  │
                    │  criminals.json    │
                    │  officers.json     │
                    │  cases.json        │
                    │  users.json        │
                    │  victims.json      │
                    └───────────────────┘
```

### Data Flow

```
Application Start
     ↓
FileHandler.Load() → DataStore (in-memory Dictionary<string, T>)
     ↓
User Login via AuthService (SHA-256 password verification)
     ↓
MenuController renders screen → user input
     ↓
Service (CriminalService / CaseService / ...) executes logic
     ↓
DataStore updated → FileHandler.Save() persists JSON
     ↓
Loop back to menu
```

---

## 3. Folder & File Structure

```
CriminalRecordMS/
│
├── Program.cs                          ← Entry point; startup, login, main loop
│
├── Interfaces/
│   └── IReportable.cs                  ← IReportable, ISearchable<T>, IFileStorable
│
├── Models/
│   ├── Enums.cs                        ← CaseStatus, CrimeSeverity, OfficerRank,
│   │                                      EvidenceType, UserRole
│   ├── Person.cs                       ← Abstract base class (Inheritance root)
│   ├── Criminal.cs                     ← Extends Person; adds crime history, danger level
│   ├── Officer.cs                      ← Extends Person; adds badge, rank, case list
│   ├── Victim.cs                       ← Extends Person; adds case link, statement
│   ├── CrimeRecord.cs                  ← Single crime entry in criminal's history
│   ├── Evidence.cs                     ← Abstract Evidence + 3 subtypes
│   ├── Case.cs                         ← Full case record; implements IReportable
│   └── User.cs                         ← Authentication model (Admin / Officer)
│
├── Services/
│   ├── DataStore.cs                    ← Central in-memory repository + DTO classes + seeder
│   ├── AuthService.cs                  ← Login, logout, password change
│   ├── CriminalService.cs              ← Criminal CRUD + search + most-wanted logic
│   ├── Services.cs                     ← CaseService, OfficerService,
│   │                                      EvidenceService, ReportService
│   └── MenuController.cs               ← All console menus (800+ lines)
│
├── Utilities/
│   ├── Helpers.cs                      ← PasswordHelper, IdGenerator,
│   │                                      ConsoleUI, Validator
│   └── FileHandler.cs                  ← JSON serialization/deserialization + all DTOs
│
└── Data/                               ← Auto-created at runtime
    ├── criminals.json
    ├── officers.json
    ├── cases.json
    ├── users.json
    └── victims.json
```

### Purpose of Each Folder

| Folder | Responsibility |
|--------|---------------|
| `Models/` | Pure domain entities — no business logic, no I/O. Just data and validation. |
| `Interfaces/` | Abstraction contracts that decouple components and enable polymorphism. |
| `Services/` | All business logic lives here. Services operate on DataStore and never touch files directly. |
| `Utilities/` | Stateless, reusable helpers. No business logic. Cross-cutting concerns only. |
| `Data/` | Runtime JSON files. Never manually edited. Managed exclusively by FileHandler. |

---

## 4. OOP Design — Deep Analysis

This section explains exactly **where and how** each OOP pillar is implemented.

---

### 4.1 Encapsulation

Every model class uses **private backing fields** with **validated public properties**. Business rules are enforced at the property level, not scattered in service code.

**Examples from `Person.cs`:**

```csharp
private string _name;

public string Name
{
    get => _name;
    set => _name = !string.IsNullOrWhiteSpace(value) ? value
        : throw new ArgumentException("Name cannot be empty.");
}

private DateTime _dateOfBirth;

public DateTime DateOfBirth
{
    get => _dateOfBirth;
    set
    {
        if (value > DateTime.Now)
            throw new ArgumentException("Date of birth cannot be in the future.");
        _dateOfBirth = value;
    }
}
```

**Examples from `Criminal.cs`:**

```csharp
// CrimeHistory is fully private — external code cannot add crimes directly
private List<CrimeRecord> _crimeHistory;

// Read-only access — enforces controlled modification
public List<CrimeRecord> CrimeHistory
{
    get => _crimeHistory;
    private set => _crimeHistory = value ?? new List<CrimeRecord>();
}

// Only way to add a crime — automatically triggers danger level recalculation
public void AddCrime(CrimeRecord crime)
{
    if (crime == null) throw new ArgumentNullException(nameof(crime));
    _crimeHistory.Add(crime);
    UpdateDangerLevel();         // private method — encapsulated logic
    UpdateMostWantedStatus();    // private method — encapsulated logic
}
```

**Why this matters:** The `DangerLevel` and `IsMostWanted` properties are always consistent with the crime list because they can only change through `AddCrime()` or `RemoveCrime()`. External code cannot put Criminal into an inconsistent state.

---

### 4.2 Inheritance

Two complete inheritance hierarchies are implemented:

#### Hierarchy 1: Person

```
Person  (abstract)
├── Criminal
│   Extra: CrimeHistory (List<CrimeRecord>), DangerLevel, IsMostWanted,
│          IsArrested, ArrestDate, Nationality, PhysicalDescription
├── Officer
│   Extra: BadgeNumber, Rank (OfficerRank enum), Department,
│          AssignedCaseIds, Username, PasswordHash
└── Victim
    Extra: CaseId, InjuryDescription, IsWitness, StatementSummary
```

**What is shared via Person:**
- `Id`, `Name`, `Cnic`, `Address`, `PhoneNumber`, `DateOfBirth` — all with validated setters
- `Age` — computed property (`DateTime.Now.Year - DateOfBirth.Year`)
- `ToString()` — base implementation (`[Id] Name | CNIC: Cnic`)
- `GenerateReport()` — abstract (each subclass MUST implement it)
- `PrintReport()` — virtual (calls `GenerateReport()`, can be overridden)

#### Hierarchy 2: Evidence

```
Evidence  (abstract)
├── DocumentEvidence
│   Extra: DocumentTitle, DocumentType, IsForensicallyCertified
├── WeaponEvidence
│   Extra: WeaponType, SerialNumber, IsRegistered, Manufacturer
└── DigitalEvidence
    Extra: DeviceType, HashValue, FileFormat, IsEncrypted, FileSizeKb
```

**What is shared via Evidence:**
- `EvidenceId`, `CaseId`, `Description`, `CollectedOn`, `CollectedBy`, `Type`, `StorageLocation`
- `GetTypeSpecificDetails()` — abstract (each subtype overrides to display its own fields)
- `GenerateReport()` — virtual base implementation + each subtype can extend it
- `PrintReport()` — concrete implementation calling `GenerateReport()`
- `ToString()` — `[EvidenceId] Type - Description`

---

### 4.3 Polymorphism

Polymorphism is used in **three distinct ways** throughout CRMS:

#### Runtime Polymorphism — Report Generation

```csharp
// ReportService treats all entities via base type
public void CriminalHistoryReport(string criminalId)
{
    Criminal cr = _store.Criminals[criminalId];
    cr.PrintReport();  // Calls Criminal.GenerateReport() at runtime
}

// Same method name, completely different output for each type:
Person p1 = new Criminal(...);   p1.PrintReport(); // → Criminal report format
Person p2 = new Officer(...);    p2.PrintReport(); // → Officer report format
Person p3 = new Victim(...);     p3.PrintReport(); // → Victim report format
```

#### Polymorphism — Evidence Display Loop

```csharp
// Case stores List<Evidence> (base type)
List<Evidence> evidenceList = case.EvidenceList;

// Iterating calls each subtype's overridden method automatically
foreach (var ev in evidenceList)
    ev.PrintReport();  // Document? → DocumentEvidence.GenerateReport()
                       // Weapon?   → WeaponEvidence.GenerateReport()
                       // Digital?  → DigitalEvidence.GenerateReport()
```

#### Polymorphism — DTO Deserialization (Type Discrimination)

```csharp
// EvidenceDTO.ToEvidence() uses switch expression — type-based polymorphic construction
public Evidence ToEvidence()
{
    return Type switch
    {
        EvidenceType.Document => new DocumentEvidence(EvidenceId, CaseId, Description,
            CollectedOn, CollectedBy, DocumentTitle, DocumentType)
            { IsForensicallyCertified = IsForensicallyCertified },
        EvidenceType.Weapon   => new WeaponEvidence(EvidenceId, CaseId, Description,
            CollectedOn, CollectedBy, WeaponType, SerialNumber, IsRegistered)
            { Manufacturer = Manufacturer },
        EvidenceType.Digital  => new DigitalEvidence(EvidenceId, CaseId, Description,
            CollectedOn, CollectedBy, DeviceType, HashValue, FileFormat)
            { IsEncrypted = IsEncrypted, FileSizeKb = FileSizeKb },
        _ => throw new InvalidOperationException("Unknown evidence type")
    };
}
```

---

### 4.4 Abstraction

#### Interfaces (`Interfaces/IReportable.cs`)

```csharp
// Contract: anything that can generate a report
public interface IReportable
{
    string GenerateReport();
    void PrintReport();
}

// Contract: anything that can be searched
public interface ISearchable<T>
{
    T? FindById(string id);
    List<T> FindByName(string name);
}

// Contract: anything that can serialize itself
public interface IFileStorable
{
    string Serialize();
}
```

**Implemented by:**
- `Person` (abstract) → `IReportable` — forces Criminal, Officer, Victim to implement report generation
- `Case` → `IReportable` — case reports work through same interface
- `Evidence` (abstract) → `IReportable` — all evidence subtypes are reportable

#### Abstract Classes

- `Person` — cannot be instantiated; defines the contract for all people in the system
- `Evidence` — cannot be instantiated; `GetTypeSpecificDetails()` is abstract, forcing each subtype to declare its own field summary

This means a developer **cannot accidentally create a generic "Person"** or a generic "Evidence" — the type system enforces correct usage at compile time.

---

## 5. Class Reference — All 18 Files

### `Models/Enums.cs`

| Enum | Values | Used In |
|------|--------|---------|
| `CaseStatus` | Open, UnderInvestigation, Closed | Case, CaseService, MenuController |
| `CrimeSeverity` | Minor, Moderate, Major, Capital | CrimeRecord, Case, CriminalService |
| `OfficerRank` | Constable, SubInspector, Inspector, SeniorInspector, DSP | Officer |
| `EvidenceType` | Document, Weapon, Digital | Evidence, EvidenceDTO |
| `UserRole` | Admin, Officer | User, AuthService |

---

### `Models/Person.cs`

**Type:** Abstract class implementing `IReportable`

| Member | Type | Notes |
|--------|------|-------|
| `Id` | `string` | Protected setter; validated non-empty |
| `Name` | `string` | Validated non-empty |
| `Cnic` | `string` | Min 13 characters |
| `Address` | `string` | Nullable-safe |
| `PhoneNumber` | `string` | Nullable-safe |
| `DateOfBirth` | `DateTime` | Cannot be in future |
| `Age` | `int` (computed) | `DateTime.Now.Year - DateOfBirth.Year` |
| `GenerateReport()` | `abstract string` | Must be overridden |
| `PrintReport()` | `virtual void` | Calls `GenerateReport()` |

---

### `Models/Criminal.cs`

**Type:** Concrete class extending `Person`

| Member | Type | Notes |
|--------|------|-------|
| `CrimeHistory` | `List<CrimeRecord>` | Private setter; add only via `AddCrime()` |
| `DangerLevel` | `string` | Auto-computed: Low/Medium/High/Extreme |
| `IsMostWanted` | `bool` | Auto-computed: 3+ crimes OR any Capital crime |
| `IsArrested` | `bool` | Manually set via `UpdateCriminal()` |
| `ArrestDate` | `DateTime?` | Nullable; set when arrested |
| `Nationality` | `string` | — |
| `PhysicalDescription` | `string` | Free text description for identification |
| `AddCrime(CrimeRecord)` | `void` | Validates, adds, recomputes DangerLevel + MostWanted |
| `RemoveCrime(string)` | `void` | Finds by CrimeId, removes, recomputes |
| `UpdateDangerLevel()` | `private void` | Capital ≥2 or total ≥5 → Extreme; Capital=1 or Major≥2 → High; etc. |
| `GenerateReport()` | `override string` | Full criminal profile with crime history list |

**Danger Level Algorithm:**
```
Capital crimes ≥ 2  OR  Total crimes ≥ 5  →  "Extreme"
Capital crimes = 1  OR  Major crimes ≥ 2  →  "High"
Major crimes = 1    OR  Total crimes ≥ 3  →  "Medium"
Otherwise                                 →  "Low"
```

**Most Wanted Algorithm:**
```
IsMostWanted = true  when  CrimeHistory.Count ≥ 3
                     OR    any CrimeRecord.Severity == Capital
```

---

### `Models/Officer.cs`

**Type:** Concrete class extending `Person`

| Member | Type | Notes |
|--------|------|-------|
| `BadgeNumber` | `string` | Validated non-empty |
| `Rank` | `OfficerRank` (enum) | — |
| `Department` | `string` | — |
| `JoinDate` | `DateTime` | Set to `DateTime.Now` on creation |
| `IsActive` | `bool` | Soft-delete flag |
| `Username` | `string` | Used for login |
| `PasswordHash` | `string` | SHA-256 hash stored, never plaintext |
| `AssignedCaseIds` | `List<string>` | Private setter; managed via `AssignCase()` / `RemoveCase()` |
| `CasesHandled` | `int` (computed) | `AssignedCaseIds.Count` |
| `AssignCase(string)` | `void` | Deduplication check before adding |
| `RemoveCase(string)` | `void` | — |
| `GenerateReport()` | `override string` | Officer profile + assigned case ID list |

---

### `Models/Victim.cs`

**Type:** Concrete class extending `Person`

| Member | Type | Notes |
|--------|------|-------|
| `CaseId` | `string` | Links victim to a case |
| `InjuryDescription` | `string` | — |
| `IsWitness` | `bool` | Can also serve as witness |
| `StatementSummary` | `string` | Summary of witness/victim statement |
| `GenerateReport()` | `override string` | Victim profile + case link |

---

### `Models/CrimeRecord.cs`

**Type:** Plain class (no inheritance); represents one crime entry

| Member | Type | Notes |
|--------|------|-------|
| `CrimeId` | `string` | Auto-generated (`CRC-6xx`) |
| `CrimeType` | `string` | e.g. "Murder", "Robbery", "Drug Trafficking" |
| `Description` | `string` | Detailed description |
| `DateCommitted` | `DateTime` | — |
| `Severity` | `CrimeSeverity` | Drives DangerLevel computation |
| `CaseId` | `string` | Links crime to a case |

---

### `Models/Evidence.cs`

**Type:** Abstract class implementing `IReportable`; contains 3 concrete subtypes

#### `Evidence` (abstract base)

| Member | Type | Notes |
|--------|------|-------|
| `EvidenceId` | `string` | Auto-generated (`EVD-4xx`) |
| `CaseId` | `string` | — |
| `Description` | `string` | — |
| `CollectedOn` | `DateTime` | — |
| `CollectedBy` | `string` | Officer ID |
| `Type` | `EvidenceType` | Set by each subtype constructor |
| `StorageLocation` | `string` | Default: "Evidence Room A" |
| `GetTypeSpecificDetails()` | `abstract string` | Must override in each subtype |
| `GenerateReport()` | `virtual string` | Base report; subtypes can extend |
| `PrintReport()` | `void` | Calls `GenerateReport()` |

#### `DocumentEvidence : Evidence`

| Extra Field | Type | Notes |
|-------------|------|-------|
| `DocumentTitle` | `string` | e.g. "Ransom Note", "Contract" |
| `DocumentType` | `string` | e.g. "Letter", "Financial Record" |
| `IsForensicallyCertified` | `bool` | Chain of custody indicator |

#### `WeaponEvidence : Evidence`

| Extra Field | Type | Notes |
|-------------|------|-------|
| `WeaponType` | `string` | "Firearm", "Knife", "Blunt", etc. |
| `SerialNumber` | `string` | For firearms registration lookup |
| `IsRegistered` | `bool` | Legally registered or not |
| `Manufacturer` | `string` | Default: "Unknown" |

#### `DigitalEvidence : Evidence`

| Extra Field | Type | Notes |
|-------------|------|-------|
| `DeviceType` | `string` | "Laptop", "Phone", "USB", "CCTV Hard Drive" |
| `HashValue` | `string` | MD5 or SHA-256 for integrity verification |
| `FileFormat` | `string` | "MP4", "PDF", "Mixed", etc. |
| `IsEncrypted` | `bool` | Indicates forensic decryption needed |
| `FileSizeKb` | `long` | Size in kilobytes |

---

### `Models/Case.cs`

**Type:** Concrete class implementing `IReportable`

| Member | Type | Notes |
|--------|------|-------|
| `CaseId` | `string` | Auto-generated (`CSE-3xx`) |
| `Title` | `string` | Validated non-empty |
| `Description` | `string` | — |
| `Status` | `CaseStatus` | Open → UnderInvestigation → Closed |
| `DateOpened` | `DateTime` | Set on creation |
| `DateClosed` | `DateTime?` | Set when `CloseCase()` is called |
| `Location` | `string` | — |
| `Severity` | `CrimeSeverity` | — |
| `PrimaryOfficerId` | `string` | Lead officer |
| `CriminalIds` | `List<string>` | Cross-reference to Criminal records |
| `OfficerIds` | `List<string>` | Cross-reference to Officer records |
| `EvidenceList` | `List<Evidence>` | Embedded evidence (stored in case JSON) |
| `VictimIds` | `List<string>` | Cross-reference to Victim records |
| `AddCriminal(string)` | `void` | Deduplication check |
| `AssignOfficer(string)` | `void` | Deduplication check |
| `AddEvidence(Evidence)` | `void` | Null check before adding |
| `CloseCase()` | `void` | Sets Status=Closed and stamps DateClosed |
| `GenerateReport()` | `override string` | Full case detail with evidence list |

---

### `Models/User.cs`

**Type:** Plain class for authentication

| Member | Type | Notes |
|--------|------|-------|
| `UserId` | `string` | `USR-xxx` |
| `Username` | `string` | Login identifier |
| `PasswordHash` | `string` | SHA-256 (never plaintext) |
| `Role` | `UserRole` | Admin or Officer |
| `LinkedOfficerId` | `string` | Empty for Admin; Officer ID for Officer users |
| `IsActive` | `bool` | Deactivation without deletion |
| `LastLogin` | `DateTime` | Updated on each successful login |

---

### `Services/DataStore.cs`

**Type:** Central in-memory repository (used as a singleton by all services)

| Member | Type | Notes |
|--------|------|-------|
| `Criminals` | `Dictionary<string, Criminal>` | O(1) lookup by ID |
| `Officers` | `Dictionary<string, Officer>` | O(1) lookup by ID |
| `Cases` | `Dictionary<string, Case>` | O(1) lookup by ID |
| `Users` | `Dictionary<string, User>` | O(1) lookup by ID |
| `Victims` | `Dictionary<string, Victim>` | O(1) lookup by ID |
| `CurrentUser` | `User?` | Session state |
| `IsLoggedIn` | `bool` (computed) | `CurrentUser != null` |
| `LoadAll()` | `void` | Deserializes all JSON files into collections |
| `SaveAll()` | `void` | Serializes all collections to JSON files |
| `SeedDefaultUsers()` | `void` | Creates admin account if no Admin exists |
| `SeedSampleData()` | `void` | Loads 3 officers, 3 criminals, 4 cases with full evidence |

Also contains all **DTO classes** used for JSON serialization:
`CrimeRecordDTO`, `CriminalDTO`, `OfficerDTO`, `EvidenceDTO`, `CaseDTO`, `VictimDTO`

---

### `Services/AuthService.cs`

| Method | Signature | Notes |
|--------|-----------|-------|
| `Login` | `bool Login(string username, string password)` | SHA-256 verify; sets `CurrentUser` |
| `Logout` | `void Logout()` | Clears `CurrentUser` |
| `IsAdmin` | `bool IsAdmin()` | Checks current role |
| `HasAccess` | `bool HasAccess(UserRole required)` | Admin bypasses all restrictions |
| `ChangePassword` | `void ChangePassword(string old, string new)` | Verifies old; min 6 chars new |

---

### `Services/CriminalService.cs`

| Method | Notes |
|--------|-------|
| `AddCriminal(...)` | Validates unique CNIC; auto-generates ID |
| `UpdateCriminal(...)` | Null-safe partial update (only non-null params applied) |
| `DeleteCriminal(string)` | Admin only (enforced in MenuController) |
| `GetAll()` | Returns all criminals as `List<Criminal>` |
| `GetById(string)` | Throws if not found |
| `SearchByName(string)` | Case-insensitive partial match |
| `SearchByCnic(string)` | Exact match; returns `Criminal?` |
| `AddCrimeToCriminal(...)` | Creates `CrimeRecord`, calls `AddCrime()`, saves |
| `FilterByCrimeType(string)` | Searches CrimeHistory of all criminals |
| `GetMostWanted()` | `IsMostWanted == true && IsArrested == false`, sorted by crime count |

---

### `Services/Services.cs` — Contains 4 service classes:

#### `CaseService`
| Method | Notes |
|--------|-------|
| `CreateCase(...)` | Auto-generates CaseId |
| `AssignOfficer(caseId, officerId)` | Bidirectional: updates Case.OfficerIds AND Officer.AssignedCaseIds |
| `RemoveOfficer(...)` | Bidirectional removal |
| `LinkCriminal(caseId, criminalId)` | Validates criminal exists before linking |
| `UpdateStatus(caseId, status)` | Calls `CloseCase()` automatically if Closed |
| `GetByStatus(CaseStatus)` | Filter cases by status |
| `GetByOfficer(officerId)` | All cases assigned to a specific officer |

#### `OfficerService`
| Method | Notes |
|--------|-------|
| `AddOfficer(...)` | Creates Officer + auto-creates User account |
| `GetOfficerCases(officerId)` | Resolves AssignedCaseIds to actual Case objects |
| `DeactivateOfficer(string)` | Soft-delete: sets IsActive=false |

#### `EvidenceService`
| Method | Notes |
|--------|-------|
| `AddDocumentEvidence(...)` | Validates case exists; auto-generates EvidenceId |
| `AddWeaponEvidence(...)` | Same pattern |
| `AddDigitalEvidence(...)` | Same pattern |
| `GetEvidenceForCase(string)` | Returns `List<Evidence>` from the case |

#### `ReportService`
| Method | Notes |
|--------|-------|
| `CriminalHistoryReport(id)` | Calls `cr.PrintReport()` — polymorphic dispatch |
| `CaseSummaryReport(id)` | Calls `cs.PrintReport()` — polymorphic dispatch |
| `OfficerPerformanceReport(id)` | Officer profile + closure rate metrics |
| `SystemSummaryReport()` | Aggregate statistics across all entities |

---

### `Utilities/Helpers.cs` — 4 static classes:

#### `PasswordHelper`
```csharp
Hash(string password)    // SHA-256 with "CRMS_SALT_2024" salt → hex string
Verify(string, string)   // Re-hash and compare
```

#### `IdGenerator`
```
NewCriminalId()  →  "CRM-1xx"  (starts at 100, increments)
NewOfficerId()   →  "OFC-2xx"  (starts at 200)
NewCaseId()      →  "CSE-3xx"  (starts at 300)
NewEvidenceId()  →  "EVD-4xx"  (starts at 400)
NewVictimId()    →  "VCT-5xx"  (starts at 500)
NewCrimeId()     →  "CRC-6xx"  (starts at 600)
SetCounters()    →  Restore counters from loaded data
```

#### `ConsoleUI`
```
PrintHeader(string)    // Cyan box around title
PrintSuccess(string)   // Green ✔ prefix
PrintError(string)     // Red ✘ ERROR: prefix
PrintWarning(string)   // Yellow ⚠ prefix
PrintInfo(string)      // White ℹ prefix
PrintDivider()         // DarkGray ── line
PromptInput(string)    // Yellow » label: prompt
PromptPassword(string) // Masked input with * characters
PressAnyKey()          // "Press any key to continue..."
Confirm(string)        // (y/n) → bool
PrintMenuItem(int, string)  // [N] label format
```

#### `Validator`
```
IsValidCnic(string)           // length ≥ 13
IsValidPhone(string)          // length ≥ 10
TryParseDate(string, out DateTime)   // "yyyy-MM-dd" format
TryParseEnum<T>(string, out T)       // Case-insensitive enum parse
```

---

### `Utilities/FileHandler.cs`

**Type:** Static class; all file I/O in one place

| Method | Notes |
|--------|-------|
| `Save<T>(fileName, List<T>)` | JSON serialize with indentation + enum-as-string |
| `Load<T>(fileName)` | Deserialize; returns empty list if file missing |
| `SaveCases(List<Case>)` | Converts to `List<CaseDTO>` (handles polymorphic Evidence) |
| `LoadCases()` | Deserializes `List<CaseDTO>`, calls `ToCase()` on each |

**JSON Options used:**
```csharp
new JsonSerializerOptions
{
    WriteIndented = true,
    Converters = { new JsonStringEnumConverter() }  // Enums as strings, not numbers
}
```

---

### `Services/MenuController.cs`

The largest file (~900 lines). Handles all console interaction.

| Menu Screen | Method | Key Operations |
|-------------|--------|----------------|
| Welcome | `ShowWelcome()` | ASCII art banner display |
| Login | `ShowLoginScreen()` | 3-attempt limit; calls AuthService |
| Main | `ShowMainMenu()` | Role-aware menu items (Admin sees item 7) |
| Criminal | `CriminalMenu()` | 7 sub-options |
| Case | `CaseMenu()` | 7 sub-options |
| Officer | `OfficerMenu()` | 5 sub-options |
| Evidence | `EvidenceMenu()` | 4 sub-options |
| Reports | `ReportsMenu()` | 5 sub-options |
| Search | `SearchMenu()` | 4 sub-options |
| User Admin | `UserAdminMenu()` | Admin-only; 4 sub-options |

Every menu method wraps its switch block in `try/catch (Exception ex)` → `PrintError()` → `PressAnyKey()`, ensuring no crash can propagate to the user.

---

## 6. Features & Modules

### Module 1: Authentication
- SHA-256 password hashing with application-specific salt
- Role-based: Admin has full access; Officers have restricted access
- 3 failed attempts → session blocked (no account lockout, just exit)
- Masked password entry (asterisks)
- `LastLogin` timestamp recorded per session
- Password change from within the application

### Module 2: Criminal Management
- Add criminal with full demographic profile
- Update any subset of fields (null-safe partial update pattern)
- Delete criminal (Admin-only; requires confirmation)
- View all criminals in color-coded table (red = most wanted)
- View detailed profile with complete crime history
- Search by name (partial, case-insensitive)
- Search by CNIC (exact match)
- Add crimes to criminal's history (linked to case ID + severity)
- Auto-computed DangerLevel and MostWanted flags
- Arrest status tracking with date

### Module 3: Case Management
- Create cases with title, description, location, severity
- Assign / remove officers (bidirectional — updates Officer record too)
- Link criminals to cases
- Add victims
- Update status (Open → UnderInvestigation → Closed; closure auto-timestamps)
- View all cases in color-coded table (Green=Open, Yellow=Investigation, Gray=Closed)
- View full case detail report

### Module 4: Officer Management
- Add officers with login credentials (auto-creates User account)
- View all officers with rank, badge, department, active case count
- View full officer profile
- View all cases assigned to a specific officer
- Soft-deactivate officers (Admin only; data preserved)

### Module 5: Evidence Management
- Three evidence subtypes with type-specific input flows
- All evidence embedded within its Case record
- Storage location tracking
- Forensic certification flag (DocumentEvidence)
- Registration status and manufacturer (WeaponEvidence)
- Hash integrity value + encryption flag (DigitalEvidence)
- Polymorphic display — each type shows its own relevant fields

### Module 6: Reports System
- **Criminal History Report** — Full profile + crime list (polymorphic call)
- **Case Summary Report** — Full case + embedded evidence list
- **Officer Performance Report** — Profile + case closure rate metric
- **System Summary Report** — Aggregate counts, status breakdown, top 5 criminals by crime count
- **Most Wanted List** — Ranked list of active fugitives with physical descriptions

### Module 7: Search & Filtering
- Search criminal by name (partial, case-insensitive)
- Search criminal by CNIC (exact)
- Filter criminals by crime type keyword (searches all crime records)
- Filter cases by status

### Module 8: File Persistence
- Auto-saves after every modification
- Auto-loads on startup
- `/Data` directory auto-created if missing
- Sample data auto-seeded on first run
- Graceful degradation if files are missing or corrupt (logs error, continues with empty collections)

---

## 7. Authentication System

### Password Security

Passwords are **never stored in plaintext**. The SHA-256 algorithm is used with a fixed application salt:

```csharp
// PasswordHelper.Hash()
using var sha = SHA256.Create();
var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password + "CRMS_SALT_2024"));
return Convert.ToHexString(bytes);  // 64-character hex string
```

### Role Access Matrix

| Feature | Admin | Officer |
|---------|-------|---------|
| View criminals | ✅ | ✅ |
| Add criminal | ✅ | ✅ |
| Update criminal | ✅ | ✅ |
| Delete criminal | ✅ | ❌ |
| All case operations | ✅ | ✅ |
| Add/view officers | ✅ | ✅ |
| Add new officer | ✅ | ❌ |
| Deactivate officer | ✅ | ❌ |
| Evidence management | ✅ | ✅ |
| All reports | ✅ | ✅ |
| User administration | ✅ | ❌ |
| Seed sample data | ✅ | ❌ |

---

## 8. Data Persistence & DTO Pattern

### Why DTOs Are Needed

C# `System.Text.Json` cannot directly serialize/deserialize polymorphic class hierarchies. When `Case` stores `List<Evidence>` and that list contains `DocumentEvidence`, `WeaponEvidence`, and `DigitalEvidence` objects, standard serialization loses the subtype information.

### Solution: Data Transfer Objects

Each domain class has a flat DTO equivalent:

```
Criminal       →  CriminalDTO       (+ nested List<CrimeRecordDTO>)
Officer        →  OfficerDTO
Case           →  CaseDTO           (+ nested List<EvidenceDTO>)
Evidence       →  EvidenceDTO       (all fields from all subtypes, EvidenceType discriminator)
Victim         →  VictimDTO
```

### EvidenceDTO Discriminator Pattern

```csharp
// When saving:
new EvidenceDTO(evidenceObject)  // copies all fields; Type enum identifies the subtype

// When loading:
dto.ToEvidence()  // switch on Type → instantiate correct concrete class
```

This is equivalent to JSON "discriminated unions" used in TypeScript/Rust, implemented manually in C#.

---

## 9. Sample Pre-Seeded Data

On first run (empty database), the following data is automatically created:

### Officers

| ID | Name | Badge | Rank | Department | Username |
|----|------|-------|------|------------|---------|
| OFC-201 | Inspector Khalid Mahmood | B-2041 | Inspector | Criminal Investigation | khalid.mahmood |
| OFC-202 | Sub-Inspector Ayesha Noor | B-2042 | SubInspector | Cybercrime | ayesha.noor |
| OFC-203 | DSP Tariq Bashir | B-1001 | DSP | Homicide | tariq.bashir |

### Criminals

| ID | Name | Status | Danger | Crimes | Notable |
|----|------|--------|--------|--------|---------|
| CRM-101 | Ahmed Raza Khan | Fugitive | Extreme | 3 | Most Wanted (2 Capital crimes) |
| CRM-102 | Tariq Shah | Arrested 2023-04-15 | High | 1 | Drug trafficking |
| CRM-103 | Zara Malik | At Large | Low | 1 | Cybercrime / fraud |

### Cases

| ID | Title | Status | Severity | Criminals | Evidence |
|----|-------|--------|----------|-----------|---------|
| CSE-301 | HBL Bank Robbery 2021 | UnderInvestigation | Capital | CRM-101 | Glock firearm + CCTV footage |
| CSE-302 | Raza Kidnapping Case | Open | Major | CRM-101 | Forensically certified ransom note |
| CSE-303 | Narcotics Operation Sweep | Closed | Major | CRM-102 | Drug distribution ledger |
| CSE-304 | Online Banking Fraud | UnderInvestigation | Moderate | CRM-103 | Encrypted laptop with phishing toolkit |

---

## 10. Console UI & Menu Navigation

### Visual Design System

The console UI uses `ConsoleColor` to create a visual hierarchy:

| Element | Color | Format |
|---------|-------|--------|
| Section headers | Cyan | `╔══ TITLE ══╗` |
| Success messages | Green | `✔ Message` |
| Error messages | Red | `✘ ERROR: Message` |
| Warnings | Yellow | `⚠ Message` |
| Info | White | `ℹ Message` |
| Dividers | DarkGray | `────────────` |
| Most wanted rows | Red | Highlighted row |
| Open cases | Green | Highlighted row |
| Under investigation | Yellow | Highlighted row |
| Closed cases | DarkGray | Dimmed row |
| Input prompts | Yellow | `» Label: _` |

### Menu Hierarchy

```
Main Menu
├── [1] Criminal Management
│   ├── [1] Add New Criminal
│   ├── [2] View All Criminals
│   ├── [3] View Criminal Profile
│   ├── [4] Update Criminal Details
│   ├── [5] Mark as Arrested
│   ├── [6] Add Crime to Criminal
│   └── [7] Delete Criminal (Admin only)
│
├── [2] Case Management
│   ├── [1] Create New Case
│   ├── [2] View All Cases
│   ├── [3] View Case Details
│   ├── [4] Update Case Status
│   ├── [5] Assign Officer to Case
│   ├── [6] Remove Officer from Case
│   └── [7] Link Criminal to Case
│
├── [3] Officer Management
│   ├── [1] Add New Officer (Admin only)
│   ├── [2] View All Officers
│   ├── [3] View Officer Profile
│   ├── [4] View Officer's Assigned Cases
│   └── [5] Deactivate Officer (Admin only)
│
├── [4] Evidence Management
│   ├── [1] Add Document Evidence
│   ├── [2] Add Weapon Evidence
│   ├── [3] Add Digital Evidence
│   └── [4] View Evidence for a Case
│
├── [5] Reports
│   ├── [1] Criminal History Report
│   ├── [2] Case Summary Report
│   ├── [3] Officer Performance Report
│   ├── [4] System Summary Report
│   └── [5] Most Wanted List
│
├── [6] Search & Filter
│   ├── [1] Search Criminal by Name
│   ├── [2] Search Criminal by CNIC
│   ├── [3] Filter Criminals by Crime Type
│   └── [4] Filter Cases by Status
│
└── [7] User Administration (Admin only)
    ├── [1] View All Users
    ├── [2] Deactivate User
    ├── [3] Change My Password
    └── [4] Seed Sample Data
```

---

## 11. Exception Handling Strategy

CRMS uses a **layered exception handling approach**:

### Layer 1: Model Validation (Property Setters)
```csharp
// Throws immediately if invalid data is set
public string Name
{
    set => _name = !string.IsNullOrWhiteSpace(value)
        ? value : throw new ArgumentException("Name cannot be empty.");
}
```

### Layer 2: Service Layer (Business Rules)
```csharp
// Throws descriptive business exceptions
public Criminal AddCriminal(...)
{
    if (_store.Criminals.Values.Any(c => c.Cnic == cnic))
        throw new Exception($"Criminal with CNIC {cnic} already exists.");
    // ...
}
```

### Layer 3: Menu Layer (Global Catch-All)
```csharp
// Every menu switch is wrapped in try/catch
try
{
    switch (choice) { /* ... */ }
}
catch (Exception ex)
{
    ConsoleUI.PrintError(ex.Message);  // Shows descriptive message
    ConsoleUI.PressAnyKey();           // Gives user time to read
    // Loop continues — app NEVER crashes
}
```

### Layer 4: File I/O (Graceful Degradation)
```csharp
public static List<T> Load<T>(string fileName)
{
    try
    {
        // ... deserialize
    }
    catch (Exception ex)
    {
        ConsoleUI.PrintError($"Failed to load {fileName}: {ex.Message}");
        return new List<T>();  // App continues with empty data
    }
}
```

### Layer 5: Input Validation (Before Entity Creation)
```csharp
// Validator.TryParseDate prevents exceptions from bad user input
if (!Validator.TryParseDate(dobStr, out DateTime dob))
    throw new Exception("Invalid date format. Use yyyy-MM-dd.");

// Validator.TryParseEnum prevents exceptions from bad enum input
if (!Validator.TryParseEnum<CrimeSeverity>(sevStr, out var severity))
    throw new Exception("Invalid severity. Use Minor/Moderate/Major/Capital.");
```

---

## 12. Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8) or later
- Visual Studio 2022, VS Code with C# Dev Kit, or any .NET-compatible IDE

### Option A — Visual Studio

1. Open Visual Studio 2022
2. Create new project → **Console App (.NET 8)**
3. Name it `CriminalRecordMS`
4. Delete the default `Program.cs` content
5. Create the folder structure as shown in section 3
6. Add each `.cs` file to its corresponding folder
7. Press **F5** to run

### Option B — .NET CLI

```bash
# 1. Create the project
dotnet new console -n CriminalRecordMS --framework net8.0
cd CriminalRecordMS

# 2. Create folders
mkdir Models Services Interfaces Utilities Data

# 3. Copy all .cs files into their respective folders
# (from the source zip)

# 4. Run
dotnet run
```

### Option C — From Source ZIP

```bash
# Extract CRMS_Source_Code.zip
unzip CRMS_Source_Code.zip

# The project is already structured — just run
cd CriminalRecordMS
dotnet run
```

### No NuGet Packages Required

All dependencies are part of the .NET 8 Standard Library:
- `System.Text.Json` — JSON serialization
- `System.Security.Cryptography` — SHA-256 hashing
- `System.Linq` — collection queries
- `System.IO` — file operations

---

## 13. Default Credentials

| Role | Username | Password | Notes |
|------|----------|----------|-------|
| **Admin** | `admin` | `admin123` | Full access to all features |
| Officer | `khalid.mahmood` | `officer123` | Criminal Investigation dept. |
| Officer | `ayesha.noor` | `officer123` | Cybercrime dept. |
| Officer | `tariq.bashir` | `officer123` | Homicide dept. (seeded only) |

> **Note:** Passwords are stored as SHA-256 hashes. Change the admin password via `User Administration → Change My Password` after first login.

---

## 14. How to Use — Step by Step

### First Run

1. Launch the application
2. The system detects no data files → auto-seeds sample data (3 officers, 3 criminals, 4 cases)
3. Login with `admin` / `admin123`
4. Explore the pre-seeded data via Reports → System Summary Report

### Adding a New Criminal

```
Main Menu → [1] Criminal Management → [1] Add New Criminal

» Full Name: Muhammad Ali Raza
» CNIC (13 digits): 3520399887766
» Address (or 'Unknown'): House 5, Gulberg III, Lahore
» Phone Number: 03001234567
» Date of Birth (yyyy-MM-dd): 1990-03-15
» Nationality: Pakistani
» Physical Description: 5'8", medium build, beard, glasses

✔  Criminal added with ID: CRM-102
```

### Creating a Case and Linking Evidence

```
[2] Case Management → [1] Create New Case
» Case Title: Gulberg Burglary 2024
» Description: Residential break-in; jewellery stolen
» Location: House 45, Gulberg II, Lahore
» Severity: Major
✔  Case created with ID: CSE-305

[2] Case Management → [5] Assign Officer to Case
» Case ID: CSE-305
» Officer ID: OFC-201
✔  Officer OFC-201 assigned to case CSE-305.

[4] Evidence Management → [1] Add Document Evidence
» Case ID: CSE-305
» Description: Pawn shop receipt for stolen jewellery
» Collected By (Officer ID): OFC-201
» Document Title: Pawn Receipt
» Document Type: Financial Record
✔  Document evidence EVD-406 added.
```

### Generating a Most Wanted Report

```
[5] Reports → [5] Most Wanted List

  ╔══════════════════════════════════════════════════════╗
  ║           MOST WANTED CRIMINALS                     ║
  ╚══════════════════════════════════════════════════════╝
  # 1. [CRM-101] Ahmed Raza Khan
       Danger: Extreme | Crimes: 3 | Nationality: Pakistani
       Desc  : 5'10", Athletic build, scar on left cheek, black hair
  ────────────────────────────────────────────────────────
```

---

## 15. Design Decisions & Tradeoffs

| Decision | Rationale | Tradeoff |
|----------|-----------|----------|
| **Dictionary<string, T> over List<T>** | O(1) lookup by ID vs O(n) linear scan | Slightly more memory; worth it for search performance |
| **JSON over SQLite/SQL** | No external library dependency; human-readable files | Not suitable for large datasets (>10,000 records) |
| **DTO pattern for serialization** | Handles polymorphic hierarchies cleanly | Extra code per class |
| **Static service class for FileHandler** | Stateless I/O utility needs no instance | Cannot mock for unit testing |
| **SHA-256 with hardcoded salt** | Simple, dependency-free security | Salt should be per-user in production |
| **Embedded evidence in Case JSON** | Keeps related data together; simpler loading | Evidence list could become large for complex cases |
| **MenuController as single class** | All UI in one file is easy to navigate | Large file (~900 lines); could be split in future |
| **Auto-save after every operation** | No data loss risk | Slight performance hit on rapid consecutive edits |

---

## 16. Known Limitations & Future Scope

### Current Limitations

- **No database backend** — JSON files are not suitable beyond ~10,000 records
- **Single-user session** — no concurrent multi-user support
- **No full audit trail** — changes are not logged with timestamps and user attribution
- **No report export** — reports display on console only (no PDF/CSV export)
- **CNIC uniqueness for criminals only** — no deduplication for officers/victims
- **Age calculation** — uses year difference only (not exact birthday-aware)
- **Password salt is shared** — production systems use per-user random salts

### Suggested Future Enhancements

- Migrate to **SQLite** (via `Microsoft.Data.Sqlite`) or **SQL Server** for scalable storage
- Add a **WinForms or WPF GUI** for a graphical interface
- Implement a **full audit log** (who changed what and when)
- Add **PDF report export** using `iTextSharp` or `QuestPDF`
- Add **photograph support** for criminal profiles (file path storage)
- Implement **per-user random salt** for stronger password security
- Add **case timeline** — chronological event log per case
- Implement **biometric linking** (fingerprint hash or face recognition ID)
- Add **network support** for multi-station police department use

---

## 17. Academic Compliance

This project was designed to satisfy the following academic requirements for BSCS final year at KICSIT, Rawalpindi:

| Requirement | Implementation |
|-------------|----------------|
| Encapsulation | Private fields + validated properties in all Model classes |
| Inheritance (Person hierarchy) | `Criminal`, `Officer`, `Victim` all extend `Person` |
| Inheritance (Evidence hierarchy) | `DocumentEvidence`, `WeaponEvidence`, `DigitalEvidence` extend `Evidence` |
| Polymorphism | `GenerateReport()` / `PrintReport()` overridden in all Person/Evidence subtypes; called via base reference |
| Abstraction | `IReportable`, `ISearchable<T>`, `IFileStorable` interfaces; `Person` and `Evidence` are abstract |
| Collections | `Dictionary<string,T>` for all primary stores; `List<T>` for sub-collections |
| Enums | `CaseStatus`, `CrimeSeverity`, `OfficerRank`, `EvidenceType`, `UserRole` |
| File Handling | JSON persistence via `System.Text.Json`; all data loaded/saved automatically |
| Exception Handling | 5-layer strategy (property, service, menu, file I/O, input validation) |
| Menu-driven UI | Hierarchical console menus with color-coded output |
| Modular structure | 5 folders, 18 files, each with single responsibility |
| Authentication | SHA-256 hashed passwords, role-based access control |
| Search & Filter | By name, CNIC, crime type, case status, most-wanted |
| Reports | 5 distinct report types including performance metrics |

---

<div align="center">

**Criminal Record Management System — CRMS v1.0**

*KICSIT | Department of Computer Science | BSCS Spring 2026*

*Built with C# (.NET 8) — Object-Oriented Programming Final Year Project*

---

*"Effective law enforcement begins with effective information management."*

</div>
