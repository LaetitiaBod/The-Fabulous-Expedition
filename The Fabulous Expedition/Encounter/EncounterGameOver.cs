using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class EncounterGameOver : Encounter
{
	private GraphicsManager graphicsManager;
	private GameManager gameManager;
	private Player player;
	private Inventory inventory;

	private Texture2D textureTitle;
	private Rectangle placeholder;

	private Button menuButton;
	private Button quitButton;
	private ButtonsList buttonsGameover = new ButtonsList();

	private int scoreFame;
	private int scoreValue;
	private int scoreFood;

	private string coroner = "";

	public EncounterGameOver(string _name, Vector2 coords, bool _isRevealed) : base(_name, coords, _isRevealed)
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

		textureTitle = graphicsManager.GetTexture("gameOver");

		placeholder = new Rectangle((gameManager.gameScreenWidth - textureTitle.Width) / 2, (gameManager.gameScreenHeight - textureTitle.Height) / 2, textureTitle.Width, textureTitle.Height);

		menuButton = new Button(new Rectangle(placeholder.X + 100, placeholder.Y + placeholder.Height - 150, placeholder.Width / 3, 60), "Menu");
		quitButton = new Button(new Rectangle(placeholder.X + placeholder.Width*2/3 - 100, placeholder.Y + placeholder.Height - 150, placeholder.Width / 3, 60), "Quit");

		buttonsGameover.AddButton(menuButton);
		buttonsGameover.AddButton(quitButton);

		foreach (InventoryItem item in inventory.stashDict.Values)
		{
			scoreFame += item.data.fame * item.stackSize;
			scoreValue += item.data.value * item.stackSize;
			scoreFood += item.data.foodAmount * item.stackSize;
		}

		if (player.currentFood <= 0)
			coroner = "You are starving to death";
	}

	public override void Update()
	{
		base.Update();
		buttonsGameover.Update();

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

		// title
		DrawTexturePro(
			textureTitle,
			new Rectangle(0f, 0f, textureTitle.Width, textureTitle.Height),
			new Rectangle(placeholder.X, placeholder.Y, textureTitle.Width, textureTitle.Height),
			new Vector2(0, 0), 0f, Color.White
		);

		// description
		Vector2 sizeText = MeasureTextEx(graphicsManager.GetFont("helvetica"), coroner, 20, 4);
		DrawTextEx(graphicsManager.GetFont("helvetica"), coroner, new Vector2(placeholder.X + (textureTitle.Width - sizeText.X)/2, placeholder.Y + 200), 20, 4, Color.Black);

		// score
		string scoreFameStr		= $"Your actual fame :";
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreFameStr, new Vector2(placeholder.X  + 100, placeholder.Y + 300), 20, 4, Color.Black);
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreFame.ToString(), new Vector2(placeholder.X + placeholder.Width - 150, placeholder.Y + 300), 20, 4, Color.Black);

		string scoreValueStr	= $"Value of your inventory :";
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreValueStr, new Vector2(placeholder.X + 100, placeholder.Y + 340), 20, 4, Color.Black);
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreValue.ToString(), new Vector2(placeholder.X + placeholder.Width - 150, placeholder.Y + 340), 20, 4, Color.Black);

		string scoreFoodStr		= $"Amount of your remaining food :";
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreFoodStr, new Vector2(placeholder.X + 100, placeholder.Y + 380), 20, 4, Color.Black);
		DrawTextEx(graphicsManager.GetFont("helvetica"), scoreFood.ToString(), new Vector2(placeholder.X + placeholder.Width - 150, placeholder.Y + 380), 20, 4, Color.Black);

		buttonsGameover.Draw();
	}

	public override void Close()
	{
		base.Close();
		buttonsGameover.Hide();
	}
}