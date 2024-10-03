using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Base
{
    public abstract class BaseEntity
    {
        public string ID { get; set; }
        public bool Deleted { get; set; }
        public virtual void Delete()
        {
            Deleted = true;
        }

        private readonly List<BaseEvent> _DomainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _DomainEvents.AsReadOnly();
        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _DomainEvents.Add(domainEvent);
        }

        public void ClearEventualConsistencyDomainEvents()
        {
            _DomainEvents.Clear();
        }
    }
}
