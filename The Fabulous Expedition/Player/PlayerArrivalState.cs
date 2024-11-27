using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class PlayerArrivalState : PlayerState
{
	private Map map = ServiceLocator.GetService<Map>();

	public Vector2 destination;
	private List<Vector2> movements;

	private float moveSpeed = 200;

	public PlayerArrivalState(Player _player, PlayerStateMachine _stateMachine, Animator _anim) : base(_player, _stateMachine, _anim)
	{
		destination = player.startPosition;
		movements = new List<Vector2>()
		{
			new Vector2(29,19),new Vector2(29,20),new Vector2(29,21), new Vector2(30,21), new Vector2(30,22),new Vector2(30,23),new Vector2(30,24)
		};
	}

	public override void Show()
	{
		base.Show();

		player.anim = ServiceLocator.GetService<GraphicsManager>().Boat();
		map = ServiceLocator.GetService<Map>();

        for (int i = 0; i < movements.Count; i++)
        {
			movements[i] = player.ConvertMapToPixelPosition(movements[i]);
		}
    }

	public override void Update()
	{
		base.Update();

		// when the node is reach, remove the node
		if (destination == player.position)
		{
			movements.Remove(destination);

			// if we are on the final destination, idle
			if (movements.Count <= 0)
			{
				stateMachine.ChangeState(player.idleState);
				return;
			}
		}

		destination = movements.First();

		// move to the node
		player.position = player.moveState.MoveTowards(player.position, destination, moveSpeed * GetFrameTime());

		//reveal close encounters
		foreach (Encounter encounter in map.encounterList)
		{
			Vector2 distance = Vector2.Subtract(encounter.coords, player.ConvertPixelToMapPosition(player.position));
			if ((Math.Abs(distance.X) + Math.Abs(distance.Y)) < 4)
				encounter.isRevealed = true;
		}

	}
	public override void Draw()
	{
		base.Draw();

	}

	public override void Close()
	{
		base.Close();
		player.movements.Clear();
	}
}