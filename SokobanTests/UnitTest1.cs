using FluentAssertions;

namespace SokobanTests;

// 1. El personaje se puede mover en las cuatro direcciones (arriba, abajo, izquierda, derecha)
// 2. El personaje no puede moverse a través de una pared
// 3. El personaje puede empujar una caja hacia la dirección en la que se mueve
// 4. El personaje no puede empujar una caja si hay una pared o una caja bloqueando el camino
// 5. Todas las cajas deben estar sobre un objetivo para ganar el juego

public class Tests
{
    [Test]
    public void LoseGame()
    {
        new Sokoban(targets: new[] { (0, 0) }, boxes: new[] { (1, 0) }).IsSolved.Should().BeFalse();
        new Sokoban(targets: new[] { (0, 0), (1, 0) }, boxes: new[] { (0, 0), (2, 0) }).IsSolved.Should().BeFalse();
    }

    [Test]
    public void SolveGame()
    {
        var sut = new Sokoban(new[] { (0, 0) }, new[] { (0, 0) });

        sut.IsSolved.Should().BeTrue();
    }

    [Test]
    public void Spawn_Player()
    {
        var sut = new Sokoban((1, 1), new[] { (0, 0) }, new[] { (0, 0) });

        sut.WherePlayerIs.Should().Be((1, 1));
    }

    [Test]
    public void Move_Player()
    {
        new Sokoban((0, 0), targets: new[] { (0, 0) }, boxes: new[] { (2, 0) })
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((1, 0));
    }
}

public class Sokoban
{
    readonly (int, int)[] targets;
    readonly (int, int)[] boxes;
    public bool IsSolved => targets.All(t => boxes.Contains(t));
    public (int x, int y) WherePlayerIs { get; }

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

        this.targets = targets;
        this.boxes = boxes;
    }

    public Sokoban MoveTowards((int x, int y) direction) 
        => new((WherePlayerIs.x + direction.x, WherePlayerIs.y + direction.y), targets, boxes);
}