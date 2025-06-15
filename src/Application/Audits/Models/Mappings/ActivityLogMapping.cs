using Mapster;
using RuanFa.FashionShop.Application.Abstractions.Loggings.Entities;
using RuanFa.FashionShop.Application.Audits.Models.Responses;

namespace RuanFa.FashionShop.Application.Audits.Models.Mappings;
internal class ActivityLogMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Response
        config.NewConfig<ActivityLogEntry, ActivityLogResult>();
    }
}
