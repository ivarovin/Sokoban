namespace SokobanTests;

public readonly struct WallBump : Movement
{
    public readonly Position from;
    public readonly (int,int) MovingTowards;

    public WallBump(Position from, (int,int) movingTowards)
    {
        this.from = from;
        this.MovingTowards = movingTowards;
    }
}