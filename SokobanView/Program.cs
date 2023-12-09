using SokobanTests;

Console.WriteLine("Hello, World!");

var sokoban = Sokoban.FromAscii(@"
    ####..
    #.O#..
    #..###
    #@P..#
    #..*.#
    #..###
    ####..
");

while (!sokoban.IsSolved)
{
    Render(sokoban);

    var input = Console.ReadLine();
    if (input == "d")
        sokoban = sokoban.MoveTowards((1, 0));
    if (input == "a")
        sokoban = sokoban.MoveTowards((-1,0));
    if (input == "s")
        sokoban = sokoban.MoveTowards((0,1));
    if (input == "w")
        sokoban = sokoban.MoveTowards((0,-1));
    if (input == "z")
        sokoban = sokoban.Undo();
}

await ReplayBackwards(sokoban);

void Render(Sokoban sokoban1)
{
    Console.Clear();

    for (int y = 0; y < sokoban1.LevelSize.y; y++)
    {
        for (int x = 0; x < sokoban1.LevelSize.x; x++)
        {
            if (sokoban1.WherePlayerIs == (x, y))
                Console.Write("P");
            else if (sokoban1.Boxes.Contains((x, y)))
                Console.Write("B");
            else if (sokoban1.Targets.Contains((x, y)))
                Console.Write("T");
            else if (sokoban1.Walls.Contains((x, y)))
                Console.Write("#");
            else
                Console.Write(".");
        }

        Console.WriteLine();
    }

    Console.WriteLine();
}

async Task ReplayBackwards(Sokoban sokoban2)
{
    while (sokoban2 != null)
    {
        Render(sokoban2);
        sokoban2 = sokoban2.Undo();
        await Task.Delay(100);
    }
}