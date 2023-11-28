namespace SokobanTests;

public class Sokoban
{
    public readonly (int, int)[] Targets;
    public readonly (int, int)[] Boxes;
    private readonly Sokoban previous;

    public bool IsSolved => Targets.All(t => Boxes.Contains(t));
    public (int x, int y) WherePlayerIs { get; }

    public Sokoban((int, int) wherePlayerIs, (int, int)[] targets, (int, int)[] boxes, Sokoban previous)
        : this(wherePlayerIs, targets, boxes)
    {
        this.previous = previous;
    }

    public Sokoban((int, int) wherePlayerIs, (int, int)[] targets, (int, int)[] boxes)
        : this(targets, boxes)
    {
        if (boxes.Contains(wherePlayerIs))
            throw new ArgumentException("Player cannot be in a box");

        WherePlayerIs = wherePlayerIs;
    }

    public Sokoban((int, int)[] targets, (int, int)[] boxes)
    {
        if (targets.Length != boxes.Length)
            throw new ArgumentException("Targets and boxes must have the same length");
        if (targets.Distinct().Count() != targets.Length)
            throw new ArgumentException("Targets must be unique");
        if (boxes.Distinct().Count() != boxes.Length)
            throw new ArgumentException("Boxes must be unique");

        this.Targets = targets;
        this.Boxes = boxes;
    }

    public Sokoban MoveTowards((int x, int y) direction)
    {
        return IsBoxAt((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y))
            ? PushBoxTowards(direction)
            : new Sokoban((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y), Targets, Boxes, this);
    }

    bool IsBoxAt((int x, int y) position) => Boxes.Contains((position.x, position.y));

    Sokoban PushBoxTowards((int x, int y) direction)
    {
        if (IsBoxAt((WherePlayerIs.x + direction.x * 2, WherePlayerIs.y + direction.y * 2)))
            return this;

        var boxIndex = Array.IndexOf(Boxes, (WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y));
        var newBoxes = Boxes.ToArray();
        newBoxes[boxIndex] = (WherePlayerIs.x + direction.x * 2, WherePlayerIs.y + direction.y * 2);
        return new Sokoban((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y), Targets, newBoxes, this);
    }

    public Sokoban Undo()
    {
        return previous;
    }

}