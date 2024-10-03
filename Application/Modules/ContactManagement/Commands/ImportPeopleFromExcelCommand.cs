using Application.Common;
using Application.Modules.ContactManagement.Helpers;
using Application.Modules.ContactManagement.Services;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Modules.ContactManagement.Commands
{
    public record ImportPeopleFromExcelCommand : IRequest<string>
    {
        public IFormFile File { get; init; }
    }

    public class ImportPeopleFromExcelCommandHandler : IRequestHandler<ImportPeopleFromExcelCommand, string>
    {
        private readonly IFileUploader _fileUploader;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IPersonImporter _personImporter;

        public ImportPeopleFromExcelCommandHandler(IFileUploader fileUploader
            , IBackgroundJobClient backgroundJobClient
            , IPersonImporter personImporter)
        {
            _fileUploader = fileUploader;
            _backgroundJobClient = backgroundJobClient;
            _personImporter = personImporter;
        }

        public async Task<string> Handle(ImportPeopleFromExcelCommand command, CancellationToken cancellationToken)
        {
            var uploadedFileName = await _fileUploader.UploadAsync(command.File, PeopleDirectoyHelper.ExcelImports());
            var jobID = _backgroundJobClient.Enqueue(() => _personImporter.ImportFromExcelAsync($"{PeopleDirectoyHelper.ExcelImports()}{uploadedFileName}"));
            return jobID;
        }
    }
}
