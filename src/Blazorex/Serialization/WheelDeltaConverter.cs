using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace Blazorex.Serialization;

public class WheelDeltaConverter : JsonConverter<WheelDelta>
{
    public override WheelDelta Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        int deltaX = 0, deltaY = 0, clientX = 0, clientY = 0;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName.ToLower())
                {
                    case "deltax":
                        deltaX = reader.GetInt32();
                        break;
                    case "deltay":
                        deltaY = reader.GetInt32();
                        break;
                    case "clientx":
                        clientX = reader.GetInt32();
                        break;
                    case "clienty":
                        clientY = reader.GetInt32();
                        break;
                }
            }
        }

        return new WheelDelta(clientX, clientY, deltaX, deltaY);
    }

    public override void Write(Utf8JsonWriter writer, WheelDelta value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("deltaX", value.DeltaX);
        writer.WriteNumber("deltaY", value.DeltaY);
        writer.WriteNumber("clientX", value.ClientX);
        writer.WriteNumber("clientY", value.ClientY);
        writer.WriteEndObject();
    }
}
