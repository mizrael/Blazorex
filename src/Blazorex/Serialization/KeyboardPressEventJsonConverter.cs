using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorex.Serialization;

internal sealed class KeyboardPressEventJsonConverter : JsonConverter<KeyboardPressEvent>
{
    public override KeyboardPressEvent Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        int keyCode = 0;
        string key = string.Empty;
        bool isHeld = false;
        int[] heldKeys = [];
        KeyboardModifierState modifiers = new();

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
                    case "keycode":
                        keyCode = reader.GetInt32();
                        break;

                    case "key":
                        key = reader.GetString() ?? string.Empty;
                        break;

                    case "isheld":
                        isHeld = reader.GetBoolean();
                        break;

                    case "heldkeys":
                        heldKeys = JsonSerializer.Deserialize<int[]>(ref reader, options) ?? [];
                        break;

                    case "modifiers":
                        modifiers = JsonSerializer.Deserialize<KeyboardModifierState>(
                            ref reader,
                            options
                        );
                        break;
                }
            }
        }

        return new KeyboardPressEvent(keyCode, key, isHeld, heldKeys, modifiers);
    }

    public override void Write(
        Utf8JsonWriter writer,
        KeyboardPressEvent value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        writer.WriteNumber("keyCode", value.KeyCode);
        writer.WriteString("key", value.Key);
        writer.WriteBoolean("isHeld", value.IsHeld);

        writer.WritePropertyName("heldKeys");
        JsonSerializer.Serialize(writer, value.HeldKeys, options);

        writer.WritePropertyName("modifiers");
        JsonSerializer.Serialize(writer, value.Modifiers, options);

        writer.WriteEndObject();
    }
}
