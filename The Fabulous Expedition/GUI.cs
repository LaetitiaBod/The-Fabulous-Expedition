using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class Button
{
	public Button(Rectangle _rect, string _text, string _spriteName = "primaryButton")
	{
		rect = _rect;
		text = _text;
		spriteName = _spriteName;
	}

	public Rectangle rect { get; set; }
	public NPatchInfo ninePatchInfo { get; set; }
	public Texture2D texture { get; set; } = ServiceLocator.GetService<GraphicsManager>().GetTexture("primaryButton");
	public string spriteName { get; set; }
	public string text { get; set; } = "";
	public Color textColor { get; set; } = Color.Black;
	public int fontSize { get; set; } = 30;
	public Color originalColor { get; set; }
	public bool isClicked { get; set; } = false;
}

public class ButtonsList
{
	private List<Button> buttonList = new List<Button>();
	public Mouse mouse = new Mouse();

	public void AddButton(Button button)
	{
		button.texture = ServiceLocator.GetService<GraphicsManager>().GetTexture(button.spriteName);
		if (button.spriteName == "secondaryButton")
			button.textColor = Color.White;
		button.originalColor = button.textColor;
		button.ninePatchInfo = new NPatchInfo
		{
			Source = new Rectangle(0f, 0f, button.texture.Width, button.texture.Height),
			Left = 30,
			Top = 30,
			Right = 30,
			Bottom = 30,
			Layout = NPatchLayout.NinePatch
		};
		buttonList.Add(button);
	}

	public void Update()
	{
		Vector2 mousePos = mouse.GetMousePosition();

		foreach (Button button in buttonList)
		{
			button.isClicked = false;
			if (CheckCollisionPointRec(mousePos, button.rect))
			{
				button.textColor = Color.Red;
				if (IsMouseButtonPressed(MouseButton.Left))
				{
					button.isClicked = true;
				}
			}
			else
			{
				button.textColor = button.originalColor;
			}
		}
	}

	public void Draw()
	{
		foreach (Button button in buttonList)
		{
			DrawTextureNPatch(button.texture, button.ninePatchInfo, button.rect, new Vector2(0, 0), 0, Color.White);

			Vector2 sizeText = MeasureTextEx(ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"), button.text, button.fontSize, 5);
			DrawTextEx(
				ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"), button.text,
				new Vector2((int)(button.rect.Width - sizeText.X) / 2 + button.rect.X, (int)(button.rect.Height - sizeText.Y) / 2 + button.rect.Y),
				button.fontSize, 5, button.textColor
			);
		}
	}

	public void Hide()
	{
		foreach (Button button in buttonList)
		{
			button.textColor = button.originalColor;
		}
		buttonList.Clear();
	}
}

public class Mouse
{
	bool isCameraRelated = false;
	public Mouse(bool _isCameraRelated = false)
	{
		isCameraRelated = _isCameraRelated;
	}

	public Vector2 GetMousePosition()
	{
		Vector2 mousePos = Raylib.GetMousePosition();
		mousePos.X /= ServiceLocator.GetService<GameManager>().scale;
		mousePos.Y /= ServiceLocator.GetService<GameManager>().scale;
		if (isCameraRelated)
			mousePos = GetScreenToWorld2D(mousePos, ServiceLocator.GetService<GameManager>().camera);
		return mousePos;
	}
}

public class Bar
{
	public Bar(Rectangle _rect, string _text, float _size)
	{
		rect = _rect;
		text = _text;
		size = _size;
	}

	public Rectangle rect { get; set; }
	public NPatchInfo ninePatchInfo { get; set; }
	public Texture2D texture { get; set; } = ServiceLocator.GetService<GraphicsManager>().GetTexture("yellowBar");
	public string text { get; set; } = "";
	public float size { get; set; }

