using System.Numerics;
using TiledSharp;
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
		destination = player.movements.Keys.Last();
	}

	public override void Update()
	{
		base.Update();

		// when the node is reach, remove the node
		if (destination == player.position)
		{
			UpdateHuntPosition();
			player.movements.Remove(destination);

			// if we are on the final destination, idle
			if (player.movements.Keys.Count <= 0)
			{
				stateMachine.ChangeState(player.idleState);
				return;
			}
		}

		destination = player.movements.Keys.Last();

		// move to the node
		player.position = MoveTowards(player.position, destination, player.moveSpeed * GetFrameTime());

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

	// copied from the method MoveTowards from Unity
	public Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
	{
		float XOffset = target.X - current.X;
		float YOffset = target.Y - current.Y;
		float hypothenuseSquared = XOffset * XOffset + YOffset * YOffset;
		if (hypothenuseSquared == 0f || (maxDistanceDelta >= 0f && hypothenuseSquared <= maxDistanceDelta * maxDistanceDelta))
			return target;

		float hypothenuse = (float)Math.Sqrt(hypothenuseSquared);
		return new Vector2(current.X + XOffset / hypothenuse * maxDistanceDelta, current.Y + YOffset / hypothenuse * maxDistanceDelta);
	}

	public void UpdateHuntPosition()
	{
		// find all the hunts
		for (int i = 0; i < map.tmxMap.Layers[1].Tiles.Count; i++)
		{
			TmxLayerTile currentTile = map.tmxMap.Layers[1].Tiles[i];
			int gid = currentTile.Gid;

			if (gid != 0)
			{
				int tileFrame = gid - 1;
				TileType tileType = map.tileTypesList.Find(tile => tile.id == tileFrame);

				// move randomly each hunt
				if (tileType.name == "Hunt")
				{
					Location? newLocation;
					newLocation = RandomMove(currentTile);

					// success randomly moving
					if(newLocation != null)
					{
						// modify the encounter
						Encounter? hunt = map.encounterList.Find(e => e.coords == new Vector2(currentTile.X, currentTile.Y));
						if(hunt != null)
							hunt.coords = newLocation.coords;

						// modify the old tile and the new tile
						currentTile.Gid = 0;
						for (int j = 0; j < map.tmxMap.Layers[1].Tiles.Count; j++)
						{
							TmxLayerTile newTile = map.tmxMap.Layers[1].Tiles[j];
							if (newTile.X == newLocation.coords.X && newTile.Y == newLocation.coords.Y)
								newTile.Gid = tileType.id+1;
						}
					}
				}	
			}
		}
	}

	public Location? RandomMove(TmxLayerTile currentTile)
	{
		Random randomMove = new Random();
		int moveChance = randomMove.Next(1, 5);

		Location huntLocation = new Location(new Vector2(currentTile.X, currentTile.Y), currentTile.Gid - 1);
		List<Location> validNeighbors = map.GetLocationNeighbors(huntLocation);

		if (moveChance == 1)
			return ChooseRandomLocation(validNeighbors);

		return null;
	}

	public Location ChooseRandomLocation(List<Location> validNeighbors)
	{
		Random randomLocation = new Random();
		int index = randomLocation.Next(validNeighbors.Count);

		return validNeighbors[index];
	}
}