using Domain.Modules.ContactManagement.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Base
{
    public interface IDatabaseContext
    {
        DbSet<Person> People { get; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
