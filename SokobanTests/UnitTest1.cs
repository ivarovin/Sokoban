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
        var sut = new Sokoban(new[] { (0, 0) }, new[] { (1, 0) });
        
        sut.IsSolved.Should().BeFalse();
    }

    [Test]
    public void SolveGame()
    {
        var sut = new Sokoban(new[] { (0, 0) }, new[] { (0, 0) });
        
        sut.IsSolved.Should().BeTrue();
    }
}

public class Sokoban
{
    readonly (int, int)[] targets;
    readonly (int, int)[] boxes;
    public bool IsSolved => targets.All(t => boxes.Contains(t));
    
    public Sokoban((int, int)[] targets, (int, int)[] boxes)
    {
        this.targets = targets;
        this.boxes = boxes;
    }
}