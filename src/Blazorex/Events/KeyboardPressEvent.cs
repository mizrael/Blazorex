using System.Text.Json.Serialization;
using Blazorex.Serialization;

namespace Blazorex;

[JsonConverter(typeof(KeyboardPressEventJsonConverter))]
public readonly struct KeyboardPressEvent(
    int keyCode,
    string key,
    bool isHeld,
    int[] heldKeys,
    KeyboardModifierState keyboardModifierState
)
{
    public readonly int KeyCode = keyCode;

    public readonly string Key = key;

    public readonly bool IsHeld = isHeld;

    public readonly int[] HeldKeys = heldKeys;

    public readonly KeyboardModifierState Modifiers = keyboardModifierState;

    public static KeyboardPressEvent None =>
        new(0, string.Empty, false, [], KeyboardModifierState.None);
}

[JsonConverter(typeof(KeyboardModifierJsonConverter))]
public readonly struct KeyboardModifierState(bool shift, bool ctrl, bool alt, bool meta)
{
    public readonly bool Shift = shift;

    public readonly bool Ctrl = ctrl;

    public readonly bool Alt = alt;

    public readonly bool Meta = meta;

    public static KeyboardModifierState None => new(false, false, false, false);
}
