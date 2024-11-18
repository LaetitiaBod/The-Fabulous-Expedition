using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Program;

public class SceneMenu : Scene
{
    private GraphicsManager graphicsManager;
    private GameManager gameManager;
    private AudioManager audioManager;

    private Texture2D texBg1;
    private Texture2D texBg2;
    private Texture2D texBg3;
    private Texture2D texBg4;
    private Texture2D texBg5;
    private Texture2D texBg6;
    private Texture2D texTitle;

    private float titleScale;

    private float scroll2 = 0;
    private float scroll3 = 0;
    private float scroll4 = 0;
    private float scroll5 = 0;
    private float scroll6 = 0;

    private Button  playButton;
    private Button settingsButton;
    private Button quitButton;
    private int buttonLength = 300;
    private ButtonsList buttonsMenu = new ButtonsList();

    public SceneMenu()
    {
		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
        audioManager = ServiceLocator.GetService<AudioManager>();

		texBg1 = graphicsManager.GetTexture("sky");
        texBg2 = graphicsManager.GetTexture("farClouds");
		texBg3 = graphicsManager.GetTexture("nearClouds");
		texBg4 = graphicsManager.GetTexture("farMountains");
		texBg5 = graphicsManager.GetTexture("mountains");
		texBg6 = graphicsManager.GetTexture("trees");
		texTitle = graphicsManager.GetTexture("title");
		titleScale = 1f;

		scroll2 = 0;
        scroll3 = 0;
        scroll4 = 0;
        scroll5 = 0;
        scroll6 = 0;
    }
    public override void Show()
    {
        base.Show();

		gameManager = ServiceLocator.GetService<GameManager>();

		StopSound(audioManager.GetSound("gameplay"));
		audioManager.PlaySound("menu");

		playButton      = new Button(new Rectangle((gameManager.gameScreenWidth - buttonLength) / 2, gameManager.gameScreenHeight - (gameManager.gameScreenHeight / 3), buttonLength, 60), "Play");
		settingsButton  = new Button(new Rectangle((gameManager.gameScreenWidth - buttonLength) / 2, gameManager.gameScreenHeight - (gameManager.gameScreenHeight / 3) + 80, buttonLength, 60), "Settings");
		quitButton      = new Button(new Rectangle((gameManager.gameScreenWidth - buttonLength) / 2, gameManager.gameScreenHeight - (gameManager.gameScreenHeight / 3) + 160, buttonLength, 60), "Quit");

		buttonsMenu.AddButton(playButton);
        buttonsMenu.AddButton(settingsButton);
        buttonsMenu.AddButton(quitButton);
    }

    public override void Update(float _dt)
    {
        base.Update(_dt);
		ServiceLocator.GetService<DebugManager>().AddOption("screen size", GetScreenWidth().ToString() + "x" + GetScreenHeight().ToString());
        gameManager.debugManager.AddOption("virtual screen size", gameManager.gameScreenWidth.ToString() + "x" + gameManager.gameScreenHeight.ToString());

        titleScale = Math.Min(
			(float)gameManager.gameScreenWidth / texTitle.Width, (float)gameManager.gameScreenHeight / texTitle.Height
		);

		scroll2 -= .5f;
        scroll3 -= .7f;
        scroll4 -= .9f;
        scroll5 -= 1.3f;
        scroll6 -= 1.7f;
        
        if (scroll2 <= -gameManager.gameScreenWidth) scroll2 += gameManager.gameScreenWidth;
        if (scroll3 <= -gameManager.gameScreenWidth) scroll3 += gameManager.gameScreenWidth;
        if (scroll4 <= -gameManager.gameScreenWidth) scroll4 += gameManager.gameScreenWidth;
        if (scroll5 <= -gameManager.gameScreenWidth) scroll5 += gameManager.gameScreenWidth;
        if (scroll6 <= -gameManager.gameScreenWidth) scroll6 += gameManager.gameScreenWidth;

        buttonsMenu.Update();

        if (playButton.isClicked)
            gameManager.ChangeScene("gameplay");
        else if (settingsButton.isClicked)
            gameManager.ChangeScene("settings");
        else if (quitButton.isClicked)
            gameManager.exitWindow = true;
    }

    public override void Draw()
    {
        base.Draw();

        DrawBackground(texBg1, new Rectangle(0, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));
        
        DrawBackground(texBg2, new Rectangle(scroll2, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));
        DrawBackground(texBg2, new Rectangle(scroll2 + gameManager.gameScreenWidth, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));

        DrawBackground(texBg3, new Rectangle(scroll3, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));
        DrawBackground(texBg3, new Rectangle(scroll3 + gameManager.gameScreenWidth, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));

        DrawBackground(texBg4, new Rectangle(scroll4, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));
        DrawBackground(texBg4, new Rectangle(scroll4 + gameManager.gameScreenWidth, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));

        DrawBackground(texBg5, new Rectangle(scroll5, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));
        DrawBackground(texBg5, new Rectangle(scroll5 + gameManager.gameScreenWidth, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));

        DrawBackground(texBg6, new Rectangle(scroll6, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));
        DrawBackground(texBg6, new Rectangle(scroll6 + gameManager.gameScreenWidth, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight));

        DrawBackground(texTitle, new Rectangle(
            (gameManager.gameScreenWidth - Math.Min(texTitle.Width * titleScale, gameManager.gameScreenWidth*5/6)) / 2,
			gameManager.gameScreenHeight / 6,
			texTitle.Width * titleScale - gameManager.gameScreenWidth/6,
			texTitle.Height * titleScale));

        buttonsMenu.Draw();
    }

    public override void Hide()
    {
        base.Hide();

        buttonsMenu.Hide();
    }

    public override void Close()
    {
        base.Close();

        UnloadTexture(texBg1);
        UnloadTexture(texBg2);
        UnloadTexture(texBg3);
        UnloadTexture(texBg4);
        UnloadTexture(texBg5);
        UnloadTexture(texBg6);
        UnloadTexture(texTitle);
    }

    private void DrawBackground(Texture2D tex, Rectangle position)
    {
        DrawTexturePro(
            tex,
            new Rectangle(0f, 0f, tex.Width, tex.Height),
            position,
            new Vector2(0, 0), 0f, Color.White
        );
    }
}
