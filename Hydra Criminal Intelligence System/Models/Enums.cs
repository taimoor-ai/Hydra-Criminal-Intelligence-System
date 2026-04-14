// ============================================================
// Models/Enums.cs
// All enumerations used across the system
// ============================================================
namespace CriminalRecordMS.Models
{
    public enum CaseStatus
    {
        Open,
        UnderInvestigation,
        Closed
    }

    public enum CrimeSeverity
    {
        Minor,
        Moderate,
        Major,
        Capital
    }

    public enum OfficerRank
    {
        Constable,
        SubInspector,
        Inspector,
        SeniorInspector,
        DSP
    }

    public enum EvidenceType
    {
        Document,
        Weapon,
        Digital
    }

    public enum UserRole
    {
        Admin,
        Officer
    }
}