using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class GameManager
{
	public Scene? currentScene;
	public Scene? previousScene;

	public float gameScreenWidth { get; private set; }
	public float gameScreenHeight { get; private set; }
	public float scale = 1;

	public float masterVolume { get; private set; }
	public bool borderlessScreen { get; private set; }
	public bool exitWindow { get; set; }

	private Dictionary<string, Scene> scenes;
	public DebugManager debugManager { get; private set; }
	public OptionsFile optionsFile { get; private set; }
	public Map map { get; private set; }
	public Player player { get; private set; }
	public Inventory inventory { get; private set; }
	public AudioManager audioManager { get; private set; }
	public GraphicsManager graphicsManager { get; private set; }
	public Camera2D camera;

	public GameManager()
	{
		scenes = new Dictionary<string, Scene>();
		debugManager = new DebugManager();
		ServiceLocator.AddService(debugManager);
		optionsFile = new OptionsFile();
		ServiceLocator.AddService(optionsFile);
		audioManager = new AudioManager();
		ServiceLocator.AddService(audioManager);
		graphicsManager = new GraphicsManager();
		ServiceLocator.AddService(graphicsManager);
		camera = new Camera2D();
		ServiceLocator.AddService(camera);

		InitGame();

		audioManager.AddAllSounds();
		graphicsManager.AddAllTextures();
		graphicsManager.AddAllFonts();
		optionsFile.Load();

		if (optionsFile.IsOptionExists("volume"))
		{
			float volume = optionsFile.GetOptionFloat("volume");
			SetVolume(volume);
		}
		else
			SetVolume(0.1f);

		if (optionsFile.IsOptionExists("borderless"))
		{
			bool borderlessOption = optionsFile.GetOptionBool("borderless");
			SetBorderlessWindowed(borderlessOption);
			if (!borderlessOption)
			{
				ClearWindowState(ConfigFlags.BorderlessWindowMode);
				SetWindowSize(1280, 720);
				SetWindowPosition((GetMonitorWidth(GetCurrentMonitor()) - 1280) / 2, (GetMonitorHeight(GetCurrentMonitor()) - 720) / 2);
				SetVirtualGameResolution(1280, 720);
			}
		}
		else
			SetBorderlessWindowed(false);

		camera.Offset = new Vector2(GetScreenWidth() / 2, GetScreenHeight() / 2);
		camera.Zoom = 1f;
		camera.Target = new Vector2(GetScreenWidth() / 2, GetScreenHeight() / 2);

		RegisterAllScenes();
	}

	public void InitGame()
	{
		if(player != null)
			player.Destroy();

		inventory = new Inventory();
		ServiceLocator.AddService(inventory);
		map = new Map(graphicsManager.tmxMap);
		ServiceLocator.AddService(map);
		player = new Player(graphicsManager.Idle(), new Vector2(0,0));
		ServiceLocator.AddService(player);
	}

	public void SetVolume(float volume)
	{
		masterVolume = volume;
		SetMasterVolume(volume);
	}

	public void SetBorderlessWindowed(bool isBorderlessWindowed)
	{
		borderlessScreen = isBorderlessWindowed;

		if (borderlessScreen)
			ToggleBorderlessWindowed();

		else
			ClearWindowState(ConfigFlags.BorderlessWindowMode);
	}

	public void SetVirtualGameResolution(int W, int H)
	{
		gameScreenWidth = W;
		gameScreenHeight = H;
	}

	public void RegisterScene(string name, Scene scene)
	{
		scenes.Add(name, scene);
		scene.name = name;
	}

	public void RegisterAllScenes()
	{
		SceneLogo logo = new SceneLogo();
		SceneMenu menu = new SceneMenu();
		SceneGameplay gameplay = new SceneGameplay();
		SceneSettings settings = new SceneSettings();
		SceneGameOver gameover = new SceneGameOver();
		SceneVictory victory = new SceneVictory();

		RegisterScene("logo", logo);
		RegisterScene("menu", menu);
		RegisterScene("gameplay", gameplay);
		RegisterScene("settings", settings);
		RegisterScene("gameover", gameover);
		RegisterScene("victory", victory);
	}

	public void RemoveScene(string name)
	{
		if (scenes.ContainsKey(name))
		{
			scenes[name].Close();
			scenes.Remove(name);
		}
	}

	public void ChangeScene(string name)
	{
		if (scenes.ContainsKey(name))
		{
			if (currentScene != null)
			{
				currentScene.Hide();
			}
			currentScene = scenes[name];
			currentScene.Show();
		}
	}

	public void UpdateScene(float _dt)
	{
		currentScene?.Update(_dt);
		ServiceLocator.GetService<Hud>().UpdateHud();
	}

	public void DrawScene()
	{
		currentScene?.Draw();
	}

	public void Close()
	{
		foreach (KeyValuePair<string, Scene> item in scenes)
		{
			Console.WriteLine($"La scene {item.Value.name} est détruite");
			item.Value.Close();
		}

		graphicsManager.Close();
		audioManager.Close();
	}
}