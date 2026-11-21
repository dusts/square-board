using SquareBoard;

namespace SquareBoard.Painter;

public sealed class Studio
{
    public Board Board { get; private set; } = Board.Blank("yard", "Yard");
    public string Tool { get; set; } = "wall";
    public int? LookX { get; private set; }
    public int? LookY { get; private set; }
    public int BurstSize { get; set; } = 2;

    public int Width
    {
        get => Board.MaxX - Board.MinX + 1;
        set => Resize(width: Math.Clamp(value, 6, 20), height: Height);
    }

    public int Height
    {
        get => Board.MaxY - Board.MinY + 1;
        set => Resize(width: Width, height: Math.Clamp(value, 6, 20));
    }

    public void NewBoard()
    {
        Board = Board.Blank("yard", "Yard");
        Tool = "wall";
        LookX = null;
        LookY = null;
    }

    public void Paint(int x, int y)
    {
        switch (Tool)
        {
            case "wall":
                if (Board.IsWall(x, y))
                    Board.RemoveAt(Board.Walls, x, y);
                else
                {
                    Board.RemoveAt(Board.SideA, x, y);
                    Board.RemoveAt(Board.SideB, x, y);
                    Board.Walls.Add(new Square(x, y));
                }
                break;
            case "a":
                Toggle(Board.SideA, Board.SideB, x, y);
                break;
            case "b":
                Toggle(Board.SideB, Board.SideA, x, y);
                break;
            case "look":
                LookX = x;
                LookY = y;
                break;
            default:
                Board.RemoveAt(Board.Walls, x, y);
                Board.RemoveAt(Board.SideA, x, y);
                Board.RemoveAt(Board.SideB, x, y);
                break;
        }
    }

    public bool IsSeen(int x, int y)
    {
        if (LookX is null || LookY is null || Board.IsWall(x, y))
            return false;
        return Area.Seen(Board, LookX.Value, LookY.Value, Area.Burst(LookX.Value, LookY.Value, BurstSize))
            .Contains((x, y));
    }

    private void Toggle(List<Square> keep, List<Square> other, int x, int y)
    {
        Board.RemoveAt(Board.Walls, x, y);
        Board.RemoveAt(other, x, y);
        if (Board.Contains(keep, x, y))
            Board.RemoveAt(keep, x, y);
        else
            keep.Add(new Square(x, y));
    }

    private void Resize(int width, int height)
    {
        Board.MaxX = Board.MinX + width - 1;
        Board.MaxY = Board.MinY + height - 1;
        Board.DropOutside();
    }
}
