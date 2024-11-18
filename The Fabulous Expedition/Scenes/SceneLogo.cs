using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using Font = Raylib_cs.Font;
using static Program;


public class SceneLogo : Scene
{
    private Texture2D texLogo;
	public int timerFrame = 0;
	public int currentFrame = 0;
    public int speedFrame = 2;

	public SceneLogo()
    {
        texLogo = ServiceLocator.GetService<GraphicsManager>().GetTexture("raylib-logo-0");
    }
    public override void Show()
    {
        base.Show();
    }

    public override void Update(float _dt)
    {
        base.Update(_dt);

		timerFrame++;
		if (timerFrame >= speedFrame)
		{
			timerFrame = 0;
			currentFrame++;
		    texLogo = ServiceLocator.GetService<GraphicsManager>().GetTexture("raylib-logo-"+currentFrame);
			if (currentFrame >= 74)
				ServiceLocator.GetService<GameManager>().ChangeScene("menu");
		}
    }

    public override void Draw()
    {
        base.Draw();

        ClearBackground(Color.Black);

        DrawTexture(texLogo, ((int)ServiceLocator.GetService<GameManager>().gameScreenWidth - texLogo.Width) / 2, ((int)ServiceLocator.GetService<GameManager>().gameScreenHeight - texLogo.Height) / 2, Color.White);
    }

    public override void Hide()
    {
        base.Hide();
        ClearBackground(Color.White);
        currentFrame = 0;
        timerFrame = 0;
	}

    public override void Close()
    {
        base.Close();
        ClearBackground(Color.White);
    }
}
