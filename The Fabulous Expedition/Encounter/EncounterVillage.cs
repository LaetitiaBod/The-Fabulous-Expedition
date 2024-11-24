using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

public class EncounterVillage : Encounter
{
	private GameManager gameManager;
	private GraphicsManager graphicsManager;
	private Inventory inventory;

	private Texture2D textureBg;
	public int timerFrame = 0;
	public int currentFrame = 0;
	public int speedFrame = 5;

	private Texture2D textureBook;
	private Texture2D slot;
	private Texture2D bar;
	private Rectangle placeholderTrade;
	private float sizeSlot;

	private Button confirmButton;
	private Button cancelButton;
	private ButtonsList buttonsVillage = new ButtonsList();

	public Dictionary<ItemData, InventoryItem> goodsDict = new Dictionary<ItemData, InventoryItem>();
	public Dictionary<ItemData, InventoryItem> toBuyDict;
	public Dictionary<ItemData, InventoryItem> toSellDict;


	public ItemSlotsList goodsList = new ItemSlotsList();
	public ItemSlotsList toBuyList = new ItemSlotsList();
	public ItemSlotsList toSellList = new ItemSlotsList();

	private int balance = 0;
	public bool wasInitialized = false;

	public EncounterVillage(string _name, Vector2 coords, bool _isRevealed) : base(_name, coords, _isRevealed)
	{
		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();

		textureBg = graphicsManager.GetTexture("village-0");
		textureBook = graphicsManager.GetTexture("book");
		slot = graphicsManager.GetTexture("itemSlot");
		bar = graphicsManager.GetTexture("brownBar");

		placeholderTrade = new Rectangle();
	}

	public override void Show()
	{
		base.Show();

		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		inventory = ServiceLocator.GetService<Inventory>();

		placeholderTrade = new Rectangle(50, 50, gameManager.gameScreenWidth / 3, gameManager.gameScreenHeight / 2);
		sizeSlot = (placeholderTrade.Width - 90) / 8;
		
		confirmButton = new Button(new Rectangle(placeholderTrade.X + 50, placeholderTrade.Height - 50, placeholderTrade.Width/3, 60), "Confirm", "secondaryButton");
		cancelButton = new Button(new Rectangle(placeholderTrade.Width*2/3, placeholderTrade.Height - 50, placeholderTrade.Width/3, 60), "Cancel", "secondaryButton");
		confirmButton.fontSize = 20;
		cancelButton.fontSize = 20;
		buttonsVillage.AddButton(confirmButton);
		buttonsVillage.AddButton(cancelButton);

		toBuyDict = new Dictionary<ItemData, InventoryItem>();
		toSellDict = new Dictionary<ItemData, InventoryItem>();

		if(!wasInitialized)
			InitializeTraderInventory();
		else
			UpdateInventoryGoods();
	}

	public override void Update()
	{
		base.Update();

		timerFrame++;
		if (timerFrame >= speedFrame)
		{
			timerFrame = 0;
			currentFrame++;
			textureBg = ServiceLocator.GetService<GraphicsManager>().GetTexture("village-" + currentFrame);
			if (currentFrame >= 57) currentFrame = 0;
		}

        goodsList.Update();
		toBuyList.Update();
		toSellList.Update();

		buttonsVillage.Update();

		if (balance < 0)
		{
			confirmButton.text = "Not agree";
			confirmButton.originalColor = Color.Red;
		}
		else
		{
			confirmButton.text = "Confirm";
			confirmButton.originalColor = Color.White;

			if (confirmButton.isClicked)
			{
				balance = 0;
				foreach (var item in toBuyDict.Values)
                {
                    for (int i = 0; i < item.stackSize; i++)
                    {
						inventory.AddItemToDict(inventory.stashDict, item.data);
                    }
				}
				foreach (var item in toSellDict.Values)
				{
					for (int i = 0; i < item.stackSize; i++)
					{
						inventory.AddItemToDict(goodsDict, item.data);
					}
				}
				toBuyDict.Clear();
				toSellDict.Clear();
				UpdateInventoryToBuy();
				UpdateInventoryToSell();
				UpdateInventoryGoods();
				inventory.UpdateInventoryStash();
			}
		}
		if (cancelButton.isClicked)
		{
			balance = 0;
			foreach (var item in toSellDict.Values)
			{
				for (int i = 0; i < item.stackSize; i++)
				{
					inventory.AddItemToDict(inventory.stashDict, item.data);
				}
			}
			foreach (var item in toBuyDict.Values)
			{
				for (int i = 0; i < item.stackSize; i++)
				{
					inventory.AddItemToDict(goodsDict, item.data);
				}
			}
			toBuyDict.Clear();
			toSellDict.Clear();
			UpdateInventoryToBuy();
			UpdateInventoryToSell();
			UpdateInventoryGoods();
			inventory.UpdateInventoryStash();

			ServiceLocator.GetService<Player>().stateMachine.ChangeState(ServiceLocator.GetService<Player>().idleState);
		}
	}

