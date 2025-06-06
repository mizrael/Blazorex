namespace Blazorex.Samples.KeyAndMouse;

/// <summary>
/// High-performance key rain engine that visualizes keystrokes as falling characters
/// </summary>
public static class KeyRainEngine
{
    public const int CanvasWidth = 1024;
    public const int CanvasHeight = 768;
    public const float Gravity = 0.1f;
    public const float KeySize = 40f;
    public const float KeyRadius = 6f;
    public const int GroundLevel = CanvasHeight - 50;

    // Styling constants
    private const string KeyBackground = "#2c3e50";
    private const string KeyBorder = "#34495e";
    private const string KeyText = "#ecf0f1";
    private const string KeyFont = "bold 18px 'Segoe UI', Arial, sans-serif";

    // Performance: Pre-allocate list with reasonable capacity and limit max keys
    private static readonly List<FallingKey> _fallingKeys = new(50);
    private const int MaxKeys = 20; // Prevent performance degradation

    // Prevent duplicate key events
    private static readonly HashSet<string> _recentKeys = new();
    private static DateTime _lastKeyTime = DateTime.MinValue;
    private const int DedupeWindowMs = 50; // 50ms window to prevent duplicates

    /// <summary>Adds a new falling key for the specified character</summary>
    public static void AddKey(string character)
    {
        // Dedupe rapid key events (keydown + keypress, etc.)
        var now = DateTime.UtcNow;
        var keyId = $"{character}_{now.Ticks / 500000}"; // 50ms buckets

        if (_recentKeys.Contains(keyId))
        {
            return; // Duplicate within time window
        }

        // Clean old entries periodically
        if ((now - _lastKeyTime).TotalMilliseconds > DedupeWindowMs * 2)
        {
            _recentKeys.Clear();
        }

        _recentKeys.Add(keyId);
        _lastKeyTime = now;

        // Performance: Limit number of simultaneous keys
        if (_fallingKeys.Count >= MaxKeys)
        {
            _fallingKeys.RemoveAt(0); // Remove oldest key
        }

        // Clamp character to single char for display
        var displayChar = string.IsNullOrEmpty(character)
            ? "?"
            : character[0].ToString().ToUpperInvariant();

        // Generate random X position ensuring key stays within bounds
        var x = Random.Shared.NextSingle() * (CanvasWidth - KeySize) + KeySize / 2;

        _fallingKeys.Add(new FallingKey(displayChar, x, -KeySize, 0f));
    }

    /// <summary>Updates physics and renders all falling keys</summary>
    public static void UpdateAndRender(IRenderContext ctx)
    {
        // CRITICAL: Clear canvas completely to prevent trails
        ctx.ClearRect(0, 0, CanvasWidth, CanvasHeight);

        // Set dark background
        ctx.FillStyle = "#1a1a1a";
        ctx.FillRect(0, 0, CanvasWidth, CanvasHeight);

        // Process keys in reverse order for safe removal
        for (var i = _fallingKeys.Count - 1; i >= 0; i--)
        {
            var key = _fallingKeys[i];

            // Update physics
            var updatedKey = key.Update();

            // Remove if below ground level
            if (updatedKey.Y > GroundLevel)
            {
                _fallingKeys.RemoveAt(i);
                continue;
            }

            // Render the key
            RenderKey(ctx, updatedKey);

            // Update the key in place
            _fallingKeys[i] = updatedKey;
        }
    }

    /// <summary>Renders a single falling key with modern styling</summary>
    private static void RenderKey(IRenderContext ctx, FallingKey key)
    {
        // Performance: Avoid Save/Restore for better performance
        // Store current state
        var previousFillStyle = ctx.FillStyle;
        var previousStrokeStyle = ctx.StrokeStyle;
        var previousLineWidth = ctx.LineWidth;
        var previousFont = ctx.Font;
        var previousTextAlign = ctx.TextAlign;
        var previousTextBaseline = ctx.TextBaseline;

        try
        {
            // Draw key background with rounded corners
            ctx.FillStyle = KeyBackground;
            ctx.StrokeStyle = KeyBorder;
            ctx.LineWidth = 2;

            var keyLeft = key.X - KeySize / 2;
            var keyTop = key.Y;

            ctx.BeginPath();
            ctx.RoundRect(keyLeft, keyTop, KeySize, KeySize, KeyRadius);
            ctx.Fill();
            ctx.Stroke();

            // Draw character text
            ctx.Font = KeyFont;
            ctx.FillStyle = KeyText;
            ctx.TextAlign = TextAlign.Center;
            ctx.TextBaseline = TextBaseline.Middle;

            var textX = key.X;
            var textY = key.Y + KeySize / 2;

            ctx.FillText(key.Character, textX, textY);
        }
        finally
        {
            // Restore state manually for better performance
            ctx.FillStyle = previousFillStyle;
            ctx.StrokeStyle = previousStrokeStyle;
            ctx.LineWidth = previousLineWidth;
            ctx.Font = previousFont;
            ctx.TextAlign = previousTextAlign ?? TextAlign.Center;
            ctx.TextBaseline = previousTextBaseline ?? TextBaseline.Middle;
        }
    }

    /// <summary>Gets current number of active falling keys</summary>
    public static int ActiveKeyCount => _fallingKeys.Count;

    /// <summary>Clears all falling keys</summary>
    public static void Clear() => _fallingKeys.Clear();
}

/// <summary>Immutable record representing a falling key with physics</summary>
public readonly record struct FallingKey(string Character, float X, float Y, float Velocity)
{
    /// <summary>Updates key position and velocity for next frame</summary>
    public FallingKey Update() =>
        this with
        {
            Velocity = Velocity + KeyRainEngine.Gravity,
            Y = Y + Velocity
        };
}
