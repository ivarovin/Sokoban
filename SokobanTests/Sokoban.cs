using FluentAssertions;

namespace SokobanTests;

public class Sokoban
{
    public readonly (int, int)[] Targets;
    public readonly (int, int)[] Boxes;
    private readonly Sokoban previous;
    public readonly (int, int)[] Walls;

    public bool IsSolved => Targets.All(t => Boxes.Contains(t));
    public (int x, int y) WherePlayerIs { get; }

    public Sokoban((int, int) wherePlayerIs, (int, int)[] targets, (int, int)[] boxes, (int, int)[] walls, Sokoban previous)
        : this(wherePlayerIs, targets, boxes, walls)
    {
        this.previous = previous;
    }

    public Sokoban((int, int) wherePlayerIs, (int, int)[] targets = null, (int, int)[] boxes = null, (int, int)[] walls = null)
    {
        targets ??= Array.Empty<(int, int)>();
        boxes ??= Array.Empty<(int, int)>();
        walls ??= Array.Empty<(int, int)>();

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
                ? this
                : new Sokoban((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y), Targets, Boxes, Walls, this);
    }

    bool IsWallAt((int x, int y) position) => Walls.Contains(position);

    bool IsBoxAt((int x, int y) position) => Boxes.Contains((position.x, position.y));

    Sokoban PushBoxTowards((int x, int y) direction)
    {
        if (IsWallAt((WherePlayerIs.x + direction.x * 2, WherePlayerIs.y + direction.y * 2)))
            return this;
        if (IsBoxAt((WherePlayerIs.x + direction.x * 2, WherePlayerIs.y + direction.y * 2)))
            return this;

        var boxIndex = Array.IndexOf(Boxes, (WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y));
        var newBoxes = Boxes.ToArray();
        newBoxes[boxIndex] = (WherePlayerIs.x + direction.x * 2, WherePlayerIs.y + direction.y * 2);
        return new Sokoban((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y), Targets, newBoxes, Walls, this);
    }

    public Sokoban Undo() => previous;

    public static Sokoban FromAscii(string ascii)
    {
        return new Sokoban(
            wherePlayerIs: Utils.SingleValue<(int,int)>(Utils.FindCharactersCoordinates(ascii, "Pp")),
            targets: Utils.FindCharactersCoordinates(ascii, "O@"),
            boxes: Utils.FindCharactersCoordinates(ascii, "*@"),
            walls: Utils.FindCharactersCoordinates(ascii, "#")
        );
    }
}

public class Utils
{
    public static (int, int)[] FindCharactersCoordinates(string asciiRectangle, string searchChars)
    {
        List<(int, int)> coordinates = new List<(int, int)>();

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

    public static T SingleValue<T>(T[] arr) {
        if (arr.Length != 1)
        {
            throw new ArgumentException("Array does not have exactly 1 element.");
        }
        return arr[0];
    }
}