	public override void Draw()
	{
		base.Draw();

		// Background
		DrawTexturePro(
			textureBg,
			new Rectangle(0f, 0f, textureBg.Width, textureBg.Height),
			new Rectangle(0, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight),
			new Vector2(0, 0), 0f, Color.White
		);
		// book
		DrawTexturePro(
			textureBook, new Rectangle(0f, 0f, textureBook.Width, textureBook.Height),
			placeholderTrade, new Vector2(0, 0), 0f, Color.White
		);

		NPatchInfo ninePatchInfoSlot = new NPatchInfo
		{
			Source = new Rectangle(0f, 0f, slot.Width, slot.Height),
			Left = 30, Top = 30, Right = 30, Bottom = 30, Layout = NPatchLayout.NinePatch
		}; 

		// TRADER
		DrawTextEx(graphicsManager.GetFont("helvetica"), "Trader", new Vector2(placeholderTrade.X + 50, placeholderTrade.Y + 20), 20, 3, Color.Black);
		for (int i = 0; i < 8; i++)
		{
			// outline
			DrawTextureNPatch(
				slot, ninePatchInfoSlot,
				new Rectangle( placeholderTrade.X+50 + i * (sizeSlot - 2), placeholderTrade.Y+50, sizeSlot, sizeSlot),
				new Vector2(0, 0), 0, Color.White);
		}
		goodsList.Draw();

		// TO BUY
		DrawTextEx(graphicsManager.GetFont("helvetica"), "To buy", new Vector2(placeholderTrade.X + 50, placeholderTrade.Y + 60 + sizeSlot), 20, 3, Color.Black);
		for (int i = 0; i < 8; i++)
		{
			// outline
			DrawTextureNPatch(
				slot, ninePatchInfoSlot,
				new Rectangle(placeholderTrade.X + 50 + i * (sizeSlot - 2), placeholderTrade.Y + 80 + sizeSlot, sizeSlot, sizeSlot),
				new Vector2(0, 0), 0, Color.White);
		}
		toBuyList.Draw();

		// BALANCE
		NPatchInfo ninePatchInfoBar= new NPatchInfo
		{
			Source = new Rectangle(0f, 0f, bar.Width, bar.Height),
			Left = 3, Top = 3, Right = 3, Bottom = 3, Layout = NPatchLayout.NinePatch
		};
		DrawTextureNPatch(bar, ninePatchInfoBar, new Rectangle(
					new Vector2(placeholderTrade.X + textureBook.Width / 2 - 210, placeholderTrade.Y + 90 + sizeSlot * 2),
					new Vector2(420, 40)), new Vector2(0, 0), 0, Color.White);
		if (balance > 0)
			DrawRectangleRec(
				new Rectangle(
					new Vector2(placeholderTrade.X + textureBook.Width/2, placeholderTrade.Y + 100 + sizeSlot * 2),
					new Vector2(Math.Min(balance, 200), 20)),
				Color.Green);
		if(balance < 0)
			DrawRectangleRec(
				new Rectangle(
					new Vector2(placeholderTrade.X + textureBook.Width / 2 - Math.Min(Math.Abs(balance), 200), placeholderTrade.Y + 100 + sizeSlot * 2),
					new Vector2(Math.Min(Math.Abs(balance), 200), 20)),
				Color.Red);
		DrawLineEx(
			new Vector2(placeholderTrade.X + textureBook.Width / 2, placeholderTrade.Y + 90 + sizeSlot * 2),
			new Vector2(placeholderTrade.X + textureBook.Width / 2, placeholderTrade.Y + 130 + sizeSlot * 2),
			5, Color.Black);
		DrawTextEx(graphicsManager.GetFont("helvetica"), balance.ToString(), new Vector2(placeholderTrade.X + 50, placeholderTrade.Y + 100 + sizeSlot * 2), 20, 3, Color.Black);

		// TO SELL
		DrawTextEx(graphicsManager.GetFont("helvetica"), "To sell", new Vector2(placeholderTrade.X + 50, placeholderTrade.Y + 150 + sizeSlot * 2), 20, 3, Color.Black);
		for (int i = 0; i < 8; i++)
		{
			DrawTextureNPatch(
				slot, ninePatchInfoSlot,
				new Rectangle(placeholderTrade.X + 50 + i * (sizeSlot - 2), placeholderTrade.Y + 170 + sizeSlot * 2, sizeSlot, sizeSlot),
				new Vector2(0, 0), 0, Color.White);
		}
		toSellList.Draw();

		// BUTTONS
		buttonsVillage.Draw();
	}

