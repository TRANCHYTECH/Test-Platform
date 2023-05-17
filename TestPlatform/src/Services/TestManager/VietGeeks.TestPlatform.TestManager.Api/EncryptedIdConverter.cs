using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Api
{
    public class EncryptedIdConverter : JsonConverter<EncryptedId>
    {
        public override EncryptedId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, EncryptedId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}

