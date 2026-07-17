namespace AC.Domain.Common;

public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    public void AddEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public List<DomainEvent> ReleaseEvents()
    {
        var events = _domainEvents;
        _domainEvents.Clear();

        return events;
    }
}

