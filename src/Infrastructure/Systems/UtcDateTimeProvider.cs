using RuanFa.FashionShop.SharedKernel.Interfaces.System;

namespace RuanFa.FashionShop.Infrastructure.Systems;
internal class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
