﻿using System.Text.Json;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RuanFa.FashionShop.Infrastructure.Data.Converters;

public class ValueJsonConverter<T>(ConverterMappingHints? mappingHints = null)
    : ValueConverter<T, string>(
        v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
        v => JsonSerializer.Deserialize<T>(v, JsonSerializerOptions.Default)!,
        mappingHints)
{
}

public class ValueJsonComparer<T>() : ValueComparer<T>(
    (l, r) => JsonSerializer.Serialize(l, JsonSerializerOptions.Default) ==
              JsonSerializer.Serialize(r, JsonSerializerOptions.Default),
    v => v == null ? 0 : JsonSerializer.Serialize(v, JsonSerializerOptions.Default).GetHashCode(),
    v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
        JsonSerializerOptions.Default)!);