	public override void Close()
	{
		base.Close();

		currentFrame = 0;
		timerFrame = 0;

		toBuyDict.Clear();
		toSellDict.Clear();
		goodsList.Hide();
		toBuyList.Hide();
		toSellList.Hide();

		buttonsVillage.Hide();
	}

	public void InitializeTraderInventory()
	{
		List<ItemData> poolList = new List<ItemData>()
		{
			inventory.apple,
			inventory.cheese,
			inventory.goldenNecklace,
			inventory.meat
		};

		Random? randomItem = new Random();
		int numberOfItems = randomItem.Next(6, 15);

		for (int i = 0; i < numberOfItems; i++)
        {  
			inventory.AddRandomItem(poolList, goodsDict);
			UpdateInventoryGoods();
		}
		wasInitialized = true;
	}

	public override void TradeItem(ItemSlot _item)
	{
		switch(_item.origin)
		{
			case "stash":
				inventory.RemoveItemFromDict(inventory.stashDict, _item.inventoryItem.data);
				inventory.UpdateInventoryStash();

				inventory.AddItemToDict(toSellDict, _item.inventoryItem.data);
				UpdateInventoryToSell();

				balance += _item.inventoryItem.data.value;
				break;

			case "goods":
				inventory.RemoveItemFromDict(goodsDict, _item.inventoryItem.data);
				UpdateInventoryGoods();

				inventory.AddItemToDict(toBuyDict, _item.inventoryItem.data);
				UpdateInventoryToBuy();

				balance -= _item.inventoryItem.data.value;
				break;

			case "toBuy":
				inventory.RemoveItemFromDict(toBuyDict, _item.inventoryItem.data);
				UpdateInventoryToBuy();

				inventory.AddItemToDict(goodsDict, _item.inventoryItem.data);
				UpdateInventoryGoods();

				balance += _item.inventoryItem.data.value;
				break;

			case "toSell":
				inventory.RemoveItemFromDict(toSellDict, _item.inventoryItem.data);
				UpdateInventoryToSell();

				inventory.AddItemToDict(inventory.stashDict, _item.inventoryItem.data);
				inventory.UpdateInventoryStash();

				balance -= _item.inventoryItem.data.value;
				break;
		}
	}

	public void UpdateInventoryGoods()
	{
		goodsList.Hide();
		if (goodsDict != null)
		{
			for (int i = 0; i < goodsDict.Count; i++)
			{
				Rectangle slotRect = new Rectangle(
					placeholderTrade.X + 50 + i * (sizeSlot - 2),
					placeholderTrade.Y + 50,
					sizeSlot, sizeSlot);
				ItemSlot newItemSlot = new ItemSlot(slotRect, goodsDict.ElementAt(i).Value);
				goodsList.AddItem(newItemSlot, "goods");
			}
		}
	}

	public void UpdateInventoryToBuy()
	{
		toBuyList.Hide();
		if (toBuyDict != null)
		{
			for (int i = 0; i < toBuyDict.Count; i++)
			{
				Rectangle slotRect = new Rectangle(
					placeholderTrade.X + 50 + i * (sizeSlot - 2),
					placeholderTrade.Y + 80 + sizeSlot,
					sizeSlot, sizeSlot);
				ItemSlot newItemSlot = new ItemSlot(slotRect, toBuyDict.ElementAt(i).Value);
				toBuyList.AddItem(newItemSlot, "toBuy");
			}
		}
	}

	public void UpdateInventoryToSell()
	{
		toSellList.Hide();
		if (toSellDict != null)
		{
			for (int i = 0; i < toSellDict.Count; i++)
			{
				Rectangle slotRect = new Rectangle(
					placeholderTrade.X + 50 + i * (sizeSlot - 2),
					placeholderTrade.Y + 170 + sizeSlot * 2,
					sizeSlot, sizeSlot);
				ItemSlot newItemSlot = new ItemSlot(slotRect, toSellDict.ElementAt(i).Value);
				toSellList.AddItem(newItemSlot, "toSell");
			}
		}
	}
}