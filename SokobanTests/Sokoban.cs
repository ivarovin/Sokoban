namespace SokobanTests;

public class Sokoban
{
    public readonly Position[] Targets;
    public readonly Position[] Boxes;
    private readonly Sokoban previous;
    public readonly Position[] Walls;

    public bool IsSolved => Targets.All(t => Boxes.Contains(t));
    public Position WherePlayerIs { get; }
    public (int, int) LastPlayerDirection { get; }

    public (int x, int y) LevelSize
    {
        get
        {
            var allPoints = Targets.Concat(Boxes).Concat(Walls).Append(WherePlayerIs);
            return (1 + allPoints.Max(p => p.x), 1 + allPoints.Max(p => p.y));
        }
    }

    public Movement PlayerMove
    {
        get
        {
            if (previous == null) return PlayerLinearMovement.Between(WherePlayerIs, WherePlayerIs);
            if (previous.WherePlayerIs.Equals(WherePlayerIs)) return WallBump.Crash(WherePlayerIs, LastPlayerDirection);

            return PlayerLinearMovement.Between(previous.WherePlayerIs, WherePlayerIs);
        }
    }

    public Sokoban(Position wherePlayerIs, Position[] targets, Position[] boxes, Position[] walls,
        Sokoban previous, (int, int) lastPlayerDirection)
        : this(wherePlayerIs, targets, boxes, walls, previous)
    {
        this.LastPlayerDirection = lastPlayerDirection;
    }

    public Sokoban(Position wherePlayerIs, Position[] targets, Position[] boxes, Position[] walls,
        Sokoban previous)
        : this(wherePlayerIs, targets, boxes, walls)
    {
        this.previous = previous;
    }

    public Sokoban(Position wherePlayerIs, Position[] targets = null, Position[] boxes = null,
        Position[] walls = null)
    {
        targets ??= Array.Empty<Position>();
        boxes ??= Array.Empty<Position>();
        walls ??= Array.Empty<Position>();

        if (walls.Contains(wherePlayerIs))
            throw new ArgumentException("Player cannot be in a wall");
        if (boxes.Contains(wherePlayerIs))
            throw new ArgumentException("Player cannot be in a box");
        if (targets.Length != boxes.Length)
            throw new ArgumentException("Targets and boxes must have the same length");
        if (targets.Distinct().Count() != targets.Length)
            throw new ArgumentException("Targets must be unique");
        if (boxes.Distinct().Count() != boxes.Length)
            throw new ArgumentException("Boxes must be unique");

        this.WherePlayerIs = wherePlayerIs;
        this.Walls = walls;
        this.Targets = targets;
        this.Boxes = boxes;
    }

    public Sokoban MoveTowards((int x, int y) direction)
    {
        return IsBoxAt((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y))
            ? PushBoxTowards(direction)
            : IsWallAt((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y))
                ? new Sokoban(WherePlayerIs, Targets, Boxes, Walls, this, direction)
                : new Sokoban((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y), Targets, Boxes, Walls,
                    this, direction);
    }

    bool IsWallAt(Position position) => Walls.Contains(position);

    bool IsBoxAt(Position position) => Boxes.Contains((position.x, position.y));

    Sokoban PushBoxTowards((int x, int y) direction)
    {
        if (IsWallAt((WherePlayerIs.x + direction.x * 2, WherePlayerIs.y + direction.y * 2)) ||
            IsBoxAt((WherePlayerIs.x + direction.x * 2, WherePlayerIs.y + direction.y * 2)))
            return this;

        var boxIndex = Array.IndexOf(Boxes, (WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y));
        var newBoxes = Boxes.ToArray();
        newBoxes[boxIndex] = (WherePlayerIs.x + direction.x * 2, WherePlayerIs.y + direction.y * 2);
        return new Sokoban((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y), Targets, newBoxes, Walls,
            this, direction);
    }

    public Sokoban Undo() => this.previous == null ? this : previous;
    public Sokoban Restart() => this.previous == null ? this : this.previous.Restart();

    public static Sokoban FromAscii(string ascii)
    {
        return new Sokoban(
            wherePlayerIs: Utils.SingleValue<Position>(Utils.FindCharactersCoordinates(ascii, "Pp")),
            targets: Utils.FindCharactersCoordinates(ascii, "O@"),
            boxes: Utils.FindCharactersCoordinates(ascii, "*@"),
            walls: Utils.FindCharactersCoordinates(ascii, "#")
        );
    }
}

public class Utils
{
    public static Position[] FindCharactersCoordinates(string asciiRectangle, string searchChars)
    {
        List<Position> coordinates = new List<Position>();

        string[] lines = asciiRectangle.Trim().Split('\n');

        int width = -1; // Initializing width to an impossible value

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim(); // Trim each line

            if (width == -1)
            {
                width = lines[i].Length; // Set width for the first line
            }
            else if (lines[i].Length != width)
            {
                throw new ArgumentException("Input is not a rectangle.");
            }

            for (int j = 0; j < lines[i].Length; j++)
            {
                if (searchChars.Contains(lines[i][j]))
                {
                    coordinates.Add((j, i)); // (x, y) coordinates
                }
            }
        }

        return coordinates.ToArray();
    }

    public static T SingleValue<T>(T[] arr)
    {
        if (arr.Length != 1)
        {
            throw new ArgumentException("Array does not have exactly 1 element.");
        }

        return arr[0];
    }
}