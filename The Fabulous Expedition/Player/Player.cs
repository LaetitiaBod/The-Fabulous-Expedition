using Raylib_cs;
using System.Numerics;
using TiledSharp;
using static Program;
using static Raylib_cs.Raylib;

public class Player : Entity
{
    private GraphicsManager graphicsManager;
	public float moveSpeed = 300;
    public float foodMax { get; private set; } = 100f;
    public float currentFood = 100f;
    public float movementCost = 0f;
	public Dictionary<Vector2, TileType> movements = new Dictionary<Vector2, TileType>();

	public PlayerStateMachine stateMachine { get; private set; }
	public PlayerIdleState idleState { get; private set; }
	public PlayerMoveState moveState { get; private set; }
	public PlayerEncounterState encounterState { get; private set; }

	public Player(Animator _anim, Vector2 _position) : base(_anim, _position)
    {
        stateMachine = new PlayerStateMachine();
        graphicsManager = ServiceLocator.GetService<GraphicsManager>();

		idleState = new PlayerIdleState(this, stateMachine, graphicsManager.Idle());
		moveState = new PlayerMoveState(this, stateMachine, graphicsManager.Move());
        encounterState = new PlayerEncounterState(this, stateMachine, graphicsManager.Idle());
    }

    public override void Show()
    {
        base.Show();

		stateMachine.Initialize(idleState);
	}

    public override void Update()
    {
        base.Update();

        if (isDestroyed)
            return;

        if(!IsMouseButtonDown(MouseButton.Right))
			ServiceLocator.GetService<GameManager>().camera.Target = position;

        ServiceLocator.GetService<DebugManager>().AddOption("food", currentFood.ToString());
		stateMachine.currentState!.Update();
		anim.Update();

        hitbox = new Rectangle(
            (int)position.X - anim.widthFrame / 2 * scale,
            (int)position.Y - anim.texture.Height / 2 * scale,
            anim.widthFrame * scale,
            anim.texture.Height * scale
        );
    }

    public override void Draw()
    {
        base.Draw();

		if (isDestroyed)
            return;

        DrawTexturePro(
            anim.texture,
            anim.rectFrame,
            new Rectangle(position.X, position.Y, anim.widthFrame * scale, anim.texture.Height * scale),
            new Vector2(anim.widthFrame / 2 * scale, anim.texture.Height / 2 * scale), 0f, Color.White
        );

		if (stateMachine.currentState != null)
			stateMachine.currentState.Draw();

#if DEBUG
		ServiceLocator.GetService<DebugManager>().AddOption("pos x", position.X);
		ServiceLocator.GetService<DebugManager>().AddOption("pos y", position.Y);

        DrawRectangleLines((int)hitbox.X,(int) hitbox.Y, (int)hitbox.Width, (int)hitbox.Height,Color.Green);
#endif
    }

    public override void Hide()
    {
        base.Hide();
	}

    public override void Close()
    {
        base.Close();
	}
}
