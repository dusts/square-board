# Square board

A square grid where a diagonal step costs the same as a straight one. Walls block movement and lines of effect. A line that only touches a corner stays clear. A ring or a block drops any square it cannot see.

## Layout

| Path | What it is |
|------|------------|
| `src/SquareBoard` | The board, paths, line of effect, and areas. |
| `src/SquareBoard.Painter` | A page for painting walls and two start zones. |
| `tests/SquareBoard.Tests` | The geometry checks. |

A null board is an open field. A `Board` adds edges, walls, side A, and side B.

## Rules

- Distance is Chebyshev. One step, including a diagonal, costs 1.
- `GridPath` finds a path, lists reachable squares, walks toward a target and stops short, and seeks the first square a predicate accepts. It steps around blocked squares.
- `LineOfEffect` draws from corner to corner. Overlapping a wall along an edge blocks. Touching only a corner does not.
- `Area.Burst` is a ring around a square and does not include that square. `Area.Cube` is a block that grows away from the origin. `Area.Seen` keeps the squares a line of effect can reach.

## Tests

```powershell
dotnet test tests/SquareBoard.Tests
```

## Painter

```powershell
dotnet run --project src/SquareBoard.Painter
```

Open the URL it prints. Floor, Wall, Side A, and Side B paint the grid. Look marks a square and outlines the ring that can see past the walls. Width and height stay between 6 and 20. Shrinking the board drops anything that falls outside.

## License

MIT. See [LICENSE](LICENSE).
