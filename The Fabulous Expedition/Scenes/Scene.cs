using System.Diagnostics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Program;
using System.Numerics;

public abstract class Scene
{
    public string name = "";

    public virtual void Show() {
		ServiceLocator.GetService<GameManager>().camera.Offset = new Vector2(GetScreenWidth()/2, GetScreenHeight()/2);
		ServiceLocator.GetService<GameManager>().camera.Target = new Vector2(GetScreenWidth()/2, GetScreenHeight()/2);
		ServiceLocator.GetService<GameManager>().camera.Zoom = 1f;
	}

    public virtual void Update(float _dt) { }

    public virtual void Draw() {
        ClearBackground(Color.White);
    }

    public virtual void Hide() {
		ServiceLocator.GetService<GameManager>().previousScene = this;
	}

    public virtual void Close() { }
}
