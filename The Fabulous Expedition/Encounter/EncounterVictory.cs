using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class EncounterVictory : Encounter
{
	private GraphicsManager graphicsManager;
	private GameManager gameManager;
	private Player player;
	private Texture2D texture;

	private Button menuButton;
	private Button quitButton;
	private ButtonsList buttonsWin = new ButtonsList();

	public EncounterVictory(string _name, Vector2 coords, bool _isRevealed) : base(_name, coords, _isRevealed)
	{
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		gameManager = ServiceLocator.GetService<GameManager>();
		player = ServiceLocator.GetService<Player>();
	}

	public override void Show()
	{
		base.Show();

		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		gameManager = ServiceLocator.GetService<GameManager>();

		texture = graphicsManager.GetTexture("victory");

		int buttonLength = 200;
		menuButton = new Button(new Rectangle((gameManager.gameScreenWidth - buttonLength - 300) / 2, gameManager.gameScreenHeight / 8 + texture.Height - 150, buttonLength, 60), "Menu");
		quitButton = new Button(new Rectangle((gameManager.gameScreenWidth - buttonLength + 300) / 2, gameManager.gameScreenHeight / 8 + texture.Height - 150, buttonLength, 60), "Quit");

		buttonsWin.AddButton(menuButton);
		buttonsWin.AddButton(quitButton);
	}

	public override void Update()
	{
		base.Update();
		buttonsWin.Update();

		if (menuButton.isClicked)
		{
			player.stateMachine.ChangeState(player.idleState);
			gameManager.ChangeScene("menu");
		}
		else if (quitButton.isClicked)
		{
			gameManager.exitWindow = true;
		}
	}

	public override void Draw()
	{
		base.Draw();

		DrawTexturePro(
			texture,
			new Rectangle(0f, 0f, texture.Width, texture.Height),
			new Rectangle((gameManager.gameScreenWidth - texture.Width) / 2, gameManager.gameScreenHeight / 8, texture.Width, texture.Height),
			new Vector2(0, 0), 0f, Color.White
		);
		buttonsWin.Draw();
	}

	public override void Close()
	{
		base.Close();
		buttonsWin.Hide();
	}
}