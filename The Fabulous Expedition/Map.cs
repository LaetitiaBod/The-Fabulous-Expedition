using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Program;
using TiledSharp;
using static System.Formats.Asn1.AsnWriter;


public class Map
{
	public TmxMap tmxMap;
	private Dictionary<int, Texture2D> tilesTextures;

	public int spriteWidth;
	public int spriteHeight;
	public int tileWidth;
	public int tileHeight;

	private Mouse mouse;
	private Vector2 mousePos;

	public TmxLayerTile mouseTile;
	public TmxLayerTile playerTile;

	public Texture2D textureToDisplay;
	public Player player = ServiceLocator.GetService<Player>();
	public FogOfWar fogOfWar;

	public Animator animWolf;

	public List<TileType> tileTypesList = new List<TileType>
	{
		new TileType( 0, "Grass", 1 ),
		new TileType( 1, "LightForest", 2 ),
		new TileType( 2, "Mountain", 0 ),
		new TileType( 3, "Pyramid", 0 , true ),
		new TileType( 4, "QuestionMark", 0 ),
		new TileType( 5, "Sanctuary", 0, true ),
		new TileType( 6, "Village", 0, true ),
		new TileType( 7, "Shelter", 0, true ),
		new TileType( 8, "Old camp", 0, true ),
		new TileType( 11, "DenseForest", 8 ),
		new TileType( 12, "Desert", 5 ),
		new TileType( 13, "Fog", 5 ),
		new TileType( 14, "Water", 0 ),
		new TileType( 15, "Cave", 0, true ),
		new TileType( 16, "ElephantCemetery", 0, true ),
		new TileType( 17, "Hunt", 0, true ),
	};

	public List<Encounter> encounterList;

	public Map(TmxMap _tmxMap)
	{
		tmxMap = _tmxMap;
		tilesTextures = new Dictionary<int, Texture2D>();
		mouse = new Mouse();
		mousePos = new Vector2();
		mouseTile = new TmxLayerTile(0, 0, 0);
		playerTile = new TmxLayerTile(0, 0, 0);
		fogOfWar = new FogOfWar();
		encounterList = new List<Encounter>();
		animWolf = ServiceLocator.GetService<GraphicsManager>().WolfIdle();
	}

	public void Show()
	{
		player = ServiceLocator.GetService<Player>();

		for (int i = 0; i < tmxMap.Tilesets[0].Tiles.Count; i++)
		{
			Texture2D tile = ServiceLocator.GetService<GraphicsManager>().GetTexture("tile_" + (i + 1));
			tilesTextures.Add(tmxMap.Tilesets[0].Tiles[i].Id, tile);
		}

		for (int i = 0; i < tmxMap.Layers[1].Tiles.Count; i++)
		{
			TmxLayerTile currentTile = tmxMap.Layers[1].Tiles[i];
			int tileFrame = currentTile.Gid - 1;

			TileType tileType = tileTypesList.Find(tile => tile.id == tileFrame);

			if (tileType.name == "Pyramid")
				encounterList.Add(new EncounterVictory(tileType.name, new Vector2(currentTile.X, currentTile.Y), false));
			if (tileType.name == "Village")
				encounterList.Add(new EncounterVillage(tileType.name, new Vector2(currentTile.X, currentTile.Y), false));
			if (tileType.name == "Sanctuary")
				encounterList.Add(new EncounterSanctuary(tileType.name, new Vector2(currentTile.X, currentTile.Y), false));
			if (tileType.name == "Cave")
				encounterList.Add(new EncounterCave(tileType.name, new Vector2(currentTile.X, currentTile.Y), false));
			if (tileType.name == "ElephantCemetery")
				encounterList.Add(new EncounterElephantCemetery(tileType.name, new Vector2(currentTile.X, currentTile.Y), false));
			if (tileType.name == "Hunt")
				encounterList.Add(new EncounterHunt(tileType.name, new Vector2(currentTile.X, currentTile.Y), true));
		}

		spriteWidth = tmxMap.Tilesets[0].TileWidth;
		spriteHeight = tmxMap.Tilesets[0].TileHeight;

		tileWidth = tmxMap.TileWidth;
		tileHeight = tmxMap.TileHeight;

		player.position = player.ConvertMapToPixelPosition(new Vector2(30, 24));

		animWolf = ServiceLocator.GetService<GraphicsManager>().WolfIdle();
	}

