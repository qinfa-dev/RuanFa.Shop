using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RuanFa.FashionShop.Web.Api.Extensions
{
    /// <summary>
    /// Converter for DateTimeOffset that serializes to UTC with "Z" suffix.
    /// </summary>
    public class SystemTextJsonUtcDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var dateString = reader.GetString();
                if (string.IsNullOrEmpty(dateString))
                    throw new JsonException("Cannot convert empty string to DateTimeOffset.");

                if (DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dto))
                    return dto;

                throw new JsonException($"Invalid date format: {dateString}");
            }

            throw new JsonException($"Unexpected token parsing date. Expected String, got {reader.TokenType}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            string isoString = value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff", CultureInfo.InvariantCulture) + "Z";
            writer.WriteStringValue(isoString);
        }
    }

    /// <summary>
    /// Converter for nullable DateTimeOffset that serializes to UTC with "Z" suffix or null.
    /// </summary>
    public class SystemTextJsonNullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
    {
        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.String)
            {
                var dateString = reader.GetString();
                if (string.IsNullOrEmpty(dateString))
                    return null;

                if (DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dto))
                    return dto;

                throw new JsonException($"Invalid date format: {dateString}");
            }

            throw new JsonException($"Unexpected token parsing date. Expected String or Null, got {reader.TokenType}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                string isoString = value.Value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff", CultureInfo.InvariantCulture) + "Z";
                writer.WriteStringValue(isoString);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
