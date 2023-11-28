using SokobanTests;

Console.WriteLine("Hello, World!");

var sokoban = new Sokoban(targets: new[] { (2, 0) }, boxes: new[] { (1, 0) });

while (!sokoban.IsSolved)
{
    Render(sokoban);

    var input = Console.ReadLine();
    if (input == "d")
        sokoban = sokoban.MoveTowards((1, 0));
}

Render(sokoban);



void Render(Sokoban sokoban1)
{
    Console.Clear();

    for (int y = 0; y < 5; y++)
    {
        for (int x = 0; x < 5; x++)
        {
            if (sokoban1.WherePlayerIs == (x, y))
                Console.Write("P");
            else if (sokoban1.Boxes.Contains((x, y)))
                Console.Write("B");
            else if (sokoban1.Targets.Contains((x, y)))
                Console.Write("T");
            else
                Console.Write(" ");
        }

        Console.WriteLine();
    }
}