namespace SokobanTests;

// command representing the diff between .previous state & .this one
public interface DeltaState
{
}

public readonly struct NewGame : DeltaState
{}