using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class Inventory
{
	private GraphicsManager graphicsManager = ServiceLocator.GetService<GraphicsManager>();
	private GameManager gameManager = ServiceLocator.GetService<GameManager>();
	private Player player = ServiceLocator.GetService<Player>();

	private List<InventoryItem> startingItems = new List<InventoryItem>();
	public Dictionary<ItemData, InventoryItem> stashDict = new Dictionary<ItemData, InventoryItem>();

	public int inventoryItemSlot = 10;
	private float sizeSlot = 90;

	public ItemData apple = new ItemData();
	public ItemData meat = new ItemData();
	public ItemData cheese = new ItemData();
	public ItemData goldenPlate = new ItemData();
	public ItemData goldenNecklace = new ItemData();
	public ItemData goldenLama = new ItemData();
	public ItemData goldenMask = new ItemData();
	public ItemData mummy = new ItemData();
	public ItemData torch = new ItemData();
	public ItemData tusk = new ItemData();
	public ItemData wolfSkin = new ItemData();
	public ItemData tooth = new ItemData();

	public ItemSlotsList itemSlotsList = new ItemSlotsList();

	public void Show()
	{
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		gameManager = ServiceLocator.GetService<GameManager>();
		player = ServiceLocator.GetService<Player>();

		InitializeItemDatabase();

		stashDict = new Dictionary<ItemData, InventoryItem>();
		startingItems = new List<InventoryItem>
		{
			new InventoryItem(apple, 5),
			new InventoryItem(meat, 2),
			new InventoryItem(cheese, 5),
			new InventoryItem(torch, 2),
			new InventoryItem(goldenLama, 1),
			new InventoryItem(goldenMask, 1),
			//new InventoryItem(goldenNecklace, 1),
			//new InventoryItem(goldenPlate, 1),
			//new InventoryItem(mummy, 1),
			//new InventoryItem(tusk, 1),
		};

		AddStartingItems();

		sizeSlot = gameManager.gameScreenHeight * 1 / 12;
		UpdateInventoryStash();
	}

	public void Update() {
		Mouse mouse = new Mouse();
		Vector2 mousePos = mouse.GetMousePosition();

		itemSlotsList.Update();
	}

	public void Draw()
	{
		Texture2D outline = graphicsManager.GetTexture("itemSlot");
		NPatchInfo ninePatchInfo = new NPatchInfo
		{
			Source = new Rectangle(0f, 0f, outline.Width, outline.Height),
			Left = 30, Top = 30, Right = 30, Bottom = 30, Layout = NPatchLayout.NinePatch
		};
		// draw inventory slots
        for (int i = 0; i < stashDict.Count; i++)
        {
			// outline
			DrawTextureNPatch(outline, ninePatchInfo, new Rectangle(
				gameManager.gameScreenWidth / 2 + (i - stashDict.Count / 2) * (sizeSlot - 2),
				gameManager.gameScreenHeight - sizeSlot,
				sizeSlot,
				sizeSlot),
			new Vector2(0, 0), 0, Color.White);
            
        }
		// draw items
		itemSlotsList.Draw();

		if(IsOverloaded())
		{
			DrawRectangleLinesEx(new Rectangle(
				gameManager.gameScreenWidth / 2 - (stashDict.Count / 2) * (sizeSlot - 2),
				gameManager.gameScreenHeight - sizeSlot,
				(sizeSlot-2) * stashDict.Count,
				sizeSlot), 2, Color.Red);
			string warningText = $"{stashDict.Count - inventoryItemSlot} extra object slots!";
			Vector2 sizeText = MeasureTextEx(graphicsManager.GetFont("helvetica"), warningText, 30, 4);
			DrawTextEx(graphicsManager.GetFont("helvetica"), warningText, new Vector2((gameManager.gameScreenWidth - sizeText.X) / 2, gameManager.gameScreenHeight - sizeSlot - 40), 30, 4, Color.Red);
		}
	}

	public void Hide() {
		itemSlotsList.Hide();
	}

	public void InitializeItemDatabase()
	{
		apple	= new ItemData(ItemType.Food, graphicsManager.GetTexture("apple"), "Apple", 5, 0, 5);
		meat	= new ItemData(ItemType.Food, graphicsManager.GetTexture("meat"), "Meat", 10, 0, 10);
		cheese	= new ItemData(ItemType.Food, graphicsManager.GetTexture("cheese"), "Cheese", 30, 0, 30);

		goldenPlate = new ItemData(ItemType.Loot, graphicsManager.GetTexture("goldenPlate"), "Golden plate", 5, 20, 0);
		goldenNecklace = new ItemData(ItemType.Loot, graphicsManager.GetTexture("goldenNecklace"), "Golden necklace", 20, 5, 0);
		goldenLama = new ItemData(ItemType.Loot, graphicsManager.GetTexture("goldenLama"), "Golden lama", 15, 50, 0);
		goldenMask = new ItemData(ItemType.Loot, graphicsManager.GetTexture("goldenMask"), "Golden mask", 40, 70, 0);
		mummy = new ItemData(ItemType.Loot, graphicsManager.GetTexture("mummy"), "Mummy", 10, 100, 0);
		torch = new ItemData(ItemType.Loot, graphicsManager.GetTexture("torch"), "Torch", 5, 0, 0);
		tusk = new ItemData(ItemType.Loot, graphicsManager.GetTexture("tusk"), "Tusk", 60, 20, 0);
		tooth = new ItemData(ItemType.Loot, graphicsManager.GetTexture("tooth"), "Tooth", 15, 5, 0);
		wolfSkin = new ItemData(ItemType.Loot, graphicsManager.GetTexture("wolfSkin"), "Wolf skin", 30, 10, 0);
	}

	private void AddStartingItems()
	{
		foreach (var item in startingItems)
		{
			stashDict.Add(item.data, item);
		}
	}

	public void AddItemToDict(Dictionary<ItemData, InventoryItem> dict, ItemData _item)
	{
		if (dict.TryGetValue(_item, out InventoryItem? value))
		{
			value.AddStack();
		}
		else
		{
			InventoryItem newItem = new InventoryItem(_item);
			dict.Add(_item, newItem);
		}
	}

	public void RemoveItemFromDict(Dictionary<ItemData, InventoryItem> dict, ItemData _item)
	{
		if (dict.TryGetValue(_item, out InventoryItem? value))
		{
			if (value.stackSize <= 1)
			{
				dict.Remove(_item);
			}
			else
				value.RemoveStack();
		}
		UpdateInventoryStash();
	}

	public bool IsOverloaded()
	{
		if (stashDict.Count > inventoryItemSlot)
		{
			return true;
		}
		return false;
	}

	public void UpdateInventoryStash()
	{
		itemSlotsList.Hide();
		if (stashDict != null)
		{
			for (int i = 0; i < stashDict.Count; i++)
			{
				Rectangle slotRect = new Rectangle(
						gameManager.gameScreenWidth / 2 + (i - stashDict.Count / 2) * (sizeSlot - 2),
						gameManager.gameScreenHeight - sizeSlot,
						sizeSlot, sizeSlot);
				ItemSlot newItemSlot = new ItemSlot(slotRect, stashDict.ElementAt(i).Value);
				itemSlotsList.AddItem(newItemSlot, "stash");
			}
		}
	}

	public void UseItem(ItemSlot _item)
	{
		if(_item.inventoryItem.data.type == ItemType.Food)
		{
			if(player.encounterState.currentEncounter == null)
			{
				EatFood(_item.inventoryItem);
			}
			else
			{
				if(player.encounterState.currentEncounter.name == "Village") {
					player.encounterState.currentEncounter.TradeItem(_item);
				}
			}
		}
		else if (_item.inventoryItem.data.type == ItemType.Loot)
		{
			if (player.encounterState.currentEncounter == null)
			{
			}
			else
			{
				if (player.encounterState.currentEncounter.name == "Village")
				{
					player.encounterState.currentEncounter.TradeItem(_item);
				}
			}
		}
		else
		{

		}
	}

	private void EatFood(InventoryItem _item)
	{
		if(player.currentFood != player.foodMax)
		{
			player.currentFood += _item.data.foodAmount;
			RemoveItemFromDict(stashDict, _item.data);
			UpdateInventoryStash();
		}
		if (player.currentFood > player.foodMax) player.currentFood = 100;
	}

	public void AddRandomItem(List<ItemData> poolList, Dictionary<ItemData, InventoryItem> dict)
	{
		Random? randomItem = new Random();
		int index = randomItem.Next(poolList.Count);

		AddItemToDict(dict, poolList[index]);
	}
}