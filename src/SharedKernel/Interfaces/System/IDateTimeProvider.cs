namespace RuanFa.FashionShop.SharedKernel.Interfaces.System;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
