namespace SokobanTests;

public readonly struct PlayerLinearMovement : Movement
{
    public readonly Position from;
    public readonly Position to;

    public PlayerLinearMovement(Position from, Position to)
    {
        this.from = from;
        this.to = to;
    }
    
    public static PlayerLinearMovement Between(Position from, Position to) => new PlayerLinearMovement(from, to);
}