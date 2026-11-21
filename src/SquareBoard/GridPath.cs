namespace SquareBoard;

/// <summary>Shortest paths. A diagonal step costs one square.</summary>
public static class GridPath
{
    /// <summary>Path including the start. Null when the goal is blocked or farther than <paramref name="budget"/>.</summary>
    public static List<(int X, int Y)>? Find(
        int x0,
        int y0,
        int x1,
        int y1,
        int budget,
        Func<int, int, bool> blocked)
    {
        if (x0 == x1 && y0 == y1)
            return [(x0, y0)];
        if (budget <= 0 || blocked(x1, y1))
            return null;

        var prev = new Dictionary<(int X, int Y), (int X, int Y)>();
        var stepsAt = new Dictionary<(int X, int Y), int> { [(x0, y0)] = 0 };
        var queue = new Queue<(int X, int Y)>();
        queue.Enqueue((x0, y0));
        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            var steps = stepsAt[(cx, cy)];
            if (steps >= budget)
                continue;
            foreach (var (nx, ny) in Neighbors(cx, cy, x1, y1))
            {
                if (stepsAt.ContainsKey((nx, ny)) || blocked(nx, ny))
                    continue;
                stepsAt[(nx, ny)] = steps + 1;
                prev[(nx, ny)] = (cx, cy);
                if (nx == x1 && ny == y1)
                    return Rebuild(prev, x0, y0, x1, y1);
                queue.Enqueue((nx, ny));
            }
        }

