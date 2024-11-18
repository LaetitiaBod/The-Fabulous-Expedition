using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using static System.Net.Mime.MediaTypeNames;

public class SceneSettings : Scene
{
	private GameManager gameManager;
	private GraphicsManager graphicsManager;

	private Texture2D textureBg;
	private Texture2D textureBook;

	private Button confirmButton;
	private Button cancelButton;
	private Button quitButton;
	private Button addButton;
	private Button substractButton;
	private Button borderlessScreenButton;
	private ButtonsList buttonsSettings = new ButtonsList();

	private bool isBorderlessWindowed;
	private float oldMasterVolume;

	public SceneSettings()
	{
		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();

		textureBg = graphicsManager.GetTexture("settingsBg");
		textureBook = graphicsManager.GetTexture("book");
	}

	public override void Show()
	{
		base.Show();

		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();

		confirmButton = new Button(new Rectangle((gameManager.gameScreenWidth - 300) / 2, textureBook.Height - 200, 300, 60), "Confirm", "secondaryButton");
		cancelButton = new Button(new Rectangle(confirmButton.rect.X, confirmButton.rect.Y + 80, 300, 60), "Cancel", "secondaryButton");
		quitButton = new Button(new Rectangle(cancelButton.rect.X, cancelButton.rect.Y + 80, 300, 60), "Quit", "secondaryButton");
		
		addButton = new Button(new Rectangle(
			(int)((gameManager.gameScreenWidth - textureBook.Width) / 2) + 400,
			(int)((gameManager.gameScreenHeight - textureBook.Height) / 2) + 200, 
			30, 30), "+", "smallButton");
		substractButton = new Button(new Rectangle(
			(int)((gameManager.gameScreenWidth - textureBook.Width) / 2) + 450,
			(int)((gameManager.gameScreenHeight - textureBook.Height) / 2) + 200,
			30, 30), "-", "smallButton");
		borderlessScreenButton = new Button(new Rectangle(
			(int)((gameManager.gameScreenWidth - textureBook.Width) / 2) + 400,
			(int)((gameManager.gameScreenHeight - textureBook.Height) / 2) + 300,
			30, 30), " ", "smallButton");

		isBorderlessWindowed = gameManager.borderlessScreen;
		oldMasterVolume = gameManager.masterVolume;

		buttonsSettings.AddButton(confirmButton);
		buttonsSettings.AddButton(cancelButton);
		if(gameManager.previousScene!.name == "gameplay")
			buttonsSettings.AddButton(quitButton);
		buttonsSettings.AddButton(addButton);
		buttonsSettings.AddButton(substractButton);
		buttonsSettings.AddButton(borderlessScreenButton);
	}

	public override void Update(float _dt)
	{
		base.Update(_dt);

		buttonsSettings.Update();

		if (confirmButton.isClicked)
		{
			OptionsFile optionsFile = new OptionsFile();
			optionsFile.AddOption("volume", String.Format("{0:0.00}", gameManager.masterVolume));
			optionsFile.AddOption("borderless", isBorderlessWindowed);
			optionsFile.Save();

			gameManager.ChangeScene(gameManager.previousScene!.name);
		}

		if (cancelButton.isClicked)
		{
			gameManager.SetVolume(oldMasterVolume);
			gameManager.ChangeScene(gameManager.previousScene!.name);
		}

		if (quitButton.isClicked && gameManager.previousScene!.name == "gameplay")
			gameManager.ChangeScene("menu");

		if (addButton.isClicked)
		{
			if (gameManager.masterVolume + .05f < 1f)
				gameManager.SetVolume(gameManager.masterVolume + .05f);
			else
				gameManager.SetVolume(1f);
		}

		if (substractButton.isClicked)
		{
			if (gameManager.masterVolume - .05f > 0f)
				gameManager.SetVolume(gameManager.masterVolume - .05f);
			else
				gameManager.SetVolume(0f);
		}

		if (borderlessScreenButton.isClicked)
			isBorderlessWindowed = !isBorderlessWindowed;
	}

	public override void Draw()
	{
		base.Draw();
		// background
		DrawTexturePro(
			textureBg,
			new Rectangle(0f, 0f, textureBg.Width, textureBg.Height),
			new Rectangle(0, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight),
			new Vector2(0, 0), 0f, new Color(255, 255, 255, 220)
		);

		// book
		DrawTexturePro(
			textureBook,
			new Rectangle(0f, 0f, textureBook.Width, textureBook.Height),
			new Rectangle((gameManager.gameScreenWidth- textureBook.Width) / 2, (gameManager.gameScreenHeight - textureBook.Height) / 2, textureBook.Width, textureBook.Height),
			new Vector2(0, 0), 0f, Color.White
		);

		// buttons
		buttonsSettings.Draw();

		// title
		Vector2 sizeText = MeasureTextEx(graphicsManager.GetFont("helvetica"), "SETTINGS", graphicsManager.GetFont("helvetica").BaseSize, 10);
		DrawTextEx(
			graphicsManager.GetFont("helvetica"), "SETTINGS",
			new Vector2((int)(gameManager.gameScreenWidth - sizeText.X +100) / 2, (int)((gameManager.gameScreenHeight - textureBook.Height) / 2)+50),
			50, 10, Color.Black
		);

		// volume
		int pourcent = (int)(gameManager.masterVolume * 100);
		DrawTextEx(
			graphicsManager.GetFont("helvetica"), $"Volume : {pourcent} %",
			new Vector2((int)((gameManager.gameScreenWidth - textureBook.Width) / 2)+100,
			(int)((gameManager.gameScreenHeight - textureBook.Height) / 2) + 200),
			30, 1, Color.Black
		);

		// fullscreen
		string isBorderlessWindowedText = "No";
		borderlessScreenButton.text = " ";
		if (isBorderlessWindowed)
		{
			isBorderlessWindowedText = "Yes";
			borderlessScreenButton.text = "x";
		}
		DrawTextEx(
			graphicsManager.GetFont("helvetica"), $"Full screen : {isBorderlessWindowedText}",
			new Vector2((int)((gameManager.gameScreenWidth - textureBook.Width) / 2) + 100,
			(int)((gameManager.gameScreenHeight - textureBook.Height) / 2) + 300),
			30, 1, Color.Black
		);
		DrawTextEx(
			graphicsManager.GetFont("helvetica"), "Need to restart to valid changes",
			new Vector2((int)((gameManager.gameScreenWidth - textureBook.Width) / 2) + 100,
			(int)((gameManager.gameScreenHeight - textureBook.Height) / 2) + 350),
			20, 1, Color.Black
		);
	}

	public override void Hide()
	{
		base.Hide();

		buttonsSettings.Hide();
	}

	public override void Close()
	{
		base.Close();
	}
}
