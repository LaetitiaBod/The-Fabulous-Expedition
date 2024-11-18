using Raylib_cs;
using static Raylib_cs.Raylib;

public class SceneVictory : Scene
{
	private GameManager gameManager;

	
	public SceneVictory()
	{
		gameManager = ServiceLocator.GetService<GameManager>();
	}

	public override void Show()
	{
		base.Show();

		gameManager = ServiceLocator.GetService<GameManager>();
	}

	public override void Update(float _dt)
	{
		base.Update(_dt);
	}

	public override void Draw()
	{
		base.Draw();
		DrawText("VICTORY", 5, 5, 30, Color.Black);
		DrawLine(0, 30, (int)gameManager.gameScreenWidth, 30, Color.Black);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Close()
	{
		base.Close();
	}
}
