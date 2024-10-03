using Application.Base;
using ClosedXML.Excel;
using Domain.Modules.ContactManagement.People;
using Microsoft.Extensions.Logging;

namespace Application.Modules.ContactManagement.Services
{
    public class PersonImporter : IPersonImporter
    {
        private readonly IDatabaseContext _dbContext;
        private readonly ILogger<PersonImporter> _logger;

        public PersonImporter(IDatabaseContext dbContext
            , ILogger<PersonImporter> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ImportFromExcelAsync(string filePath)
        {
            var logSession = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation("{@LogSession}{@Message}{@FilePath}", logSession, "Received a person import from excel task", filePath);
                var workBook = new XLWorkbook(filePath);
                var workSheet = workBook.Worksheets.First();
                var range = workSheet.RangeUsed();
                var rowCount = range.RowCount();
                _logger.LogInformation("{@LogSession}{@RowCount}", logSession, rowCount);

                for (int i = 2; i < rowCount + 1; i++)
                {
                    var row = workSheet.Rows(i.ToString());
                    var cells = row.Cells();
                    var rowCells = cells.ToList();

                    var name = rowCells[0].Value.ToString().Trim();
                    var lastName = rowCells[1].Value.ToString().Trim();

                    // NOTE : For possible exceptions that may occure for each row I we can have different policies such as skipping, saving somewhere, log it, etc
                    // but here for sake of simplicity I have not handled them
                    var person = Person.Create(name, lastName);
                    _dbContext.People.Add(person);
                }

                _logger.LogInformation("{@LogSession}{@Message}", logSession, "Added people to dbContext");
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("{@LogSession}{@Message}", logSession, "Saved people");
            }
            catch (Exception ex)
            {
                _logger.LogError("{@LogSession}{@Exception}", logSession, ex);
                throw;
            }
        }
    }
}
