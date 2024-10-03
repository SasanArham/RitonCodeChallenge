using Application.Modules.ContactManagement.Commands;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.ContactManagement
{
    public class PersonController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> PostAsync(ImportPeopleFromExcelCommand usecase)
        {
            var createdJobID = await Mediator.Send(usecase);
            return Accepted(new { ImportProcessID = createdJobID });
            // NOTE : then depending on the requirements,
            // we can have SignalR or another Get endpoint to check the status of this backgroung job
        }
    }
}
