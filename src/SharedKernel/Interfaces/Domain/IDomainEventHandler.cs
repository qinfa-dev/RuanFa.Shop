namespace RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent, CancellationToken cancellationToken);
}
