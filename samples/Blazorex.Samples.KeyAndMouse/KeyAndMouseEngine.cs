namespace Blazorex.Samples.KeyAndMouse;

public static class KeyAndMouseEngine
{
    public const int W = 1024;
    public const int H = 768;

    // Constants
    private const float Gravity = 0.1f;
    private const float ParticleRadius = 10f;
    private const float ParticleGravity = 0.2f;
    private const float ParticleAlphaDecay = 0.03f;
    private const int ParticleInitialLife = 30;
    private const float KeyCapWidth = 60f;
    private const float KeyCapHeight = 40f;
    private const float KeyCapRadius = 8f;
    private const float GroundLevel = 600f;

    private const string KeyCapFill = "#fff";
    private const string KeyCapStroke = "#333";
    private const string KeyCapTextColor = "red";
    private const string KeyCapFont = "bold 20px sans-serif";

    // Use List for better perf and easier removal
    private static readonly List<KeyCap> FallingKeyCaps = [];
    private static readonly string[] GradientColors = ["rgba(0,150,255,0.7)", "rgba(0,150,255,0)"];
    private static readonly List<Particle> MouseTrail = [];

    public static void PlaceAndMakeKeyCapFallDown(string keyLabel)
    {
        // More efficient: find and replace in single pass
        var existingIndex = FallingKeyCaps.FindIndex(k => k.Label == keyLabel);
        var newKeyCap = new KeyCap(
            keyLabel,
            Random.Shared.NextSingle() * (W - KeyCapWidth) + KeyCapWidth / 2,
            0,
            0
        );

        if (existingIndex >= 0)
        {
            FallingKeyCaps[existingIndex] = newKeyCap;
        }
        else
        {
            FallingKeyCaps.Add(newKeyCap);
        }
    }

    public static void AddMouseTrail(double x, double y)
    {
        if (MouseTrail.Count > 200)
        {
            MouseTrail.RemoveAt(0);
        }

        MouseTrail.Add(
            new Particle(
                x,
                y,
                (Random.Shared.NextSingle() - 0.5f) * 1.5f,
                (Random.Shared.NextSingle() - 0.5f) * 1.5f,
                1.0f,
                ParticleInitialLife
            )
        );
    }

    public static void UpdateAndDrawMouseTrail(IRenderContext ctx)
    {
        for (int i = MouseTrail.Count - 1; i >= 0; i--)
        {
            var p = MouseTrail[i].Update();

            if (p.Life <= 0 || p.Alpha <= 0)
            {
                MouseTrail.RemoveAt(i);
                continue;
            }

            ctx.Save();
            ctx.GlobalAlpha = MathF.Max(0, p.Alpha);

            // Clear any existing stroke settings from previous draws
            ctx.LineWidth = 0;
            ctx.StrokeStyle = "transparent";

            var grad = ctx.CreateRadialGradient(
                (float)p.X,
                (float)p.Y,
                0,
                (float)p.X,
                (float)p.Y,
                ParticleRadius
            );
            grad.AddColorStop(0, GradientColors[0]);
            grad.AddColorStop(1, GradientColors[1]);
            ctx.FillStyle = grad;
            ctx.BeginPath();
            ctx.Arc((float)p.X, (float)p.Y, ParticleRadius, 0, MathF.Tau);
            ctx.Fill(); // Only fill, explicitly no stroke
            ctx.Restore();

            MouseTrail[i] = p;
        }
    }

    public static void UpdateAndDrawFallingKeyCaps(IRenderContext ctx)
    {
        for (int i = FallingKeyCaps.Count - 1; i >= 0; i--)
        {
            var cap = FallingKeyCaps[i].Fall();

            if (cap.Y > GroundLevel)
            {
                FallingKeyCaps.RemoveAt(i);
                continue;
            }

            ctx.Save();
            ctx.FillStyle = KeyCapFill;
            ctx.StrokeStyle = KeyCapStroke;
            ctx.LineWidth = 2;
            ctx.RoundRect(cap.X - KeyCapWidth / 2, cap.Y, KeyCapWidth, KeyCapHeight, KeyCapRadius);
            ctx.Fill();
            ctx.Stroke();

            ctx.Font = KeyCapFont;
            ctx.FillStyle = KeyCapTextColor;
            ctx.TextAlign = TextAlign.Center;
            ctx.TextBaseline = TextBaseline.Middle;
            ctx.FillText(cap.Label, cap.X, cap.Y + KeyCapHeight / 2);
            ctx.Restore();

            FallingKeyCaps[i] = cap;
        }
    }

    private readonly record struct KeyCap(string Label, float X, float Y, float Velocity)
    {
        public KeyCap Fall() =>
            this with
            {
                Velocity = Velocity + Gravity,
                Y = Y + Velocity + Gravity // Use updated velocity
            };
    }

    private readonly record struct Particle(
        double X,
        double Y,
        float VX,
        float VY,
        float Alpha,
        int Life
    )
    {
        public Particle Update() =>
            this with
            {
                X = X + VX,
                Y = Y + VY + ParticleGravity,
                Life = Life - 1,
                Alpha = Alpha - ParticleAlphaDecay
            };
    }
}
