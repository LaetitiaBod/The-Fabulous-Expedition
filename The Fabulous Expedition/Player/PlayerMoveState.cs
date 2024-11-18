using System.Numerics;
using static Raylib_cs.Raylib;

public class PlayerMoveState : PlayerState
{
	private Vector2 destination;
	private Map map = ServiceLocator.GetService<Map>();

	public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, Animator _anim) : base(_player, _stateMachine, _anim)
	{
	}

	public override void Show()
	{
		base.Show();

		player.anim = ServiceLocator.GetService<GraphicsManager>().Move();
		map = ServiceLocator.GetService<Map>();

		// initialize the next node to reach
		destination = player.movements.Keys.Last<Vector2>();
	}

	public override void Update()
	{
		base.Update();

		// when the node is reach, remove the node
		if (destination == player.position)
		{
			player.movements.Remove(destination);

			// if we are on the final destination, idle
			if (player.movements.Keys.Count <= 0)
			{
				stateMachine.ChangeState(player.idleState);
				return;
			}
		}

		destination = player.movements.Keys.Last<Vector2>();

		// move to the node
		player.position = MoveTowards(player.position, destination, player.moveSpeed * GetFrameTime());

		//reveal close encounters
		foreach (Encounter encounter in map.encounterList)
		{
			Vector2 distance = Vector2.Subtract(encounter.coords, player.ConvertPixelToMapPosition(player.position));
			if ((Math.Abs(distance.X) + Math.Abs(distance.Y)) < 5)
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

	// copied from the method MoveTowards from Unity
	public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
	{
		float XOffset = target.X - current.X;
		float YOffset = target.Y - current.Y;
		float hypothenuseSquared = XOffset * XOffset + YOffset * YOffset;
		if (hypothenuseSquared == 0f || (maxDistanceDelta >= 0f && hypothenuseSquared <= maxDistanceDelta * maxDistanceDelta))
			return target;

		float hypothenuse = (float)Math.Sqrt(hypothenuseSquared);
		return new Vector2(current.X + XOffset / hypothenuse * maxDistanceDelta, current.Y + YOffset / hypothenuse * maxDistanceDelta);
	}
}