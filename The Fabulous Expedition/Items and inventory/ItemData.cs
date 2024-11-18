using Raylib_cs;

public enum ItemType
{
	Food,
	Loot
}

public class ItemData
{
	public ItemType type;
	public Texture2D icon;
	public string name;
	public int value;
	public int fame;
	public int foodAmount;

	public ItemData()
	{
		type = ItemType.Food;
		icon = new Texture2D();
		name = "";
		value = 0;
		foodAmount = 0;
	}

	public ItemData(ItemType _type, Texture2D _icon, string _name, int _value, int _fame, int _foodAmount)
	{
		type = _type;
		icon = _icon;
		name = _name;
		value = _value;
		fame = _fame;
		foodAmount = _foodAmount;
	}
}