	public void Update() {
		mousePos = GetScreenToWorld2D(mouse.GetMousePosition(), ServiceLocator.GetService<GameManager>().camera);

		// update the mouse tile and the player tile
		for (int nLayer = 0; nLayer < tmxMap.Layers.Count; nLayer++)
		{
			for (int i = 0; i < tmxMap.Layers[nLayer].Tiles.Count; i++)
			{
				TmxLayerTile currentTile = tmxMap.Layers[nLayer].Tiles[i];
				int gid = currentTile.Gid;

				if (gid != 0)
				{
					int tileFrame = gid - 1;

					float x = currentTile.X * tileWidth;
					float y = currentTile.Y * tileHeight;

					Rectangle tilesetRec = new Rectangle(x, y, tileWidth, tileHeight);

					if (CheckCollisionPointRec(mousePos, tilesetRec))
						mouseTile = currentTile;
					GetPlayerTile(tilesetRec, currentTile);
				}
			}
		}
		ServiceLocator.GetService<DebugManager>().AddOption("mouse x", mousePos.X);
		ServiceLocator.GetService<DebugManager>().AddOption("mouse y", mousePos.Y);

		fogOfWar.Update();

		animWolf.Update();
	}

	public void Draw()
	{
		for (int nLayer = 0; nLayer < tmxMap.Layers.Count; nLayer++)
		{
			for (int i = 0; i < tmxMap.Layers[nLayer].Tiles.Count; i++)
			{
				TmxLayerTile currentTile = tmxMap.Layers[nLayer].Tiles[i];
				if (currentTile.Gid != 0)
				{
					if (nLayer == 1)
						Console.WriteLine("");
					int tileFrame = currentTile.Gid - 1;
					textureToDisplay = tilesTextures[tileFrame];

					float x = currentTile.X * tileWidth;
					float y = (float)currentTile.Y * (tileHeight) - (tilesTextures[tileFrame].Height - tileHeight);

					Rectangle tilesetRec = new Rectangle(x, y, tileWidth, tilesTextures[tileFrame].Height);
					Rectangle srcRect = new Rectangle(0, 0, spriteWidth, tilesTextures[tileFrame].Height);
					Encounter? encounter = encounterList.Find(encounter => encounter.coords.X == currentTile.X && encounter.coords.Y == currentTile.Y);

					// display the question mark
					if (nLayer == 1 && encounter != null && !encounter.isRevealed)
					{
						textureToDisplay = tilesTextures[tileTypesList.Find(t => t.name == "QuestionMark").id];
					}
					
					// Hunt animation
					if(nLayer == 1 && encounter != null && encounter.name == "Hunt" && encounter.isRevealed)
					{
						textureToDisplay = animWolf.texture;
						srcRect = animWolf.rectFrame;
					} 

					DrawTexturePro(textureToDisplay, srcRect, tilesetRec, new Vector2(0, 0), 0, Color.White);

					string text = currentTile.X.ToString() + "," + currentTile.Y.ToString();
					DrawTextEx(ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"), text, new Vector2(x, currentTile.Y * tileHeight), 40, 1, Color.White);
				}
			}
		}

		fogOfWar.Draw();
	}

	public void Hide() {
		tilesTextures.Clear();
	}

	public void Close() { }

	public void GetPlayerTile(Rectangle tilesetRec, TmxLayerTile currentTile)
	{
		if(ServiceLocator.GetService<GameManager>().player != null)
			if (CheckCollisionPointRec(ServiceLocator.GetService<GameManager>().player.position, tilesetRec))
				playerTile = currentTile;
	}

	public void GetMouseTile(Rectangle tilesetRec, TmxLayerTile currentTile)
	{
		if (CheckCollisionPointRec(mousePos, tilesetRec))
			mouseTile = currentTile;
	}

	public List<Location> GetLocationNeighbors(Location _location)
	{
        List<Location> adjacentSquares = new List<Location>()
		{
			new Location(new Vector2(_location.coords.X,  _location.coords.Y - 1)),
			new Location(new Vector2(_location.coords.X,  _location.coords.Y + 1)),
			new Location(new Vector2(_location.coords.X - 1,  _location.coords.Y)),
			new Location(new Vector2(_location.coords.X + 1,  _location.coords.Y)),
		};
		List<Location> validNeighbors = new List<Location>();

		foreach (TmxLayerTile tile in tmxMap.Layers[0].Tiles)
        {
			foreach(Location neighbor in adjacentSquares)
			{
				if (tile.X == neighbor.coords.X && tile.Y == neighbor.coords.Y)
				{
					Location validNeighbor = new Location(new Vector2(neighbor.coords.X, neighbor.coords.Y), tile.Gid-1);
					if (isTileWalkable(validNeighbor))
						validNeighbors.Add(validNeighbor);
				}
			}
		}
		return validNeighbors;
	}

	public bool isTileWalkable(TmxLayerTile tile)
	{
		int tileFrame = tile.Gid - 1;
		TileType tileType = tileTypesList.Find(tile => tile.id == tileFrame);
		if (tileType.name == "Mountain" || tileType.name == "Water")
			return false;
		return true;
	}

	public bool isTileWalkable(Location _location)
	{
		if(encounterList.Find(e => e.coords == _location.coords) == null)
		{
			if (_location.tileType.name == "Mountain" || _location.tileType.name == "Water")
				return false;

		}
		return true;
	}
}
