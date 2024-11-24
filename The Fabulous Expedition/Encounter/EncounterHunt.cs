using Raylib_cs;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using TiledSharp;
using static Raylib_cs.Raylib;

public class EncounterHunt : Encounter
{
	private GameManager gameManager;
	private GraphicsManager graphicsManager;
	private Inventory inventory;

	private Texture2D textureBg;
	public int timerFrame = 0;
	public int currentFrame = 0;
	public int speedFrame = 5;

	private Texture2D textureBook;
	private Rectangle placeholder;
	private Texture2D slot;
	private float sizeSlot;
	private string description;

	private Button confirmButton;
	private Button cancelButton;
	private ButtonsList buttonsHunt = new ButtonsList();

	public Dictionary<ItemData, InventoryItem> goodsDict = new Dictionary<ItemData, InventoryItem>();
	public ItemSlotsList goodsList = new ItemSlotsList();

	private bool isSuccess = false;

	public EncounterHunt(string _name, Vector2 coords, bool _isRevealed) : base(_name, coords, _isRevealed)
	{
		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();

		textureBg = ServiceLocator.GetService<GraphicsManager>().GetTexture("hunt-0");
		textureBook = graphicsManager.GetTexture("book");
		slot = graphicsManager.GetTexture("itemSlot");

		placeholder = new Rectangle();
	}

	public override void Show()
	{
		base.Show();

		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		inventory = ServiceLocator.GetService<Inventory>();

		placeholder = new Rectangle(50, 100, gameManager.gameScreenWidth / 3, gameManager.gameScreenHeight / 3);
		sizeSlot = (placeholder.Width - 90) / 8;

		confirmButton = new Button(new Rectangle(placeholder.X + 50, placeholder.Height, placeholder.Width / 3, 60), "Take all", "secondaryButton");
		cancelButton = new Button(new Rectangle(placeholder.Width * 2 / 3, placeholder.Height, placeholder.Width / 3, 60), "Leave", "secondaryButton");
		confirmButton.fontSize = 20;
		cancelButton.fontSize = 20;
		buttonsHunt.AddButton(confirmButton);
		buttonsHunt.AddButton(cancelButton);

		isSuccess = IsSuccess();

		if (isSuccess)
			InitializeHuntInventory();
	}

	public override void Update()
	{
		base.Update();

		timerFrame++;
		if (timerFrame >= speedFrame)
		{
			timerFrame = 0;
			currentFrame++;
			textureBg = ServiceLocator.GetService<GraphicsManager>().GetTexture("hunt-" + currentFrame);
			if (currentFrame >= 57) currentFrame = 0;
		}

		goodsList.Update();
		buttonsHunt.Update();

		if(isSuccess)
		{
			cancelButton.rect = new Rectangle(placeholder.Width * 2 / 3, placeholder.Height, placeholder.Width / 3, 60);

			if (confirmButton.isClicked)
			{
				foreach (var item in goodsDict.Values)
				{
					for (int i = 0; i < item.stackSize; i++)
					{
						inventory.AddItemToDict(inventory.stashDict, item.data);
					}
				}
				goodsDict.Clear();
				UpdateInventoryGoods();
				inventory.UpdateInventoryStash();
				ServiceLocator.GetService<Player>().stateMachine.ChangeState(ServiceLocator.GetService<Player>().idleState);
			}
		} 
		else
		{
			buttonsHunt.buttonList.Remove(confirmButton);
			cancelButton.rect = new Rectangle(placeholder.X + 50, placeholder.Height, placeholder.Width / 3, 60);
		}

		if (cancelButton.isClicked)
			ServiceLocator.GetService<Player>().stateMachine.ChangeState(ServiceLocator.GetService<Player>().idleState);

		UpdateInventoryGoods();
		inventory.UpdateInventoryStash();
	}

