using SokobanTests;
using System;
using System.Collections.Generic;

static class PositionConversions
{
    public static Vector2 ToVector2(this Position p) => new Vector2(p.x, p.y);
    public static Vector2 ToVector2(this Direction d) => new Vector2(d.dx, d.dy);
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

    // What can the player do?
    enum PlayerInput
    {
        Left,
        Right,
        Up,
        Down,
        Undo,
        Restart,
    };

    Queue<PlayerInput> pendingInputs = new Queue<PlayerInput>();

    (Key[], PlayerInput)[] keymap = new[] {
        (new[] {Key.Left, Key.A}, PlayerInput.Left),
        (new[] {Key.Right, Key.D}, PlayerInput.Right),
        (new[] {Key.Up, Key.W}, PlayerInput.Up),
        (new[] {Key.Down, Key.S}, PlayerInput.Down),
        (new[] {Key.Z}, PlayerInput.Undo),
        (new[] {Key.R}, PlayerInput.Restart),
    };

    public Game()
    {
    }

    void DrawTile(Texture texture, Vector2 position)
    {
        Engine.DrawTexture(texture, position * tile_size + offset, size: new Vector2(tile_size, tile_size));
    }

    public void Update()
    {
        // draw static level
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
                // if (cur_state.Boxes.Contains((Position)(x, y)))
                //     RenderBox(pos);
                //     // DrawTile(textures.crate, pos);
            }
        }

        for (int i = 0; i < cur_state.Boxes.Length; i++)
        {
            RenderBox(i)
        }

        turn_progress += (Engine.TimeDelta / turn_duration) * MathF.Pow(2f, pendingInputs.Count);
        turn_progress = Utils.Clamp01(turn_progress);

        if (cur_state.WherePlayerIs.Equals(cur_state.Undo().WherePlayerIs))
        {
            if (cur_state.LastPlayerDirection.Equals((Direction)(0, 0)))
            {
                // initial state
                DrawTile(textures.player, cur_state.WherePlayerIs.ToVector2());
            }
            else
            {
                // Wall bump
                DrawTile(textures.player, cur_state.WherePlayerIs.ToVector2()
                    + .2f * cur_state.LastPlayerDirection.ToVector2() * (.5f - MathF.Abs(turn_progress - .5f)));
            }
        }
        else
        {
            // linear move
            DrawTile(textures.player, Vector2.Lerp(cur_state.Undo().WherePlayerIs.ToVector2(), cur_state.WherePlayerIs.ToVector2(), turn_progress));
            // boxes move
        }

        foreach (var (keys, action) in keymap)
        {
            foreach (Key key in keys)

                if (Engine.GetKeyDown(key))
                {
                    pendingInputs.Enqueue(action);
                }
        }

        if (turn_progress == 1f && pendingInputs.Count > 0)
        {
            var playerInput = pendingInputs.Dequeue();

            switch (playerInput)
            {
                case PlayerInput.Undo:
                    cur_state = cur_state.Undo();
                    break;
                case PlayerInput.Restart:
                    cur_state = cur_state.Restart();
                    break;
                case PlayerInput.Left:
                case PlayerInput.Right:
                case PlayerInput.Up:
                case PlayerInput.Down:
                    Vector2 moveDirection = this.DirectionFromInput(playerInput);
                    cur_state = cur_state.MoveTowards(((int)moveDirection.X, (int)moveDirection.Y));
                    turn_progress = 0f;
                    break;
            }
        }
    }

    private RenderBox(int index) {
        if (cur_state.Boxes[index].Equals(cur_state.Undo().Boxes[index])) {
            DrawTile(textures.player, cur_state.WherePlayerIs.ToVector2());
        }
    }

    private Vector2 DirectionFromInput(PlayerInput playerInput)
    {

        switch (playerInput)
        {
            case PlayerInput.Left:
                return new Vector2(-1, 0);
            case PlayerInput.Right:
                return new Vector2(1, 0);
            case PlayerInput.Up:
                return new Vector2(0, -1);
            case PlayerInput.Down:
                return new Vector2(0, 1);
            case PlayerInput.Undo:
            case PlayerInput.Restart:
            default:
                throw new ArgumentException();
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