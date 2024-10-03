using Application.Base;
using ClosedXML.Excel;
using Domain.Modules.ContactManagement.People;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

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
                using (var stream = File.OpenRead(filePath))
                {
                    var workBook = new XLWorkbook(stream);
                    var workSheet = workBook.Worksheets.First();
                    var range = workSheet.RangeUsed();
                    var rowCount = range.RowCount();

                    var peopleToAdd = new ConcurrentBag<Person>();
                    Parallel.ForEach(Enumerable.Range(2, rowCount - 1), i =>
                    {
                        // NOTE : For possible exceptions that may occure for each row I we can have different policies such as skipping, saving somewhere, log it, etc
                        // but here for sake of simplicity I have not handled them
                        var row = workSheet.Row(i);
                        var person = CreatePerson(row);
                        peopleToAdd.Add(person);
                    });

                    await _dbContext.People.AddRangeAsync(peopleToAdd);
                    _logger.LogInformation("{@LogSession}{@Message}", logSession, "Added people to dbContext");
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("{@LogSession}{@Message}", logSession, "Saved people");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{@LogSession}{@Exception}", logSession, ex);
                throw;
            }
        }


        private static Person CreatePerson(IXLRow row)
        {
            // NOTE : could have been an extension method or an static factory 
            var cells = row.Cells();
            var rowCells = cells.ToList();

            var name = rowCells[0].Value.ToString().Trim();
            var lastName = rowCells[1].Value.ToString().Trim();

            var person = Person.Create(name, lastName);
            return person;
        }
    }
}
