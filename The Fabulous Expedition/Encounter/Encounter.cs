using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public abstract class Encounter
{
	public string name = "";
	public Vector2 coords;
	public bool isRevealed;

	public Encounter(string _name, Vector2 _coords, bool _isRevealed)
	{
		name = _name;
		coords = _coords;
		isRevealed = _isRevealed;
	}

	public virtual void Show() { }

	public virtual void Update() { }

	public virtual void Draw() {
		EndMode2D();
	}
	public virtual void Close() { }

	public virtual void TradeItem(ItemSlot _item) { }
}
