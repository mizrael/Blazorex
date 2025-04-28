using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace Blazorex.Serialization;

public class MouseButtonDataConverter : JsonConverter<MouseButtonData>
{
    public override MouseButtonData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        double clientX = 0, clientY = 0;
        int button = 0;

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
                    case "button":
                        button = reader.GetInt32();
                        break;
                    case "clientx":
                        clientX = reader.GetDouble();
                        break;
                    case "clienty":
                        clientY = reader.GetDouble();
                        break;
                }
            }
        }

        return new MouseButtonData(clientX, clientY, button);
    }

    public override void Write(Utf8JsonWriter writer, MouseButtonData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("button", value.Button);
        writer.WriteNumber("clientX", value.ClientX);
        writer.WriteNumber("clientY", value.ClientY);
        writer.WriteEndObject();
    }
}