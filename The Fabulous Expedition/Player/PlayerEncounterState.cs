using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class PlayerEncounterState : PlayerState
{
	private GraphicsManager graphicsManager;
	private GameManager gameManager = ServiceLocator.GetService<GameManager>();
	private AudioManager audioManager;

	public Encounter? currentEncounter;

	public PlayerEncounterState(Player _player, PlayerStateMachine _stateMachine, Animator _anim) : base(_player, _stateMachine, _anim)
	{
	}

	public override void Show()
	{
		base.Show();

		player.anim = ServiceLocator.GetService<GraphicsManager>().Idle();

		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		audioManager = ServiceLocator.GetService<AudioManager>();

		if (currentEncounter != null)
			currentEncounter.Show();
	}

	public override void Update()
	{
		base.Update();

		if (currentEncounter != null)
			currentEncounter.Update();
	}
	public override void Draw()
	{
		base.Draw();

		if (currentEncounter != null)
			currentEncounter.Draw();
	}

	public override void Close()
	{
		base.Close();

		if (currentEncounter != null)
			currentEncounter.Close();
	}
}