	public void Draw()
	{
		ninePatchInfo = new NPatchInfo
		{
			Source = new Rectangle(0f, 0f, texture.Width, texture.Height),
			Left = 30,
			Top = 30,
			Right = 30,
			Bottom = 30,
			Layout = NPatchLayout.NinePatch
		};
		// purple rect
		DrawRectangleRec(new Rectangle(rect.X + 9, rect.Y + 9, size, rect.Height - 9), Color.DarkPurple);
		// outline
		DrawTextureNPatch(texture, ninePatchInfo, rect, new Vector2(0, 0), 0, Color.White);
		// text
		Vector2 sizeText = MeasureTextEx(ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"), text, rect.Height / 2, 1);
		DrawTextEx(
			ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"), text,
			new Vector2((int)(rect.Width - sizeText.X) / 2 + rect.X, (int)(rect.Height - sizeText.Y) / 2 + rect.Y),
			rect.Height / 2, 1, Color.White
		);
	}
}

public class ItemSlot
{
	public ItemSlot(Rectangle _rect, InventoryItem _inventoryItem)
	{
		rect = _rect;
		inventoryItem = _inventoryItem;
	}

	public Rectangle rect { get; set; }
	public InventoryItem inventoryItem { get; set; }
	public Color textColor { get; set; } = Color.Brown;
	public int fontSize { get; set; } = 20;
	public Color originalColor { get; set; }
	public bool isOverflown { get; set; } = false;
	public bool isClicked { get; set; } = false;
	public string origin { get; set; } = "";
}

public class ItemSlotsList
{
	public List<ItemSlot> list = new List<ItemSlot>();
	public Mouse mouse = new Mouse();

	public void AddItem(ItemSlot _item, string _origin)
	{
		_item.originalColor = _item.textColor;
		_item.origin = _origin;
		list.Add(_item);
	}

	public void Update()
	{
		Vector2 mousePos = mouse.GetMousePosition();

		foreach (ItemSlot item in list)
		{
			item.isOverflown = false;
			item.isClicked = false;
			if (CheckCollisionPointRec(mousePos, item.rect))
			{
				item.textColor = Color.Red;
				item.isOverflown = true;
				if (IsMouseButtonPressed(MouseButton.Left))
				{
					item.isClicked = true;
					ServiceLocator.GetService<Inventory>().UseItem(item);
					return;
				}
			}
			else
			{
				item.isOverflown = false;
				item.textColor = item.originalColor;
			}
		}
	}

	public void Draw()
	{
		foreach (ItemSlot item in list)
		{
			DrawTexturePro(
				item.inventoryItem.data.icon,
				new Rectangle(0, 0, item.inventoryItem.data.icon.Width, item.inventoryItem.data.icon.Height),
				new Rectangle(item.rect.X+15, item.rect.Y+15, item.rect.Width-30, item.rect.Height-30),
				new Vector2(0, 0), 0, Color.White
			);
			DrawTextEx(
				ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"),
				item.inventoryItem.stackSize.ToString(),
				new Vector2(item.rect.X + 12, item.rect.Y + item.rect.Height - 30),
				item.fontSize, 1, Color.Brown
			);
			if(item.isOverflown)
			{
				Texture2D texture = ServiceLocator.GetService<GraphicsManager>().GetTexture("brownBar");
				Rectangle placeholder = new Rectangle(
					ServiceLocator.GetService<GameManager>().gameScreenWidth*4/5,
					ServiceLocator.GetService<GameManager>().gameScreenHeight - 250,
					ServiceLocator.GetService<GameManager>().gameScreenWidth/7, 150);
				NPatchInfo ninePatchInfoBar = new NPatchInfo
				{
					Source = new Rectangle(0f, 0f, texture.Width, texture.Height),
					Left = 3, Top = 3, Right = 3, Bottom = 3, Layout = NPatchLayout.NinePatch
				};
				DrawTextureNPatch(texture, ninePatchInfoBar, placeholder, new Vector2(0, 0), 0, Color.White);
				string description = $"" +
					$"{item.inventoryItem.data.name}\n\n" +
					$"Value : {item.inventoryItem.data.value}\n\n" +
					$"Fame : {item.inventoryItem.data.fame}";
				if (item.inventoryItem.data.type == ItemType.Food)
					description += $"\n\nFood amount : {item.inventoryItem.data.foodAmount}";

				DrawTextEx(
					ServiceLocator.GetService<GraphicsManager>().GetFont("helvetica"), description,
					new Vector2(placeholder.X + 20, placeholder.Y + 20), item.fontSize, 1, Color.White);
				DrawRectangleLinesEx(item.rect, 2, Color.Red);
			}
		}
	}

	public void Hide()
	{
		foreach (ItemSlot item in list)
		{
			item.textColor = item.originalColor;
		}
		list.Clear();
	}
}