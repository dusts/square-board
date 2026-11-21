namespace SquareBoard;

/// <summary>Chebyshev steps. A diagonal move costs the same as an orthogonal one.</summary>
public static class Squares
{
    public static int Distance(int x1, int y1, int x2, int y2) =>
        Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));

    public static (int X, int Y) StepToward(int x, int y, int targetX, int targetY)
    {
        var dx = targetX - x;
        var dy = targetY - y;
        if (dx == 0 && dy == 0)
            return (x, y);
        return (x + Math.Sign(dx), y + Math.Sign(dy));
    }
}
