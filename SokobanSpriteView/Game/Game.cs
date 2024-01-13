using SokobanTests;
using System;
using System.Collections.Generic;

static class PositionConversions
{
    public static Vector2 ToVector2(this Position p) => new Vector2(p.x, p.y);
}

class Game
{
    public static readonly string Title = "Sokoban";
    public static readonly Vector2 Resolution = new Vector2(512, 512);

    Sokoban cur_state = Sokoban.FromAscii(@"
        ####..
        #.O#..
        #..###
        #@P..#
        #..*.#
        #..###
        ####..
    ");

    float turn_duration = .2f;
    float tile_size = 64;
    Vector2 offset = Vector2.Zero;

    // Always in the [0, 1] range; 0 right after starting a turn, grows to 1 over the course of turn_duration seconds.
    float turn_progress = 1f;

    // Load some textures when the game starts:
    class Textures
    {
        public Texture player = Engine.LoadTexture("player.png");
        public Texture floor = Engine.LoadTexture("floor.png");
        public Texture wall = Engine.LoadTexture("wall.png");
        public Texture crate = Engine.LoadTexture("crate.png");
        public Texture target = Engine.LoadTexture("target.png");
    }

    readonly Textures textures = new Textures();


    public Game()
    {
    }

    void DrawTile(Texture texture, Vector2 position)
    {
        Engine.DrawTexture(texture, position * tile_size + offset, size: new Vector2(tile_size, tile_size));
    }

    public void Update()
    {
        for (int y = 0; y < cur_state.LevelSize.y; y++)
        {
            for (int x = 0; x < cur_state.LevelSize.x; x++)
            {
                var pos = new Vector2(x, y);
                DrawTile(textures.floor, pos);
                if (cur_state.Walls.Contains((Position)(x, y)))
                    DrawTile(textures.wall, pos);
                if (cur_state.Targets.Contains((Position)(x, y)))
                    DrawTile(textures.target, pos);
                if (cur_state.Boxes.Contains((Position)(x, y)))
                    DrawTile(textures.crate, pos);
            }
        }

        turn_progress += Engine.TimeDelta / turn_duration;
        turn_progress = Utils.Clamp01(turn_progress);

        DrawTile(textures.player, Vector2.Lerp(cur_state.PlayerMovement.from.ToVector2(), cur_state.PlayerMovement.to.ToVector2(), turn_progress));

        int dx = 0;
        int dy = 0;
        if (Engine.GetKeyDown(Key.Left))
        {
            dx = -1;
        }
        else if (Engine.GetKeyDown(Key.Right))
        {
            dx = 1;
        }
        else if (Engine.GetKeyDown(Key.Up))
        {
            dy = -1;
        }
        else if (Engine.GetKeyDown(Key.Down))
        {
            dy = 1;
        }

        if (dx != 0 || dy != 0)
        {
            cur_state = cur_state.MoveTowards((dx, dy));
            turn_progress = 0f;
        }

        if (Engine.GetKeyDown(Key.Z))
        {
            cur_state = cur_state.Undo();
        }
    }
}

class Utils
{

    public static float Clamp01(float value)
    {
        if (value < 0f) return 0f;
        if (value > 1f) return 1f;
        return value;
    }
}