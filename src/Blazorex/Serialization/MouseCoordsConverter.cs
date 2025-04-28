using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace Blazorex.Serialization;

public class MouseCoordsConverter : JsonConverter<MouseCoords>
{
    public override MouseCoords Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        double clientX = 0, clientY = 0, offsetX = 0, offsetY = 0;

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
                    case "clientx":
                        clientX = reader.GetDouble();
                        break;
                    case "clienty":
                        clientY = reader.GetDouble();
                        break;
                    case "offsetx":
                        offsetX = reader.GetDouble();
                        break;
                    case "offsety":
                        offsetY = reader.GetDouble();
                        break;
                }
            }
        }
        return new MouseCoords(clientX, clientY, offsetX, offsetY);
    }

    public override void Write(Utf8JsonWriter writer, MouseCoords value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("clientX", value.ClientX);
        writer.WriteNumber("clientY", value.ClientY);
        writer.WriteNumber("offsetX", value.OffsetX);
        writer.WriteNumber("offsetY", value.OffsetY);
        writer.WriteEndObject();
    }
}