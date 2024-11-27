using Raylib_cs;
using System.Numerics;
using static Program;
using static Raylib_cs.Raylib;

public abstract class Entity
{
    public Animator anim;
    public bool isDestroyed { get; private set; } = false;

    public Vector2 position;
    protected Rectangle hitbox;
    protected float scale;

    public static List<Entity> ALL = new List<Entity>();

    protected Entity(Animator _anim, Vector2 _position)
    {
        anim = _anim;
        position = _position;
        scale = 1;

        ALL.Add(this);
    }

    public virtual void Show() { }

    public virtual void Update() {
        if (isDestroyed)
            return;

        hitbox.Position = position;
    }

    public virtual void Draw() {
        if (isDestroyed)
            return;
    }

    public virtual void Hide() { }

    public virtual void Close() { }

    public void Destroy()
    {
        isDestroyed = true;
    }

    public static void CleanUp()
    {
        for (int i = ALL.Count - 1; i >= 0; i--)
        {
            if (ALL[i].isDestroyed)
            {
                ALL.RemoveAt(i);
            }
        }
    }

    public Vector2 ConvertPixelToMapPosition(Vector2 _position)
    {
		return new Vector2(
			(int)(_position.X / ServiceLocator.GetService<GameManager>().map.tileWidth),
			(int)(_position.Y / ServiceLocator.GetService<GameManager>().map.tileHeight)
		);
    }

	public Vector2 ConvertMapToPixelPosition(Vector2 _coords)
	{
		return new Vector2(
			(_coords.X * ServiceLocator.GetService<GameManager>().map.tileWidth) + ServiceLocator.GetService<GameManager>().map.tileWidth / 2,
			(_coords.Y * ServiceLocator.GetService<GameManager>().map.tileHeight) + ServiceLocator.GetService<GameManager>().map.tileHeight / 2
		);
	}
}

