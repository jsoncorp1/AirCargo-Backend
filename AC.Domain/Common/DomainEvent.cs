namespace AC.Domain.Common;

public abstract record DomainEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
