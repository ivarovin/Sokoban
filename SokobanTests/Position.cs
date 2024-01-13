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