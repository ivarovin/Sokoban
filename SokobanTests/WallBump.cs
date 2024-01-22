namespace SokobanTests;

public readonly struct WallBump : DeltaState
{
    public readonly Position from;
    public readonly (int,int) MovingTowards;

    public WallBump(Position from, (int,int) movingTowards)
    {
        this.from = from;
        this.MovingTowards = movingTowards;
    }

    public static WallBump Crash(Position from, (int, int) direction) => new WallBump(from, direction);
}