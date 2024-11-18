using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class PlayerIdleState : PlayerState
{
	private GameManager gameManager = ServiceLocator.GetService<GameManager>();
	private Map map = ServiceLocator.GetService<Map>();

	private Location current = new Location(Vector2.Zero);
	private Location start = new Location(Vector2.Zero);
	public Location target = new Location(Vector2.Zero);

	private List<Location> openList = new List<Location>();
	private List<Location> closedList = new List<Location>();
	private Dictionary<Vector2, TileType> movements = new Dictionary<Vector2, TileType>();
	private int g = 0; // G score counter

	public Encounter? currentEncounter;
	private Button enterButton;
	private ButtonsList buttonsEncounter = new ButtonsList();
	private int buttonLength = 200;
	private bool wasClicked = false;

	public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, Animator _anim) : base(_player, _stateMachine, _anim)
	{
	}

	public override void Show()
	{
		base.Show();
		gameManager = ServiceLocator.GetService<GameManager>();
		map = ServiceLocator.GetService<Map>();

		player.anim = ServiceLocator.GetService<GraphicsManager>().Idle();

		enterButton = new Button(new Rectangle(gameManager.player.position.X - buttonLength / 2, gameManager.player.position.Y - 120, buttonLength, 60), "Enter");
		buttonsEncounter.AddButton(enterButton);

		start = new Location(new Vector2(gameManager.map.playerTile.X, gameManager.map.playerTile.Y), gameManager.map.playerTile.Gid);
		target = new Location(new Vector2(gameManager.map.mouseTile.X, gameManager.map.mouseTile.Y), gameManager.map.mouseTile.Gid);
		openList.Add(start);

		Pathfinding();
	}

	public override void Update()
	{
		base.Update();

		// player is on an encounter
		currentEncounter = map.encounterList.Find(e => e.coords == player.ConvertPixelToMapPosition(player.position));
		if (currentEncounter != null)
		{
			//if (!wasClicked)
			//{
				buttonsEncounter.mouse = new Mouse(true);
				buttonsEncounter.Update();
			//}
			if (enterButton.isClicked) 
			{
				//wasClicked = true;
				player.encounterState.currentEncounter = currentEncounter;
				stateMachine.ChangeState(player.encounterState);
				return;
			}
		}
		
		if (
			target.coords.X != gameManager.map.mouseTile.X ||
			target.coords.Y != gameManager.map.mouseTile.Y ||
			movements.Count == 0
		)
		{
			// Initialize Pathfinding algorith
			movements.Clear();
			openList.Clear();
			closedList.Clear();

			current = new Location(Vector2.Zero);
			start = new Location(new Vector2(gameManager.map.playerTile.X, gameManager.map.playerTile.Y), gameManager.map.playerTile.Gid);
			target = new Location(new Vector2(gameManager.map.mouseTile.X, gameManager.map.mouseTile.Y), gameManager.map.mouseTile.Gid);
			g = 0;

			openList.Add(start);

			FogOfWar? fog = map.fogOfWar.fogOfWarList.Find(fog => fog.coords == target.coords);
			
			if(fog != null && fog.isVisited)
				// Start Pathfinding algorith
				Pathfinding();

			if (movements.Count > 0)
				CalculateCost();
		}

		if (IsMouseButtonPressed(MouseButton.Left))
		{
			if(!ServiceLocator.GetService<Inventory>().itemSlotsList.list.Exists(e => e.isOverflown == true))
			{
				// if a path is found, move to each destinations
				if (movements.Count > 0 && gameManager.map.isTileWalkable(gameManager.map.mouseTile) && player.movementCost < player.currentFood)
				{
					player.movements = movements;
					PayFoodCost();
					stateMachine.ChangeState(player.moveState);
				}
			}
		}
	}

	public override void Draw()
	{
		base.Draw();

		// Draw path
		foreach (Vector2 tiles in movements.Keys)
		{
			DrawCircle((int)tiles.X, (int)tiles.Y, 10, Color.Black);
			if (player.movementCost < player.currentFood)
				DrawCircle((int)tiles.X, (int)tiles.Y, 8, Color.Purple);
			else
				DrawCircle((int)tiles.X, (int)tiles.Y, 8, Color.Red);
		}

		string textCost = player.movementCost.ToString();
		Vector2 pos = player.ConvertMapToPixelPosition(new Vector2(ServiceLocator.GetService<Map>().mouseTile.X, ServiceLocator.GetService<Map>().mouseTile.Y));
		if (player.movementCost < player.currentFood && movements.Count > 0)
			DrawTextEx(ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"), textCost, new Vector2(pos.X + 20, pos.Y - 30), 50, 1, Color.Purple);
		else if(movements.Count <= 0)
			DrawText("x", (int)pos.X + 20, (int)pos.Y - 30, 70, Color.Red);
		else
			DrawTextEx(ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"), textCost, new Vector2(pos.X + 20, pos.Y - 30), 50, 1, Color.Red);

		if (currentEncounter != null)
			buttonsEncounter.Draw();
	}

	public override void Close()
	{
		base.Close();
		buttonsEncounter.Hide();
	}

	private void Pathfinding()
	{
		while (openList.Count > 0)
		{
			// algorithm's logic goes here
			// get the square with the lowest F score
			int lowest = openList.Min(l => l.scoreF);
			current = openList.First(l => l.scoreF == lowest);

			// add the current square to the closed list
			closedList.Add(current);

			// remove it from the open list
			openList.Remove(current);

			// if we added the destination to the closed list, we've found a path
			if (current.coords == target.coords)
			{
				openList.Clear();
				break;
			}

			List<Location> adjacentSquares = gameManager.map.GetLocationNeighbors(current);
			g++;

			foreach (var adjacentSquare in adjacentSquares)
			{
				// if this adjacent square is not in the closed list, ignore it
				if (closedList.FirstOrDefault(l => l.coords.X == adjacentSquare.coords.X
					&& l.coords.Y == adjacentSquare.coords.Y) == null)
				{
					// if it's not in the open list...
					if (openList.FirstOrDefault(l => l.coords.X == adjacentSquare.coords.X
						&& l.coords.Y == adjacentSquare.coords.Y) == null)
					{
						// compute its score, set the parent
						adjacentSquare.scoreG = g * adjacentSquare.tileType.cost;
						adjacentSquare.scoreH = (int)ComputeHScore(adjacentSquare, target);
						adjacentSquare.scoreF = adjacentSquare.scoreG + adjacentSquare.scoreH;
						adjacentSquare.Parent = current;

						// and add it to the open list
						openList.Insert(0, adjacentSquare);
					}
					else
					{
						// test if using the current G score makes the adjacent square's F score
						// lower, if yes update the parent because it means it's a better path
						if (g + adjacentSquare.scoreH < adjacentSquare.scoreF)
						{
							adjacentSquare.scoreG = g;
							adjacentSquare.scoreF = adjacentSquare.scoreG + adjacentSquare.scoreH;
							adjacentSquare.Parent = current;
						}
					}
				}
			}
		}
		// when the loop is finished
		if (openList.Count <= 0)
		{
			// if the final node is the final destination
			if (current.coords == target.coords)
			{
				// add all nodes to the movements list to reach
				while (current.coords != start.coords)
				{
					movements.Add(player.ConvertMapToPixelPosition(current.coords), current.tileType);
					current = current.Parent!;
				}
			}
		}
	}

	private void CalculateCost()
	{
		player.movementCost = 0;

		foreach (TileType tileType in movements.Values)
		{
			int cost = tileType.cost;
			player.movementCost += cost;
		}
	}

	public void PayFoodCost()
	{
		player.currentFood = player.currentFood - player.movementCost;
	}

	public static float ComputeHScore(Location _source, Location _target)
	{
		return Math.Abs(_target.coords.X - _source.coords.X) + Math.Abs(_target.coords.Y - _source.coords.Y);
	}
}
