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
}

public class Sokoban
{
    public bool IsSolved { get; set; }
    public Sokoban((int, int)[] valueTuples, (int, int)[] valueTuples1)
    {
        
    }

}