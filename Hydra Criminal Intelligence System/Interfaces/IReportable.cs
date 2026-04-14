// ============================================================
// Interfaces/IReportable.cs
// Abstraction: All reportable entities must implement this
// ============================================================
namespace CriminalRecordMS.Interfaces
{
    public interface IReportable
    {
        string GenerateReport();
        void PrintReport();
    }

    public interface ISearchable<T>
    {
        T? FindById(string id);
        List<T> FindByName(string name);
    }

    public interface IFileStorable
    {
        string Serialize();
    }
}