using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class SceneGameplay : Scene
{
	private GameManager gameManager;
	private AudioManager audioManager;

	public SceneGameplay() {
		gameManager = ServiceLocator.GetService<GameManager>();
		audioManager = ServiceLocator.GetService<AudioManager>();
	}

	public override void Show()
	{
		base.Show();

		gameManager = ServiceLocator.GetService<GameManager>();
		audioManager = ServiceLocator.GetService<AudioManager>();

		if(gameManager.previousScene!.name != "settings")
		{
			gameManager.InitGame();
			gameManager.map.fogOfWar.Show();
			gameManager.inventory.Show();
			foreach (Entity sprite in Entity.ALL)
			{
				sprite.Show();
			}
		}
		gameManager.map.Show();
		StopSound(audioManager.GetSound("menu"));
		audioManager.PlaySound("gameplay");
		gameManager.camera.Zoom = .5f;
	}

	public override void Update(float _dt)
	{
		base.Update(_dt);

		gameManager.map.Update();

		ZoomUpdate();

		foreach (Entity sprite in Entity.ALL)
		{
			sprite.Update();
		}

		if (IsKeyPressed(KeyboardKey.Escape))
			gameManager.ChangeScene("settings");
	}

	public override void Draw()
	{
		base.Draw();
		ClearBackground(Color.Beige);

		gameManager.map.Draw();

		foreach (Entity sprite in Entity.ALL)
		{
			sprite.Draw();
		}
	}

	public override void Hide()
	{
		base.Hide();

		gameManager.map.Hide();

		foreach (Entity sprite in Entity.ALL)
		{
			sprite.Hide();
		}
	}

	public override void Close()
	{
		base.Close();

		foreach (Entity sprite in Entity.ALL)
		{
			sprite.Close();
		}
	}

	private void ZoomUpdate()
	{
		ServiceLocator.GetService<DebugManager>().AddOption("zoom", gameManager.camera.Zoom);
		gameManager.camera.Zoom += ((float)GetMouseWheelMove() * 0.05f);

		if (gameManager.camera.Zoom > .8f) gameManager.camera.Zoom = .8f;
		else if (gameManager.camera.Zoom < 0.3f) gameManager.camera.Zoom = 0.3f;

		if (IsKeyPressed(KeyboardKey.Space))
		{
			gameManager.camera.Zoom = .5f;
			if (gameManager.player != null)
				gameManager.camera.Target = gameManager.player.position;
		}

		if (IsMouseButtonDown(MouseButton.Right))
		{
			Vector2 delta = GetMouseDelta();
			delta = new Vector2(delta.X * (-1.0f / gameManager.camera.Zoom), delta.Y * (-1.0f / gameManager.camera.Zoom));
			gameManager.camera.Target = Vector2.Add(gameManager.camera.Target, delta);
		}
	}
}
