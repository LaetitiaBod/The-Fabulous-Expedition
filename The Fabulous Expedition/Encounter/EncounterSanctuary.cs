using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class EncounterSanctuary : Encounter
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

	private Button confirmButton;
	private Button cancelButton;
	private ButtonsList buttonsSanctuary = new ButtonsList();

	public Dictionary<ItemData, InventoryItem> goodsDict = new Dictionary<ItemData, InventoryItem>();
	public ItemSlotsList goodsList = new ItemSlotsList();

	public bool wasInitialized = false;

	public EncounterSanctuary(string _name, Vector2 coords, bool _isRevealed) : base(_name, coords, _isRevealed)
	{
		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		
		textureBg = ServiceLocator.GetService<GraphicsManager>().GetTexture("sanctuary-0");
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

		placeholder = new Rectangle(50, 50, gameManager.gameScreenWidth / 3, gameManager.gameScreenHeight / 2);
		sizeSlot = (placeholder.Width - 90) / 8;

		confirmButton = new Button(new Rectangle(placeholder.X + 50, placeholder.Height - 50, placeholder.Width / 3, 60), "Take all", "secondaryButton");
		cancelButton = new Button(new Rectangle(placeholder.Width * 2 / 3, placeholder.Height - 50, placeholder.Width / 3, 60), "Leave", "secondaryButton");
		confirmButton.fontSize = 20;
		cancelButton.fontSize = 20;
		buttonsSanctuary.AddButton(confirmButton);
		buttonsSanctuary.AddButton(cancelButton);

		if (!wasInitialized)
			InitializeSanctuaryInventory();
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
			textureBg = ServiceLocator.GetService<GraphicsManager>().GetTexture("sanctuary-" + currentFrame);
			if (currentFrame >= 57) currentFrame = 0;
		}

		goodsList.Update();
		buttonsSanctuary.Update();

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
		if (cancelButton.isClicked)
			ServiceLocator.GetService<Player>().stateMachine.ChangeState(ServiceLocator.GetService<Player>().idleState);
	}

	public override void Draw()
	{
		base.Draw();

		// background
		DrawTexturePro(
			textureBg,
			new Rectangle(0f, 0f, textureBg.Width, textureBg.Height),
			new Rectangle(0, 0, gameManager.gameScreenWidth, gameManager.gameScreenHeight),
			new Vector2(0, 0), 0f, Color.White
		);

		// book
		DrawTexturePro(
			textureBook,
			new Rectangle(0f, 0f, textureBook.Width, textureBook.Height),
			placeholder,
			new Vector2(0, 0), 0f, Color.White
		);

		NPatchInfo ninePatchInfoSlot = new NPatchInfo
		{
			Source = new Rectangle(0f, 0f, slot.Width, slot.Height),
			Left = 30, Top = 30, Right = 30, Bottom = 30, Layout = NPatchLayout.NinePatch
		};

		// description
		string description = $"  Lorem ipsum odor amet, consectetuer\n\n" +
			$"adipiscing elit. Dolor per arcu nullam \n\n" +
			$"aliquet ridiculus consectetur? Netus eget \n\n" +
			$"nibh class nostra mattis mi elementum elit. \n\n" +
			$"Donec primis malesuada massa maecenas \n\n" +
			$"mattis nibh pretium praesent parturient. \n\n\n\n" +
			$"  Congue scelerisque montes hendrerit \n\n" +
			$"aptent diam nullam.";
		if (goodsDict.Count <= 0)
			description = "There is nothing left here.";
		DrawTextEx(graphicsManager.GetFont("helvetica"), description, new Vector2(placeholder.X + 50, placeholder.Y + 20), 20, 4, Color.Black);
		// loot
		for (int i = 0; i < 8; i++)
		{
			DrawTextureNPatch(slot, ninePatchInfoSlot, new Rectangle(
				placeholder.X + 50 + i * (sizeSlot - 2),
				placeholder.Y + 300,
				sizeSlot, sizeSlot),
			new Vector2(0, 0), 0, Color.White);
		}
		goodsList.Draw();

		// buttons
		buttonsSanctuary.Draw();
	}

	public override void Close()
	{
		base.Close();

		currentFrame = 0;
		timerFrame = 0;

		goodsList.Hide();
		buttonsSanctuary.Hide();
	}

	public void InitializeSanctuaryInventory()
	{
		List<ItemData> poolList = new List<ItemData>()
		{
			inventory.goldenPlate,
			inventory.goldenMask,
			inventory.goldenNecklace,
			inventory.goldenLama
		};

		int numberOfItems = 2;

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
					placeholder.Y + 300,
					sizeSlot, sizeSlot);
				ItemSlot newItemSlot = new ItemSlot(slotRect, goodsDict.ElementAt(i).Value);
				goodsList.AddItem(newItemSlot, "goods");
			}
		}
		wasInitialized = true;
	}
}