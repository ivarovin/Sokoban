﻿using SokobanTests;
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

    float tile_size = 64;
    Vector2 offset = Vector2.Zero;

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

        DrawTile(textures.player, cur_state.WherePlayerIs.ToVector2());
        DrawTile(textures.player,Vector2.Lerp(cur_state.PlayerMovement.from, cur_state.PlayerMovement.to, TurnTime));

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
        }

        if (Engine.GetKeyDown(Key.Z))
        {
            cur_state = cur_state.Undo();
        }
    }
}