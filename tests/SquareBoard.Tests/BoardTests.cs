namespace SquareBoard.Tests;

public class BoardTests
{
    [Fact]
    public void Open_approach_follows_the_straight_line()
    {
        var path = GridPath.Approach(0, 4, 0, 0, budget: 5, stopAt: 1, blocked: (_, _) => false);

        Assert.Equal([(0, 4), (0, 3), (0, 2), (0, 1)], path);
    }

    [Fact]
    public void A_wall_square_is_not_a_goal()
    {
        var board = Box(0, 4, 0, 4);
        Wall(board, (0, 1), (1, 1), (0, 2), (1, 2));

        Assert.Null(GridPath.Find(0, 0, 0, 1, budget: 5, board.Blocks));
        var around = GridPath.Find(0, 0, 0, 3, budget: 5, board.Blocks);
        Assert.NotNull(around);
        Assert.Equal((0, 3), around[^1]);
        Assert.DoesNotContain(around, square => board.IsWall(square.X, square.Y));
    }

    [Fact]
    public void Approach_steps_around_a_wall_and_stops_short()
    {
        var board = Box(0, 4, 0, 4);
        Wall(board, (2, 0), (2, 1), (2, 2), (2, 3));

        var path = GridPath.Approach(0, 2, 4, 2, budget: 6, stopAt: 1, board.Blocks);

        Assert.Equal(1, Squares.Distance(path[^1].X, path[^1].Y, 4, 2));
        Assert.Equal((3, 3), path[^1]);
        Assert.DoesNotContain(path, square => board.IsWall(square.X, square.Y));
    }

    [Fact]
    public void Reachable_includes_the_eight_neighbors()
    {
        var reach = GridPath.Reachable(0, 0, budget: 1, blocked: (_, _) => false);

        Assert.Equal(9, reach.Count);
        Assert.Contains((1, 1), reach);
        Assert.DoesNotContain((2, 0), reach);
    }

    [Fact]
    public void Seek_walks_to_the_accepted_square()
    {
        var path = GridPath.Seek(0, 0, budget: 3, blocked: (_, _) => false, accept: (x, y) => x == 2 && y == 0);

        Assert.Equal([(0, 0), (1, -1), (2, 0)], path);
    }

    [Fact]
    public void A_wall_blocks_the_row_and_a_corner_touch_stays_clear()
    {
        var board = Box(0, 4, 0, 2);
        Wall(board, (1, 0));

        Assert.False(LineOfEffect.Between(board, 0, 0, 2, 0));
        Assert.True(LineOfEffect.Between(board, 0, 1, 2, 1));
        Assert.True(LineOfEffect.Between(board, 0, 1, 1, 0));
        Assert.True(LineOfEffect.Between(null, 0, 0, 2, 0));
    }

    [Fact]
    public void Burst_is_a_ring_and_drops_squares_behind_a_wall()
    {
        var ring = Area.Burst(0, 0, 1);
        Assert.Equal(8, ring.Count);
        Assert.Contains((1, 0), ring);
        Assert.DoesNotContain((0, 0), ring);
        Assert.DoesNotContain((2, 0), ring);
        Assert.Contains((2, 0), Area.Burst(0, 0, 2));

        var board = Box(0, 3, 0, 2);
        Wall(board, (0, 1));
        var seen = Area.Seen(board, 0, 0, Area.Burst(0, 0, 2));

        Assert.Contains((2, 0), seen);
        Assert.Contains((1, 0), seen);
        Assert.DoesNotContain((0, 2), seen);
        Assert.DoesNotContain((0, 1), seen);
    }

    [Fact]
    public void Cube_grows_away_from_the_origin()
    {
        Assert.True(Area.LegalCubeAnchor(0, 0, 1, 0, within: 1));
        Assert.False(Area.LegalCubeAnchor(0, 0, 0, 0, within: 1));
        Assert.False(Area.LegalCubeAnchor(0, 0, 2, 0, within: 1));
        Assert.Equal([(1, 0), (2, 0), (1, 1), (2, 1)], Area.Cube(0, 0, 1, 0, 2));
        Assert.Equal([(-1, 0), (-2, 0), (-1, 1), (-2, 1)], Area.Cube(0, 0, -1, 0, 2));
    }

    [Fact]
    public void Shrinking_the_board_drops_walls_and_starts_outside_it()
    {
        var board = Board.Blank("yard", "Yard");
        board.Walls.Add(new Square(11, 7));
        board.MaxX = 5;
        board.MaxY = 4;

        board.DropOutside();

        Assert.DoesNotContain(board.Walls, square => square.X == 11);
        Assert.DoesNotContain(board.SideB, square => square.Y == 6);
        Assert.Contains(board.SideA, square => square.X == 0 && square.Y == 1);
    }

    private static Board Box(int minX, int maxX, int minY, int maxY) => new()
    {
        MinX = minX,
        MaxX = maxX,
        MinY = minY,
        MaxY = maxY,
    };

    private static void Wall(Board board, params (int X, int Y)[] squares)
    {
        foreach (var (x, y) in squares)
            board.Walls.Add(new Square(x, y));
    }
}
