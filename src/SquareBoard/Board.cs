namespace SquareBoard;

/// <summary>
/// A rectangle of squares, the walls inside it, and two start zones.
/// A null board is an open field with no edges.
/// </summary>
public sealed class Board
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int MinX { get; set; }
    public int MaxX { get; set; } = 11;
    public int MinY { get; set; }
    public int MaxY { get; set; } = 7;
    public List<Square> Walls { get; set; } = [];
    public List<Square> SideA { get; set; } = [];
    public List<Square> SideB { get; set; } = [];

    public bool InBounds(int x, int y) => x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;

    public bool IsWall(int x, int y) => Contains(Walls, x, y);

    /// <summary>Outside the rectangle, or a wall.</summary>
    public bool Blocks(int x, int y) => !InBounds(x, y) || IsWall(x, y);

    public void DropOutside()
    {
        if (MaxX < MinX)
            MaxX = MinX;
        if (MaxY < MinY)
            MaxY = MinY;
        Walls.RemoveAll(s => !InBounds(s.X, s.Y));
        Distinct(Walls);
        bool Open(Square square) => InBounds(square.X, square.Y) && !IsWall(square.X, square.Y);
        SideA.RemoveAll(s => !Open(s));
        SideB.RemoveAll(s => !Open(s));
        Distinct(SideA);
        Distinct(SideB);
    }

    public static Board Blank(string id, string name) => new()
    {
        Id = id,
        Name = name,
        MinX = 0,
        MaxX = 11,
        MinY = 0,
        MaxY = 7,
        SideA = [new Square(0, 1), new Square(1, 1), new Square(2, 1), new Square(3, 1), new Square(4, 1)],
        SideB = [new Square(7, 6), new Square(8, 6), new Square(9, 6), new Square(10, 6)],
    };

    public static bool Contains(List<Square> squares, int x, int y) =>
        squares.Any(s => s.X == x && s.Y == y);

    public static void RemoveAt(List<Square> squares, int x, int y) =>
        squares.RemoveAll(s => s.X == x && s.Y == y);

    private static void Distinct(List<Square> squares)
    {
        var seen = new HashSet<(int, int)>();
        squares.RemoveAll(s => !seen.Add((s.X, s.Y)));
    }
}
