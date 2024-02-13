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

    Sokoban target_state = Sokoban.FromAscii(@"
        ####..
        #.O#..
        #..###
        #@P..#
        #..*.#
        #..###
        ####..
    ");
    Sokoban source_state;

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

    (Key[], PlayerInput)[] keymap = new[]
    {
        (new[] { Key.Left, Key.A }, PlayerInput.Left),
        (new[] { Key.Right, Key.D }, PlayerInput.Right),
        (new[] { Key.Up, Key.W }, PlayerInput.Up),
        (new[] { Key.Down, Key.S }, PlayerInput.Down),
        (new[] { Key.Z }, PlayerInput.Undo),
        (new[] { Key.R }, PlayerInput.Restart),
    };

    public Game()
    {
        source_state = target_state;
    }

    void DrawTile(Texture texture, Vector2 position)
    {
        Engine.DrawTexture(texture, position * tile_size + offset, size: new Vector2(tile_size, tile_size));
    }

    private void RegisterInput() {
        foreach (var (keys, action) in keymap)
        {
            foreach (Key key in keys)
                if (Engine.GetKeyDown(key))
                {
                    pendingInputs.Enqueue(action);
                }
        }
    }

    public void Update()
    {
        UpdateTime();
        RenderGame();
        RegisterInput();

        if (turn_progress == 1f && pendingInputs.Count > 0)
        {
            var playerInput = pendingInputs.Dequeue();

            source_state = target_state;
            switch (playerInput)
            {
                case PlayerInput.Undo:
                    target_state = target_state.Undo();
                    turn_progress = 0f;
                    break;
                case PlayerInput.Restart:
                    target_state = target_state.Restart();
                    turn_progress = 0f;
                    break;
                case PlayerInput.Left:
                case PlayerInput.Right:
                case PlayerInput.Up:
                case PlayerInput.Down:
                    Vector2 moveDirection = this.DirectionFromInput(playerInput);
                    target_state = target_state.MoveTowards(((int)moveDirection.X, (int)moveDirection.Y));
                    turn_progress = 0f;
                    break;
            }
        }
    }

    void RenderGame()
    {
        RenderLevelGeometry();
        RenderBoxes();
        RenderPlayer();
    }

    void RenderPlayer()
    {
        if (target_state.WherePlayerIs.Equals(source_state.WherePlayerIs))
        {
            if (target_state.LastPlayerDirection.Equals((Direction)(0, 0)))
            {
                // initial state
                DrawTile(textures.player, target_state.WherePlayerIs.ToVector2());
            }
            else
            {
                // Wall bump
                DrawTile(textures.player, target_state.WherePlayerIs.ToVector2()
                                          + .2f * target_state.LastPlayerDirection.ToVector2() *
                                          (.5f - MathF.Abs(turn_progress - .5f)));
            }
        }
        else
        {
            // linear move
            DrawTile(textures.player,
                Vector2.Lerp(source_state.WherePlayerIs.ToVector2(), target_state.WherePlayerIs.ToVector2(),
                    turn_progress));
            // boxes move
        }
    }

    void UpdateTime()
    {
        turn_progress += (Engine.TimeDelta / turn_duration) * MathF.Pow(2f, pendingInputs.Count);
        turn_progress = Utils.Clamp01(turn_progress);
    }

    void RenderBoxes()
    {
        for (int i = 0; i < target_state.Boxes.Length; i++)
        {
            RenderBox(i);
        }
    }

    void RenderLevelGeometry()
    {
        for (int y = 0; y < target_state.LevelSize.y; y++)
        {
            for (int x = 0; x < target_state.LevelSize.x; x++)
            {
                var pos = new Vector2(x, y);
                DrawTile(textures.floor, pos);
                if (target_state.Walls.Contains((Position)(x, y)))
                    DrawTile(textures.wall, pos);
                if (target_state.Targets.Contains((Position)(x, y)))
                    DrawTile(textures.target, pos);
            }
        }
    }

    private void RenderBox(int index) => DrawTile(textures.crate, BoxPosition(index));

    private Vector2 BoxPosition(int index)
        => BoxRemainsAtSamePosition(index)
            ? target_state.Boxes[index].ToVector2()
            : Vector2.Lerp(source_state.Boxes[index].ToVector2(), target_state.Boxes[index].ToVector2(),
                turn_progress);

    bool BoxRemainsAtSamePosition(int index) => target_state.Boxes[index].Equals(source_state.Boxes[index]);

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