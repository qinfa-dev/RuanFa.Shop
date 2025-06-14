namespace RuanFa.FashionShop.Infrastructure.Data.Seeds;
public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}

