namespace RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

public interface IHasDomainEvent
{
    List<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
    void AddDomainEvent(IDomainEvent domainEvent);
}
