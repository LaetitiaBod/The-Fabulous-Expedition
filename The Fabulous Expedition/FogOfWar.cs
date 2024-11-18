using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Program;
using TiledSharp;

public class FogOfWar
{
	private Texture2D texture;
	private int detectionRange = 4;
	public bool isVisited = false;
	public Vector2 coords = new Vector2();
	public List<FogOfWar> fogOfWarList;

	private Player player;
	private Map map;

	public FogOfWar()
	{
		player = ServiceLocator.GetService<Player>();
		map = ServiceLocator.GetService<Map>();
		fogOfWarList = new List<FogOfWar>();
	}

	public FogOfWar(Vector2 _coords)
	{
		coords = _coords;

		player = ServiceLocator.GetService<Player>();
		map = ServiceLocator.GetService<Map>();
		fogOfWarList = new List<FogOfWar>();
	}

	public void Show() {
		player = ServiceLocator.GetService<Player>();
		map = ServiceLocator.GetService<Map>();

		texture = ServiceLocator.GetService<GraphicsManager>().GetTexture("tile_12");

		foreach (TmxLayerTile tile in map.tmxMap.Layers[0].Tiles)
        {
            FogOfWar f = new FogOfWar(new Vector2(tile.X, tile.Y));
			fogOfWarList.Add(f);
        }
    }

	public void Update() 
	{
		Vector2 playerCoords = player.ConvertPixelToMapPosition(player.position);

        for (int y = (int)playerCoords.Y - detectionRange; y <= playerCoords.Y + detectionRange; y++)
        {
			for (int x = (int)playerCoords.X - detectionRange; x <= playerCoords.X + detectionRange; x++)
			{
				FogOfWar? fog = fogOfWarList.Find(f => f.coords == new Vector2(x,y));
				if (fog != null)
					fog.isVisited = true;
			}
		}
    }

	public void Draw()
	{
		for (int i = 0; i < fogOfWarList.Count; i++)
		{
			FogOfWar currentTile = fogOfWarList[i];

			float x = currentTile.coords.X * map.tileWidth;
			float y = (float)currentTile.coords.Y * map.tileHeight;

			Rectangle tilesetRec = new Rectangle(x, y, map.tileWidth, map.tileHeight);
			if(!currentTile.isVisited)
				DrawTextureEx(texture, tilesetRec.Position, 0, 1, Color.White);
		}
	}
}
