using RuanFa.FashionShop.Domain.Accounts.Entities;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.Domain.Accounts.Events;
public record UserProfileCreatedEvent(UserProfile Profile) : IDomainEvent;
public record UserProfileUpdatedEvent(UserProfile Profile) : IDomainEvent;
