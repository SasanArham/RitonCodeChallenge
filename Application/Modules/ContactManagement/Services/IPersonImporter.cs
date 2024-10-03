namespace Application.Modules.ContactManagement.Services
{
    public interface IPersonImporter
    {
        Task ImportFromExcelAsync(string filePath);
    }
}
