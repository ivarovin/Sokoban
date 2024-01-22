namespace SokobanTests;

public readonly struct PlayerLinearMovement : DeltaState
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