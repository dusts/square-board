namespace SquareBoard;

/// <summary>
/// Square rings and square blocks. A ring is centered on a square and does not include it.
/// A block grows away from that square. Squares behind a wall are dropped.
/// </summary>
public static class Area
{
    public static IReadOnlyList<(int X, int Y)> Burst(int originX, int originY, int size)
    {
        if (size <= 0)
            return [];

        var squares = new List<(int X, int Y)>();
        for (var y = originY - size; y <= originY + size; y++)
        {
            for (var x = originX - size; x <= originX + size; x++)
            {
                var distance = Squares.Distance(originX, originY, x, y);
                if (distance >= 1 && distance <= size)
                    squares.Add((x, y));
            }
        }

        return squares;
    }

    /// <summary>
    /// A size-by-size block whose near corner is the anchor.
    /// It grows away from the origin. A tied axis grows toward positive.
    /// </summary>
    public static IReadOnlyList<(int X, int Y)> Cube(int originX, int originY, int anchorX, int anchorY, int size)
    {
        if (size <= 0)
            return [];

        var stepX = Math.Sign(anchorX - originX);
        var stepY = Math.Sign(anchorY - originY);
        if (stepX == 0)
            stepX = 1;
        if (stepY == 0)
            stepY = 1;

        var squares = new List<(int X, int Y)>(size * size);
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
                squares.Add((anchorX + (x * stepX), anchorY + (y * stepY)));
        }

        return squares;
    }

    public static bool LegalCubeAnchor(int originX, int originY, int x, int y, int within)
    {
        var distance = Squares.Distance(originX, originY, x, y);
        return distance >= 1 && distance <= Math.Max(1, within);
    }

    /// <summary>
    /// Keeps squares that can be seen from the origin. Walls and squares past a wall are left out.
    /// A null board keeps every square.
    /// </summary>
    public static IReadOnlyList<(int X, int Y)> Seen(
        Board? board,
        int fromX,
        int fromY,
        IEnumerable<(int X, int Y)> squares)
    {
        return squares.Where(square =>
            (board is null || (board.InBounds(square.X, square.Y) && !board.IsWall(square.X, square.Y)))
            && LineOfEffect.Between(board, fromX, fromY, square.X, square.Y)).ToList();
    }
}
