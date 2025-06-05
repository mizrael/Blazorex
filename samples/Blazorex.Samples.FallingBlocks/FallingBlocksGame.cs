namespace Blazorex.Samples.FallingBlocks;

/// <summary>
/// /https://github.com/dionyziz/canvas-tetris/blob/master/js/render.js
/// </summary>
public static class FallingBlocksGame
{
    public const int W = 300;
    public const int H = 600;
    public const int COLS = 10;
    public const int ROWS = 20;
    public const int BLOCK_W = W / COLS;
    public const int BLOCK_H = H / ROWS;

    public static readonly int[][] Shapes =
    [
        [1, 1, 1, 1],
        [1, 1, 1, 0, 1],
        [1, 1, 1, 0, 0, 0, 1],
        [1, 1, 0, 0, 1, 1],
        [1, 1, 0, 0, 0, 1, 1],
        [0, 1, 1, 0, 1, 1],
        [0, 1, 0, 0, 1, 1, 1]
    ];

    public static readonly string[] Colors =
    [
        "cyan",
        "orange",
        "blue",
        "yellow",
        "red",
        "green",
        "purple"
    ];

    public static readonly Dictionary<int, string> KeyMap =
        new()
        {
            { 37, "left" }, // ArrowLeft
            { 39, "right" }, // ArrowRight
            { 40, "down" }, // ArrowDown
            { 38, "rotate" }, // ArrowUp
            { 32, "drop" } // Space
        };

    public static int[][] Board { get; set; } = [];

    public static int[][] Current { get; set; } = [];

    public static int CurrentX { get; set; } = 0;

    public static int CurrentY { get; set; } = 0;

    public static bool Frozen { get; set; } = false;

    public static bool Lose { get; set; } = false;

    public static void DrawBlock(IRenderContext ctx, float x, float y)
    {
        ctx.FillRect(BLOCK_W * x, BLOCK_H * y, BLOCK_W - 1, BLOCK_H - 1);
        ctx.StrokeRect(BLOCK_W * x, BLOCK_H * y, BLOCK_W - 1, BLOCK_H - 1);
    }

    public static void Init()
    {
        Board = [..Enumerable
        .Range(0, ROWS)
        .Select(_ => new int[COLS])];
    }

    public static void Render(IRenderContext ctx)
    {
        ctx.ClearRect(0, 0, W, H);

        ctx.StrokeStyle = "black";

        for (var x = 0; x < COLS; ++x)
        {
            for (var y = 0; y < ROWS; ++y)
            {
                if (Board[y][x] is not 0)
                {
                    ctx.FillStyle = Colors[Board[y][x] - 1];
                    DrawBlock(ctx, x, y);
                }
            }
        }

        ctx.FillStyle = "red";
        ctx.StrokeStyle = "black";

        for (var y = 0; y < 4; ++y)
        {
            for (var x = 0; x < 4; ++x)
            {
                if (Current[y][x] is not 0)
                {
                    ctx.FillStyle = Colors[Current[y][x] - 1];
                    DrawBlock(ctx, CurrentX + x, CurrentY + y);
                }
            }
        }
    }

    public static bool Valid(int offsetX, int offsetY, int[][]? newCurrent = null)
    {
        offsetX = CurrentX + offsetX;
        offsetY = CurrentY + offsetY;

        newCurrent ??= Current;

        for (var y = 0; y < 4; ++y)
        {
            for (var x = 0; x < 4; ++x)
            {
                if (newCurrent[y][x] != 0)
                {
                    if (
                        y + offsetY < 0
                        || y + offsetY >= ROWS
                        || x + offsetX < 0
                        || x + offsetX >= COLS
                        || Board[y + offsetY][x + offsetX] != 0
                    )
                    {
                        if (offsetY == 1 && Frozen)
                        {
                            Lose = true; // lose if the current shape is settled at the top most row
                        }

                        return false;
                    }
                }
            }
        }

        return true;
    }

    public static void ClearLines()
    {
        for (var y = ROWS - 1; y >= 0; --y)
        {
            var rowFilled = true;

            for (var x = 0; x < COLS; ++x)
            {
                if (Board[y][x] == 0)
                {
                    rowFilled = false;
                    break;
                }
            }

            if (rowFilled)
            {
                for (var yy = y; yy > 0; --yy)
                {
                    for (var x = 0; x < COLS; ++x)
                    {
                        Board[yy][x] = Board[yy - 1][x];
                    }
                }
                ++y;
            }
        }
    }

    public static int[][] Rotate(int[][] current)
    {
        int[][] newCurrent = new int[4][];

        for (var y = 0; y < 4; ++y)
        {
            newCurrent[y] = new int[4];

            for (var x = 0; x < 4; ++x)
            {
                newCurrent[y][x] = current[3 - x][y];
            }
        }

        return newCurrent;
    }

    public static void Freeze()
    {
        for (var y = 0; y < 4; ++y)
        {
            for (var x = 0; x < 4; ++x)
            {
                if (Current[y][x] is not 0)
                {
                    Board[y + CurrentY][x + CurrentX] = Current[y][x];
                }
            }
        }

        Frozen = true;
    }

    public static void Tick()
    {
        if (Valid(0, 1))
        {
            ++CurrentY;
        }
        else
        {
            Freeze();
            Valid(0, 1);
            ClearLines();

            if (Lose)
            {
                return;
            }

            NewShape();
        }
    }

    public static void KeyUp(int key)
    {
        if (!KeyMap.TryGetValue(key, out string? value) || Lose)
        {
            return; // invalid key or game over
        }

        switch (value)
        {
            case "left":
                if (Valid(-1, 0))
                {
                    --CurrentX;
                }
                break;

            case "right":
                if (Valid(1, 0))
                {
                    ++CurrentX;
                }
                break;

            case "down":
                if (Valid(0, 1))
                {
                    ++CurrentY;
                }
                break;

            case "rotate":
                var rotated = Rotate(Current);
                if (Valid(0, 0, rotated))
                {
                    Current = rotated;
                }
                break;

            case "drop":

                while (Valid(0, 1))
                {
                    ++CurrentY;
                }
                Tick();
                break;
        }
    }

    public static void NewShape()
    {
        var id = Random.Shared.Next(Shapes.Length);
        var shape = Shapes[id]; // maintain id for color filling

        Current = new int[4][]; // Create proper 4-row array

        for (var y = 0; y < 4; ++y)
        {
            Current[y] = new int[4]; // Create each row with 4 columns

            for (var x = 0; x < 4; ++x)
            {
                var i = 4 * y + x;

                if (i < shape.Length && shape[i] != 0)
                {
                    Current[y][x] = id + 1;
                }
                else
                {
                    Current[y][x] = 0;
                }
            }
        }

        // new shape starts to move
        Frozen = false;

        // position where the shape will evolve
        CurrentX = 5;
        CurrentY = 0;
    }
}