        return null;
    }

    /// <summary>Every square reachable in at most <paramref name="budget"/> steps, including the start.</summary>
    public static HashSet<(int X, int Y)> Reachable(int x, int y, int budget, Func<int, int, bool> blocked)
    {
        var seen = new HashSet<(int X, int Y)> { (x, y) };
        var queue = new Queue<(int X, int Y, int Steps)>();
        queue.Enqueue((x, y, 0));
        while (queue.Count > 0)
        {
            var (cx, cy, steps) = queue.Dequeue();
            if (steps >= budget)
                continue;
            foreach (var (nx, ny) in Neighbors(cx, cy, cx, cy))
            {
                if (!seen.Add((nx, ny)))
                    continue;
                if (blocked(nx, ny))
                {
                    seen.Remove((nx, ny));
                    continue;
                }

                queue.Enqueue((nx, ny, steps + 1));
            }
        }

        return seen;
    }

    /// <summary>
    /// Walk up to <paramref name="budget"/> steps toward the target.
    /// Stops <paramref name="stopAt"/> squares away and will step sideways around a blocked square.
    /// The path includes the start. On an open grid it follows the straight line.
    /// </summary>
    public static List<(int X, int Y)> Approach(
        int x,
        int y,
        int targetX,
        int targetY,
        int budget,
        int stopAt,
        Func<int, int, bool> blocked)
    {
        var startDist = Squares.Distance(x, y, targetX, targetY);
        if (budget <= 0 || startDist <= stopAt)
            return [(x, y)];

        var ideal = Walk(x, y, targetX, targetY, Math.Min(budget, startDist - stopAt));
        (int X, int Y, int Dist, int Steps, int Ideal)? best = null;
        var prev = new Dictionary<(int X, int Y), (int X, int Y)>();
        var stepsAt = new Dictionary<(int X, int Y), int> { [(x, y)] = 0 };
        var queue = new Queue<(int X, int Y)>();
        queue.Enqueue((x, y));

        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            var steps = stepsAt[(cx, cy)];
            if (steps >= budget)
                continue;

            foreach (var (nx, ny) in Neighbors(cx, cy, targetX, targetY))
            {
                if (stepsAt.ContainsKey((nx, ny)) || blocked(nx, ny))
                    continue;
                var dist = Squares.Distance(nx, ny, targetX, targetY);
                if (dist < stopAt)
                    continue;

                var nextSteps = steps + 1;
                stepsAt[(nx, ny)] = nextSteps;
                prev[(nx, ny)] = (cx, cy);
                queue.Enqueue((nx, ny));
                if (dist >= startDist)
                    continue;

                var idealDist = Squares.Distance(nx, ny, ideal.X, ideal.Y);
                if (best is null
                    || dist < best.Value.Dist
                    || (dist == best.Value.Dist && nextSteps < best.Value.Steps)
                    || (dist == best.Value.Dist && nextSteps == best.Value.Steps && idealDist < best.Value.Ideal)
                    || (dist == best.Value.Dist && nextSteps == best.Value.Steps && idealDist == best.Value.Ideal
                        && (nx < best.Value.X || (nx == best.Value.X && ny < best.Value.Y))))
                {
                    best = (nx, ny, dist, nextSteps, idealDist);
                }
            }
        }

        if (best is null)
            return [(x, y)];
        return Rebuild(prev, x, y, best.Value.X, best.Value.Y);
    }

    /// <summary>
    /// Fewest steps to a square <paramref name="accept"/> allows.
    /// The path includes the start. When none is in budget, the path is only the start.
    /// </summary>
    public static List<(int X, int Y)> Seek(
        int x,
        int y,
        int budget,
        Func<int, int, bool> blocked,
        Func<int, int, bool> accept)
    {
        if (budget <= 0 || accept(x, y))
            return [(x, y)];

        var prev = new Dictionary<(int X, int Y), (int X, int Y)>();
        var stepsAt = new Dictionary<(int X, int Y), int> { [(x, y)] = 0 };
        var queue = new Queue<(int X, int Y)>();
        queue.Enqueue((x, y));
        (int X, int Y, int Steps)? best = null;

        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            var steps = stepsAt[(cx, cy)];
            if (steps >= budget || (best is not null && steps >= best.Value.Steps))
                continue;

            foreach (var (nx, ny) in Neighbors(cx, cy, cx, cy))
            {
                if (stepsAt.ContainsKey((nx, ny)) || blocked(nx, ny))
                    continue;

                var nextSteps = steps + 1;
                stepsAt[(nx, ny)] = nextSteps;
                prev[(nx, ny)] = (cx, cy);
                if (accept(nx, ny)
                    && (best is null
                        || nextSteps < best.Value.Steps
                        || (nextSteps == best.Value.Steps && (nx < best.Value.X || (nx == best.Value.X && ny < best.Value.Y)))))
                    best = (nx, ny, nextSteps);

                if (best is null || nextSteps < best.Value.Steps)
                    queue.Enqueue((nx, ny));
            }
        }

        return best is null ? [(x, y)] : Rebuild(prev, x, y, best.Value.X, best.Value.Y);
    }

    private static (int X, int Y) Walk(int x, int y, int targetX, int targetY, int steps)
    {
        for (var i = 0; i < steps; i++)
            (x, y) = Squares.StepToward(x, y, targetX, targetY);
        return (x, y);
    }

    private static List<(int X, int Y)> Rebuild(
        Dictionary<(int X, int Y), (int X, int Y)> prev,
        int x0,
        int y0,
        int x1,
        int y1)
    {
        var path = new List<(int X, int Y)> { (x1, y1) };
        var cursor = (x1, y1);
        while (cursor != (x0, y0))
        {
            cursor = prev[cursor];
            path.Add(cursor);
            if (path.Count > 512)
                break;
        }

        path.Reverse();
        return path;
    }

    /// <summary>Eight neighbors. The step toward the preference is first so an open grid stays straight.</summary>
    private static IEnumerable<(int X, int Y)> Neighbors(int x, int y, int towardX, int towardY)
    {
        var prefer = towardX != x || towardY != y;
        var px = x + Math.Sign(towardX - x);
        var py = y + Math.Sign(towardY - y);
        if (prefer && (px != x || py != y))
            yield return (px, py);

        for (var dy = -1; dy <= 1; dy++)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0)
                    continue;
                var nx = x + dx;
                var ny = y + dy;
                if (prefer && nx == px && ny == py)
                    continue;
                yield return (nx, ny);
            }
        }
    }
}
