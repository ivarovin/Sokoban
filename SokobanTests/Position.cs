namespace SokobanTests;

public struct Position
{
    public readonly int x, y;

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator (int, int)(Position position) => (position.x, position.y);
    public static implicit operator Position((int, int) position) => new(position.Item1, position.Item2);
}

public struct Direction
{
    public readonly int dx, dy;

    public Direction(int dx, int dy)
    {
        this.dx = dx;
        this.dy = dy;
    }

    public static implicit operator (int, int)(Direction direction) => (direction.dx, direction.dy);
    public static implicit operator Direction((int, int) direction) => new(direction.Item1, direction.Item2);
}