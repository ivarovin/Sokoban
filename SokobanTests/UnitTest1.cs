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

    [Test]
    public void Push_Box()
    {
        var sut = new Sokoban((0, 0), targets: new[] { (0, 0) }, boxes: new[] { (1, 0) })
            .MoveTowards((1, 0));

        sut.WherePlayerIs.Should().Be((1, 0));
        sut.Boxes.First().Should().Be((2, 0));
    }

    [Test]
    public void PushBox_IsNotPossible_IfThereIsAnotherBehind()
    {
        new Sokoban((0, 0), targets: new[] { (0, 0), (1, 0) }, boxes: new[] { (1, 0), (2, 0) })
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((0, 0));
    }
}