using Application.Base;
using Domain.Base;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        private readonly IMediator _mediator;

        public DatabaseContext(DbContextOptions options
            , IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");

            // These three configs are required if we want to implement OutBox pattern with MassTransit
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // I could have do this within EF intercerptor or Unit of work 
            // but for simplicity I hust put it here
            var entities = ChangeTracker
                    .Entries<BaseEntity>()
                    .Where(e => e.Entity.DomainEvents.Any())
                    .Select(e => e.Entity);

            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entities.ToList().ForEach(e => e.ClearEventualConsistencyDomainEvents());

            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent, cancellationToken);

            var res = await base.SaveChangesAsync(cancellationToken);
            return res;
        }
    }
}