	public override void Draw()
	{
		base.Draw();
		ClearBackground(Color.Black);

		// background
		DrawTexturePro(
			textureBg,
			new Rectangle(0f, 0f, textureBg.Width, textureBg.Height),
			new Rectangle(0, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight),
			new Vector2(0, 0), 0f, new Color(255, 255, 255, 100)
		);
		DrawTexturePro(
			textureBg,
			new Rectangle(0f, 0f, textureBg.Width, textureBg.Height),
			new Rectangle(gameManager.gameScreenWidth*.1f, gameManager.gameScreenHeight * .1f, gameManager.gameScreenWidth*.8f, gameManager.gameScreenHeight*.8f),
			new Vector2(0, 0), 0f, Color.White
		);

		// book
		DrawTexturePro(
			textureBook, new Rectangle(0f, 0f, textureBook.Width, textureBook.Height),
			placeholder, new Vector2(0, 0), 0f, Color.White
		);

		NPatchInfo ninePatchInfoSlot = new NPatchInfo
		{
			Source = new Rectangle(0f, 0f, slot.Width, slot.Height),
			Left = 30, Top = 30, Right = 30, Bottom = 30, Layout = NPatchLayout.NinePatch
		};

		if(isSuccess)
		{
			description = $"  In this region, the woods are full of\n\n " +
				$"wolves. You spot a lone wolf and decide\n\n " +
				$"to go hunting. You're desperately short\n\n " +
				$"of food.";
			if (goodsDict.Count <= 0)
				description = "There is nothing left here.";
			// loot
			for (int i = 0; i < 8; i++)
			{
				DrawTextureNPatch(slot, ninePatchInfoSlot, new Rectangle(
					placeholder.X + 50 + i * (sizeSlot - 2),
					placeholder.Y + 160,
					sizeSlot, sizeSlot),
				new Vector2(0, 0), 0, Color.White);
			}
			goodsList.Draw();
		}
		else
		{
			description = "  In this region, the woods are full of\n\n " +
				"wolves. You spot a lone wolf and decide\n\n " +
				"to go hunting. Unfortunately, he sees you\n\n " +
				"and runs off. He'll be more careful in \n\n" +
				"future, so don't count on him to feed you.";
		}
		
		DrawTextEx(graphicsManager.GetFont("helvetica"), description, new Vector2(placeholder.X + 50, placeholder.Y + 20), 20, 4, Color.Black);

		// buttons
		buttonsHunt.Draw();
	}

	public override void Close()
	{
		base.Close();

		currentFrame = 0;
		timerFrame = 0;

		goodsList.Hide();
		buttonsHunt.Hide();

		ServiceLocator.GetService<Map>().encounterList.Remove(this);

		for (int i = 0; i < ServiceLocator.GetService<Map>().tmxMap.Layers[1].Tiles.Count; i++)
		{
			TmxLayerTile currentTile = ServiceLocator.GetService<Map>().tmxMap.Layers[1].Tiles[i];
			int gid = currentTile.Gid;

			if (coords.X == currentTile.X && coords.Y == currentTile.Y)
				ServiceLocator.GetService<Map>().tmxMap.Layers[1].Tiles.Remove(currentTile);
		}
	}

	public void InitializeHuntInventory()
	{
		List<ItemData> poolList = new List<ItemData>()
		{
			inventory.meat,
			inventory.wolfSkin,
			inventory.tooth
		};

		Random randomNumberOfItems = new Random();
		int numberOfItems = randomNumberOfItems.Next(3, 7);

		for (int i = 0; i < numberOfItems; i++)
		{
			inventory.AddRandomItem(poolList, goodsDict);
			UpdateInventoryGoods();
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
					placeholder.X + 50 + i * (sizeSlot - 2),
					placeholder.Y + 160,
					sizeSlot, sizeSlot);
				ItemSlot newItemSlot = new ItemSlot(slotRect, goodsDict.ElementAt(i).Value);
				goodsList.AddItem(newItemSlot, "goods");
			}
		}
	}

	private bool IsSuccess()
	{
		Random random = new Random();
		int dice = random.Next(6);

		if (dice < 0)
			return false;

		return true;
	}
}