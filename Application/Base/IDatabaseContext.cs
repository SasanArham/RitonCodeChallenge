using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Base
{
    public interface IDatabaseContext
    {
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
