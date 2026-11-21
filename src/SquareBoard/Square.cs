namespace SquareBoard;

public sealed class Square
{
    public Square()
    {
    }

    public Square(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
}
