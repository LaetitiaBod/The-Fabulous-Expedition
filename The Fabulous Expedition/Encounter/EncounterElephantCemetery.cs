using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class EncounterElephantCemetery : Encounter
{
	private GameManager gameManager;
	private GraphicsManager graphicsManager;
	private Inventory inventory;

	private Texture2D textureBg;
	private Rectangle placeholder;
	private Texture2D slot;
	private float sizeSlot;

	private Button searchButton;
	private Button leaveButton;
	private ButtonsList buttonsFirstStep = new ButtonsList();

	private Button confirmButton;
	private Button cancelButton;
	private ButtonsList buttonsScdStep = new ButtonsList();

	public Dictionary<ItemData, InventoryItem> goodsDict = new Dictionary<ItemData, InventoryItem>();
	public ItemSlotsList goodsList = new ItemSlotsList();

	private bool wasInitialized = false;
	private bool firstChoice = false;

	public EncounterElephantCemetery(string _name, Vector2 coords, bool _isRevealed) : base(_name, coords, _isRevealed)
	{
		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		inventory = ServiceLocator.GetService<Inventory>();

		textureBg = ServiceLocator.GetService<GraphicsManager>().GetTexture("elephantCemetery");
		slot = graphicsManager.GetTexture("itemSlot");

		placeholder = new Rectangle();

		searchButton = new Button(new Rectangle(placeholder.X, placeholder.Y + 200, placeholder.Width - 100, 50), "Search the area", "secondaryButton");
		leaveButton = new Button(new Rectangle(placeholder.X, searchButton.rect.Y + 70, placeholder.Width - 100, 50), "Leave", "secondaryButton");
		confirmButton = new Button(new Rectangle(placeholder.X, placeholder.Height, placeholder.Width * 2 / 5, 60), "Take all", "secondaryButton");
		cancelButton = new Button(new Rectangle(placeholder.X + confirmButton.rect.Width + 100, placeholder.Height, placeholder.Width * 2 / 5, 60), "Leave", "secondaryButton");
	}

	public override void Show()
	{
		base.Show();

		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		inventory = ServiceLocator.GetService<Inventory>();

		placeholder = new Rectangle(100, 150, gameManager.gameScreenWidth * 0.32f, gameManager.gameScreenHeight * 2 / 3);
		sizeSlot = (placeholder.Width - 90) / 8;

		searchButton = new Button(new Rectangle(placeholder.X, placeholder.Y + 200, placeholder.Width - 100, 50), "Search the area", "secondaryButton");
		leaveButton = new Button(new Rectangle(placeholder.X, searchButton.rect.Y + 70, placeholder.Width - 100, 50), "Leave", "secondaryButton");
		searchButton.fontSize = 20;
		leaveButton.fontSize = 20;
		buttonsFirstStep.AddButton(searchButton);
		buttonsFirstStep.AddButton(leaveButton);

		confirmButton = new Button(new Rectangle(placeholder.X, placeholder.Height, placeholder.Width * 2 / 5, 60), "Take all", "secondaryButton");
		cancelButton = new Button(new Rectangle(placeholder.X + confirmButton.rect.Width + 100, placeholder.Height, placeholder.Width * 2 / 5, 60), "Leave", "secondaryButton");
		confirmButton.fontSize = 20;
		cancelButton.fontSize = 20;
		buttonsScdStep.AddButton(confirmButton);
		buttonsScdStep.AddButton(cancelButton);

		if (!wasInitialized)
			InitializeCemeteryInventory();
		else
			UpdateInventoryGoods();
	}

	public override void Update()
	{
		base.Update();

		goodsList.Update();
		buttonsScdStep.Update();
		buttonsFirstStep.Update();

		if (firstChoice)
		{
			searchButton.isDisabled = true;
			leaveButton.isDisabled = true;
		}

		if (searchButton.isClicked)
			firstChoice = true;

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
			ServiceLocator.GetService<Player>().stateMachine.ChangeState(ServiceLocator.GetService<Player>().idleState);
		}

		if (cancelButton.isClicked || leaveButton.isClicked)
			ServiceLocator.GetService<Player>().stateMachine.ChangeState(ServiceLocator.GetService<Player>().idleState);

		UpdateInventoryGoods();
		inventory.UpdateInventoryStash();
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

		NPatchInfo ninePatchInfoSlot = new NPatchInfo
		{
			Source = new Rectangle(0f, 0f, slot.Width, slot.Height),
			Left = 30, Top = 30, Right = 30, Bottom = 30, Layout = NPatchLayout.NinePatch
		};

		// description
		string description = $" This gloomy, ancient place was situated in the \n\n" +
			$"middle of a swamp, with the scattered remains \n\n" +
			$"of elephants long dead. For whatever reason, \n\n" +
			$"hundreds of these impressive animals had come \n\n" +
			$"here to die.";
		DrawTextEx(graphicsManager.GetFont("helvetica"), description, new Vector2(placeholder.X, placeholder.Y), 20, 4, Color.Black);

		// first step
		if (!firstChoice)
		{
			buttonsFirstStep.Draw();
		}
		// second step
		else if (firstChoice)
		{
			if (goodsDict.Count <= 0)
				DrawTextEx(graphicsManager.GetFont("helvetica"), "There is nothing left here.", new Vector2(placeholder.X, placeholder.Y + 250), 20, 4, Color.Black);
			else
				DrawTextEx(graphicsManager.GetFont("helvetica"), "I was sure I'd find something valuable here. All \n\nday, I combed the whole area.", new Vector2(placeholder.X, placeholder.Y + 200), 20, 4, Color.Black);

			// loot
			for (int i = 0; i < 8; i++)
			{
				DrawTextureNPatch(slot, ninePatchInfoSlot, new Rectangle(
					placeholder.X + i * (sizeSlot - 2),
					placeholder.Y + 300,
					sizeSlot, sizeSlot),
				new Vector2(0, 0), 0, Color.White);
			}
			goodsList.Draw();

			DrawTextEx(graphicsManager.GetFont("helvetica"), "Taking these items would slow me down.", new Vector2(placeholder.X, placeholder.Y + 400), 20, 4, Color.Black);

			buttonsScdStep.Draw();
		}
	}

	public override void Close()
	{
		base.Close();

		goodsList.Hide();
		buttonsScdStep.Hide();
		buttonsFirstStep.Hide();
	}

	public void InitializeCemeteryInventory()
	{
		List<ItemData> poolList = new List<ItemData>()
		{
			inventory.tusk
		};

		Random? randomItem = new Random();
		int numberOfItems = randomItem.Next(1, 3);

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
					placeholder.X + i * (sizeSlot - 2),
					placeholder.Y + 300,
					sizeSlot, sizeSlot);
				ItemSlot newItemSlot = new ItemSlot(slotRect, goodsDict.ElementAt(i).Value);
				goodsList.AddItem(newItemSlot, "goods");
			}
		}
		wasInitialized = true;
	}
}