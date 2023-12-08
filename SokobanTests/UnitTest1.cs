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
    public void Spawn_Player()
    {
        var sut = new Sokoban((1, 1));

        sut.WherePlayerIs.Should().Be((1, 1));
    }

    [Test]
    public void Move_Player()
    {
        new Sokoban((0, 0))
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((1, 0));
    }

    [Test]
    public void LoseGame()
    {
        new Sokoban((10, 0), targets: new[] { (0, 0) }, boxes: new[] { (1, 0) }).IsSolved.Should().BeFalse();
        new Sokoban((10, 0), targets: new[] { (0, 0), (1, 0) }, boxes: new[] { (0, 0), (2, 0) }).IsSolved.Should().BeFalse();
    }

    [Test]
    public void SolveGame()
    {
        var sut = new Sokoban((10, 0), new[] { (0, 0) }, new[] { (0, 0) });

        sut.IsSolved.Should().BeTrue();
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

    [Test]
    public void Undo()
    {
        new Sokoban((0, 0), targets: new[] { (0, 0) }, boxes: new[] { (2, 0) })
            .MoveTowards((1, 0))
            .Undo()
            .WherePlayerIs.Should().Be((0, 0));
    }

    [Test]
    public void UndoFirst()
    {
        new Sokoban((0, 0), targets: new[] { (0, 0) }, boxes: new[] { (2, 0) })
            .Undo()
            .Should().BeNull();
    }

    [Test]
    public void CantWalkIntoWall()
    {
        new Sokoban(wherePlayerIs: (0, 0), targets: new[] { (1, 1) }, boxes: new[] { (1, 1) }, walls: new[] { (1, 0) })
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((0, 0));
    }

    [Test]
    public void CantPushIntoWall()
    {
        new Sokoban(wherePlayerIs: (0, 0), targets: new[] { (1, 1) }, boxes: new[] { (1, 0) }, walls: new[] { (2, 0) })
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((0, 0));
    }

    [Test]
    public void FindAsciiCoordinates()
    {
        string asciiRectangle = @"
            A....
            .BBB.
            .....
        ";

        Utils.FindCharactersCoordinates(asciiRectangle, "A").Should().BeEquivalentTo(new[] { (0, 0) });
        Utils.FindCharactersCoordinates(asciiRectangle, "B").Should().BeEquivalentTo(new[] { (1, 1), (2, 1), (3, 1) });
        Utils.FindCharactersCoordinates(asciiRectangle, "AB").Should().BeEquivalentTo(new[] { (0,0), (1, 1), (2, 1), (3, 1) });
        Utils.FindCharactersCoordinates(asciiRectangle, "X").Should().BeEquivalentTo(Array.Empty<(int,int)>());
    }

    [Test]
    public void EnsuresRectangle()
    {
        string badRectangle = @"
            .....
            ..
        ";

        Action action = () => Utils.FindCharactersCoordinates(badRectangle, ".");
        action.Should().Throw<ArgumentException>().WithMessage("Input is not a rectangle.");
    }

    // [Test]
    // public void FromAscii()
    // {
    //     var sut = Sokoban.FromAscii(@"
    //         .#P
    //         *O@
    //     ");

    //     sut.WherePlayerIs.Should().Be((0, 0));
    // }
}

// ####..
// #.O#..
// #..###
// #@P..#
// #..*.#
// #..###
// ####..
