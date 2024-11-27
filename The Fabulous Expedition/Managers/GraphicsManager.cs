using Raylib_cs;
using static Raylib_cs.Raylib;
using TiledSharp;
using System.Xml.Linq;

public class GraphicsManager
{
	private Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();
	private Dictionary<string, Font> fontList = new Dictionary<string, Font>();

	public TmxMap tmxMap = new TmxMap("resources/images/Map/pixel/map_pixel.tmx");

	public Texture2D GetTexture(string name)
	{
		Texture2D texture = new Texture2D();

		textureList.TryGetValue(name, out texture);
		return texture;
	}

	public void AddTexture(string name, string fileName)
	{
		textureList.Add(name, LoadTexture(fileName));
	}

	public void AddAllTextures() {
		// UI
		AddTexture("logoRaylib", "resources/images/UI/raylib_256x256.png");
		AddTexture("primaryButton", "resources/images/UI/primaryButton.png");
		AddTexture("secondaryButton", "resources/images/UI/secondaryButton.png");
		AddTexture("smallButton", "resources/images/UI/smallButton.png");
		AddTexture("itemSlot", "resources/images/UI/itemSlot.png");
		AddTexture("bar", "resources/images/UI/tile_empty.png");
		AddTexture("yellowBar", "resources/images/UI/emptyBar.png");
		AddTexture("brownBar", "resources/images/UI/tile_0001.png");
		AddTexture("book", "resources/images/UI/book.png");
		AddTexture("victory", "resources/images/UI/victory.png");
		AddTexture("gameOver", "resources/images/UI/gameOver.png");

		// BACKGROUNDS
		AddTexture("sky", "resources/images/menu/sky.png");
		AddTexture("farClouds", "resources/images/menu/far-clouds.png");
		AddTexture("nearClouds", "resources/images/menu/near-clouds.png");
		AddTexture("farMountains", "resources/images/menu/far-mountains.png");
		AddTexture("mountains", "resources/images/menu/mountains.png");
		AddTexture("trees", "resources/images/menu/trees.png");
		AddTexture("settingsBg", "resources/images/menu/settingsBg.png");
		// village
		for (int i = 0; i <= 57; i++)
		{
			AddTexture("village-" + i, "resources/images/encounter/village/965cab86-1682-453c-8bb7-8779dae159b6-" + i + ".png");
		}
		// sanctuary
		for (int i = 0; i <= 57; i++)
		{
			AddTexture("sanctuary-" + i, "resources/images/encounter/sanctuary/07da6561-3d07-4599-ab34-a7e4124783ce-" + i + ".png");
		}
		// hunt
		for (int i = 0; i <= 57; i++)
		{
			AddTexture("hunt-" + i, "resources/images/encounter/hunt/bb1c1a0a-6ac1-4596-bf5b-a4ed8e84608f-" + i + ".png");
		}
		AddTexture("victoryBg", "resources/images/encounter/pyramide.png");
		AddTexture("caveBg", "resources/images/encounter/cave entrée.png");
		AddTexture("caveInside", "resources/images/encounter/cave intérieur.png");
		AddTexture("elephantCemetery", "resources/images/encounter/cimetière éléphant.png");


		// TEXT
		AddTexture("title", "resources/images/menu/The-Fabulous-Expedition-extra.png");

		// PLAYER
		AddTexture("playerIdle", "resources/images/Player/Idle_medium.png");
		AddTexture("playerMove", "resources/images/Player/Walk_medium.png");
		AddTexture("playerBoat", "resources/images/Player/Boat.png");

		// ITEMS
		AddTexture("apple", "resources/images/inventory/Apple.png");
		AddTexture("meat", "resources/images/inventory/Meat.png");
		AddTexture("cheese", "resources/images/inventory/Cheese.png");
		AddTexture("goldenPlate", "resources/images/inventory/Golden Plate.png");
		AddTexture("goldenNecklace", "resources/images/inventory/Golden Necklace.png");
		AddTexture("goldenLama", "resources/images/inventory/Golden Lama.png");
		AddTexture("goldenMask", "resources/images/inventory/Golden Mask.png");
		AddTexture("torch", "resources/images/inventory/Torch.png");
		AddTexture("tusk", "resources/images/inventory/Tusk.png");
		AddTexture("mummy", "resources/images/inventory/Mummy.png");
		AddTexture("tooth", "resources/images/inventory/Tooth.png");
		AddTexture("wolfSkin", "resources/images/inventory/Wolf skin.png");

		// TILES
		for (int i = 0; i < tmxMap.Tilesets[0].Tiles.Count; i++)
		{
			AddTexture("tile_" + (i+1), tmxMap.Tilesets[0].Tiles[i].Image.Source);
		}
		AddTexture("wolfIdle", "resources/images/Map/pixel/TimberWolf.png");

		// logo raylib
		for (int i = 0; i <= 74; i++)
		{
			AddTexture("raylib-logo-" + i, "resources/images/UI/raylib_logo_animation/fb465072-4653-46f9-840b-688a2fbece13-" + i + ".png");
		}

	}

	public Font GetFont(string name)
	{
		Font font = new Font();

		fontList.TryGetValue(name, out font);
		return font;
	}

	public void AddFont(string name, string fileName)
	{
		Font font = LoadFontEx(fileName, 72, null, 350);
		SetTextureFilter(font.Texture, TextureFilter.Bilinear);
		fontList.Add(name, font);
	}

	public void AddAllFonts()
	{
		AddFont("helvetica", "resources/fonts/Helvetica.ttf");
		AddFont("helveticaBold", "resources/fonts/Helvetica-Bold.ttf");
	}

	public Animator Idle()
	{
		Texture2D texture = GetTexture("playerIdle");
		return new Animator(texture, 16, texture.Width / 2, new Rectangle(0, 0, texture.Width / 2, texture.Height));
	}

	public Animator Move()
	{
		Texture2D texture = GetTexture("playerMove");
		return new Animator(texture, 8, texture.Width / 4, new Rectangle(0, 0, texture.Width / 4, texture.Height));
	}

	public Animator Boat()
	{
		Texture2D texture = GetTexture("playerBoat");
		return new Animator(texture, 20, texture.Width/4, new Rectangle(0, 0, texture.Width/4, texture.Height));
	}

	public Animator WolfIdle()
	{
		Texture2D texture = GetTexture("wolfIdle");
		return new Animator(texture, 16, texture.Width / 4, new Rectangle(0, 0, texture.Width / 4, texture.Height));
	}

	public void Close()
	{
        foreach (Texture2D texture in textureList.Values)
        {
            UnloadTexture(texture);
        }
		foreach (Font font in fontList.Values)
		{
			UnloadFont(font);
		}
	}
}