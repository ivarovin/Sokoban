namespace SokobanTests;

public readonly struct Movement
{
    public readonly Position from;
    public readonly Position to;

    public Movement(Position from, Position to)
    {
        this.from = from;
        this.to = to;
    }
    
    public static Movement Between(Position from, Position to) => new Movement(from, to);
}