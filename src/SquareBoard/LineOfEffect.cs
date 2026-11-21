namespace SquareBoard;

/// <summary>
/// Corner-to-corner line of effect.
/// A wall blocks a line that runs through it or along its edge.
/// A line that only touches a corner stays clear.
/// A null board is an open field, so every square can be seen.
/// </summary>
public static class LineOfEffect
{
    public static bool Between(Board? board, int fromX, int fromY, int toX, int toY)
    {
        if (board is null || (fromX == toX && fromY == toY))
            return true;
        if (board.Walls.Count == 0)
            return true;

        for (var fromCorner = 0; fromCorner < 4; fromCorner++)
        {
            var (ax, ay) = Corner(fromX, fromY, fromCorner);
            for (var toCorner = 0; toCorner < 4; toCorner++)
            {
                var (bx, by) = Corner(toX, toY, toCorner);
                if ((ax == bx && ay == by) || !Blocked(board, ax, ay, bx, by))
                    return true;
            }
        }

        return false;
    }

    private static (int X, int Y) Corner(int x, int y, int index) => index switch
    {
        0 => (x, y),
        1 => (x + 1, y),
        2 => (x, y + 1),
        _ => (x + 1, y + 1),
    };

    private static bool Blocked(Board board, int ax, int ay, int bx, int by)
    {
        foreach (var wall in board.Walls)
        {
            if (Crosses(ax, ay, bx, by, wall.X, wall.Y))
                return true;
        }

        return false;
    }

    /// <summary>True when the segment overlaps the closed wall square in more than one point.</summary>
    private static bool Crosses(int ax, int ay, int bx, int by, int sx, int sy)
    {
        var lower = new Bound(0, 1);
        var upper = new Bound(1, 1);
        if (!Clip(ax, bx - ax, sx, sx + 1, ref lower, ref upper))
            return false;
        return Clip(ay, by - ay, sy, sy + 1, ref lower, ref upper);
    }

    private static bool Clip(int origin, int delta, int low, int high, ref Bound lower, ref Bound upper)
    {
        if (delta == 0)
            return origin >= low && origin <= high && Bound.Compare(lower, upper) < 0;

        Bound nextLower;
        Bound nextUpper;
        if (delta > 0)
        {
            nextLower = new Bound(low - origin, delta);
            nextUpper = new Bound(high - origin, delta);
        }
        else
        {
            var den = -delta;
            nextLower = new Bound(-(high - origin), den);
            nextUpper = new Bound(-(low - origin), den);
        }

        if (Bound.Compare(nextLower, lower) > 0)
            lower = nextLower;
        if (Bound.Compare(nextUpper, upper) < 0)
            upper = nextUpper;
        return Bound.Compare(lower, upper) < 0;
    }

    private readonly record struct Bound(long Num, long Den)
    {
        public static int Compare(Bound left, Bound right)
        {
            var a = left.Num * right.Den;
            var b = right.Num * left.Den;
            return a < b ? -1 : a > b ? 1 : 0;
        }
    }
}
