using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using static System.Net.Mime.MediaTypeNames;


public class Hud
{
	private GameManager gameManager;
	private Bar foodBar;
	private float barWidth;
	private float barHeight;

	public Hud()
	{
		gameManager = ServiceLocator.GetService<GameManager>();

		barWidth = gameManager.gameScreenWidth * 1 / 3;
		barHeight = gameManager.gameScreenHeight * 1 / 16;

		foodBar = new Bar(new Rectangle(
			(gameManager.gameScreenWidth - barWidth) / 2,
			(gameManager.gameScreenHeight - gameManager.gameScreenHeight) / 2,
			barWidth,
			barHeight),
			gameManager.player.currentFood.ToString() + " / " + gameManager.player.foodMax.ToString(),
			(gameManager.gameScreenWidth * 1 / 3) * (gameManager.player.currentFood / 100)
		);
		foodBar.size = barWidth - 18;
	}

	public void UpdateHud()
	{
		foodBar.text = gameManager.player.currentFood.ToString() + " / " + gameManager.player.foodMax.ToString();
		foodBar.size = (barWidth - 18) * (gameManager.player.currentFood / gameManager.player.foodMax);

		ServiceLocator.GetService<Inventory>().Update();
	}

	public void DrawHud()
	{
		foodBar.Draw();
		ServiceLocator.GetService<Inventory>().Draw();
	}
}
