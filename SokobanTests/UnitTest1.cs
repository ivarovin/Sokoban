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
    public void SimpleLevel()
    {
        Sokoban.FromAscii(@"
            #####
            #P*O#
            #####
        ")
            .MoveTowards((1,0))
            .IsSolved.Should().BeTrue();

    }

    [Test]
    public void SampleLevel()
    {
        (int, int) right = (1,0);
        (int, int) left = (-1,0);
        (int, int) down = (0,1);
        (int, int) up = (0,-1);

        // Microban 1, by David Skinner
        var sut = Sokoban.FromAscii(@"
            ####..
            #.O#..
            #..###
            #@P..#
            #..*.#
            #..###
            ####..
        ");

        var solution = new Position[] {
            down, left, up, 
            right, right, right, down, left, 
            up, left, left, down, down, right, up,
            left, up, right,
            up, up, left, down,
            right, down, down, right, right, up, left,
            down, left, up, up 
        };

        foreach (var move in solution)
        {
            sut.IsSolved.Should().BeFalse();     
            sut = sut.MoveTowards(move);
        }
        sut.IsSolved.Should().BeTrue();
    }

    [Test]
    public void Spawn_Player()
    {
        var sut = new Sokoban((1, 1));

        sut.WherePlayerIs.Should().Be((Position)(1, 1));
    }

    [Test]
    public void Move_Player()
    {
        new Sokoban((0, 0))
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((Position)(1, 0));
    }

    [Test]
    public void Retrieve_PreviousPlayerMovement()
    {
        new Sokoban((4, 4))
            .MoveTowards((1, 0))
            .PlayerMovement.Should().Be(Movement.Between((4,4), (5,4)));
    }

    [Test]
    public void Retrieve_MovedBoxes_FromLastTick()
    {
        //Obtener todas las cajas que se movieron en el último tick
    }

    [Test]
    public void Movement_IsNone_AtStart()
    {
        new Sokoban((5, 5))
            .PlayerMovement
            .Should().Be(Movement.Between((5, 5), (5, 5)));
    }

    [Test]
    public void LoseGame()
    {
        new Sokoban((10, 0), targets: new Position[] { (0, 0) }, boxes: new Position[] { (1, 0) }).IsSolved.Should().BeFalse();
        new Sokoban((10, 0), targets: new Position[] { (0, 0), (1, 0) }, boxes: new Position[] { (0, 0), (2, 0) }).IsSolved.Should().BeFalse();
    }

    [Test]
    public void SolveGame()
    {
        var sut = new Sokoban((10, 0), new Position[] { (0, 0) }, new Position[] { (0, 0) });

        sut.IsSolved.Should().BeTrue();
    }

    [Test]
    public void Push_Box()
    {
        var sut = new Sokoban((0, 0), targets: new Position[] { (0, 0) }, boxes: new Position[] { (1, 0) })
            .MoveTowards((1, 0));

        sut.WherePlayerIs.Should().Be((Position)(1, 0));
        sut.Boxes.First().Should().Be((Position)(2, 0));
    }

    [Test]
    public void PushBox_IsNotPossible_IfThereIsAnotherBehind()
    {
        new Sokoban((0, 0), targets: new Position[] { (0, 0), (1, 0) }, boxes: new Position[] { (1, 0), (2, 0) })
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((Position)(0, 0));
    }

    [Test]
    public void Undo()
    {
        new Sokoban((0, 0), targets: new Position[] { (0, 0) }, boxes: new Position[] { (2, 0) })
            .MoveTowards((1, 0))
            .Undo()
            .WherePlayerIs.Should().Be((Position)(0, 0));
    }

    [Test]
    public void UndoFirst()
    {
        new Sokoban((0, 0), targets: new Position[] { (0, 0) }, boxes: new Position[] { (2, 0) })
            .Undo()
            .Should().BeNull();
    }

    [Test]
    public void Restart()
    {
        new Sokoban((0, 0), targets: new Position[] { (0, 0) }, boxes: new Position[] { (2, 0) })
            .MoveTowards((1, 0))
            .MoveTowards((1, 0))
            .MoveTowards((1, 0))
            .Restart()
            .WherePlayerIs.Should().Be((Position)(0, 0));
    }

    [Test]
    public void RestartFirst()
    {
        new Sokoban((0, 0), targets: new Position[] { (0, 0) }, boxes: new Position[] { (2, 0) })
            .Restart()
            .WherePlayerIs.Should().Be((Position)(0, 0));
    }

    [Test]
    public void CantWalkIntoWall()
    {
        new Sokoban(wherePlayerIs: (0, 0), targets: new Position[] { (1, 1) }, boxes: new Position[] { (1, 1) }, walls: new Position[] { (1, 0) })
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((Position)(0, 0));
    }

    [Test]
    public void CantPushIntoWall()
    {
        new Sokoban(wherePlayerIs: (0, 0), targets: new Position[] { (1, 1) }, boxes: new Position[] { (1, 0) }, walls: new Position[] { (2, 0) })
            .MoveTowards((1, 0))
            .WherePlayerIs.Should().Be((Position)(0, 0));
    }

    [Test]
    public void FindAsciiCoordinates()
    {
        string asciiRectangle = @"
            A....
            .BBB.
            .....
        ";

        Utils.FindCharactersCoordinates(asciiRectangle, "A").Should().BeEquivalentTo(new Position[] { (0, 0) });
        Utils.FindCharactersCoordinates(asciiRectangle, "B").Should().BeEquivalentTo(new Position[] { (1, 1), (2, 1), (3, 1) });
        Utils.FindCharactersCoordinates(asciiRectangle, "AB").Should().BeEquivalentTo(new Position[] { (0,0), (1, 1), (2, 1), (3, 1) });
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

    [Test]
    public void FromAscii()
    {
        var sut = Sokoban.FromAscii(@"
            .#P
            *O@
        ");

        sut.WherePlayerIs.Should().Be((Position)(2, 0));
        sut.Walls.Should().BeEquivalentTo(new Position[] {(1,0)});
        sut.Boxes.Should().BeEquivalentTo(new Position[] {(0,1), (2,1)});
        sut.Targets.Should().BeEquivalentTo(new Position[] {(1,1), (2,1)});
    }

    [Test]
    public void LevelSize()
    {
        new Sokoban((0,0), targets: new Position[]{(10, 0)}, boxes: new Position[]{(0, 10)})
            .LevelSize.Should().Be((Position)(11, 11));
    }
}