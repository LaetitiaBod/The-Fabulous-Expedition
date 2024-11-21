using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class EncounterVictory : Encounter
{
	private GraphicsManager graphicsManager;
	private GameManager gameManager;
	private Player player;
	private Inventory inventory;

	private Texture2D textureBg;
	private Texture2D textureTitle;
	private Rectangle placeholder;

	private Button menuButton;
	private Button quitButton;
	private ButtonsList buttonsWin = new ButtonsList();

	private int scoreFame;
	private int scoreValue;
	private int scoreFood;

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
		inventory = ServiceLocator.GetService<Inventory>();

		textureBg = graphicsManager.GetTexture("victoryBg");
		textureTitle = graphicsManager.GetTexture("victory");

		placeholder = new Rectangle(100, 150, gameManager.gameScreenWidth*2 / 7, gameManager.gameScreenHeight*2/3);

		menuButton = new Button(new Rectangle(placeholder.X + 100, placeholder.Height, placeholder.Width / 3, 60), "Menu");
		quitButton = new Button(new Rectangle(placeholder.Width - 100, placeholder.Height, placeholder.Width / 3, 60), "Quit");

		buttonsWin.AddButton(menuButton);
		buttonsWin.AddButton(quitButton);

		foreach (InventoryItem item in inventory.stashDict.Values)
		{
			scoreFame += item.data.fame * item.stackSize;
			scoreValue += item.data.value * item.stackSize;
			scoreFood += item.data.foodAmount * item.stackSize;
		}
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

		// Background
		DrawTexturePro(
			textureBg,
			new Rectangle(0f, 0f, textureBg.Width, textureBg.Height),
			new Rectangle(0, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight),
			new Vector2(0, 0), 0f, Color.White
		);

		// title
		DrawTexturePro(
			textureTitle,
			new Rectangle(0f, 0f, textureTitle.Width, textureTitle.Height),
			new Rectangle(placeholder.X, placeholder.Y, textureTitle.Width, textureTitle.Height),
			new Vector2(0, 0), 0f, Color.White
		);

		// description
		string description = $"  You found the golden pyramid !\n\n" +
			$"You can go home with all your treasures. \n\n";
		DrawTextEx(graphicsManager.GetFont("helvetica"), description, new Vector2(placeholder.X, placeholder.Y + textureTitle.Height + 50), 20, 4, Color.Black);

		// score
		string scoreFameStr = $"Your fame : {scoreFame}";
		string scoreValueStr = $"Value of your inventory : {scoreValue}";
		string scoreFoodStr = $"Amount of your remaining food : {scoreFood}";
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreFameStr, new Vector2(placeholder.X, placeholder.Y + textureTitle.Height + 150), 20, 4, Color.Black);
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreValueStr, new Vector2(placeholder.X, placeholder.Y + textureTitle.Height + 190), 20, 4, Color.Black);
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreFoodStr, new Vector2(placeholder.X, placeholder.Y + textureTitle.Height + 230), 20, 4, Color.Black);

		// buttons
		buttonsWin.Draw();
	}

	public override void Close()
	{
		base.Close();
		buttonsWin.Hide();
	}
}