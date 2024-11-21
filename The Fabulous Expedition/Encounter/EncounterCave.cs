using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class EncounterCave : Encounter
{
	private GameManager gameManager;
	private GraphicsManager graphicsManager;
	private Inventory inventory;

	private Texture2D textureBg;
	private Rectangle placeholder;
	private Texture2D slot;
	private float sizeSlot;

	private Button torchButton;
	private Button blindButton;
	private Button leaveButton;
	private ButtonsList buttonsFirstStep = new ButtonsList();

	private Button confirmButton;
	private Button cancelButton;
	private ButtonsList buttonsScdStep = new ButtonsList();

	public Dictionary<ItemData, InventoryItem> goodsDict = new Dictionary<ItemData, InventoryItem>();
	public ItemSlotsList goodsList = new ItemSlotsList();

	private bool wasInitialized = false;
	private bool firstChoice = false;
	private bool isSuccess;

	public EncounterCave(string _name, Vector2 coords, bool _isRevealed) : base(_name, coords, _isRevealed)
	{
		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();

		textureBg = ServiceLocator.GetService<GraphicsManager>().GetTexture("caveBg");
		slot = graphicsManager.GetTexture("itemSlot");

		placeholder = new Rectangle();
	}

	public override void Show()
	{
		base.Show();

		gameManager = ServiceLocator.GetService<GameManager>();
		graphicsManager = ServiceLocator.GetService<GraphicsManager>();
		inventory = ServiceLocator.GetService<Inventory>();

		placeholder = new Rectangle(100, 150, gameManager.gameScreenWidth * 0.32f, gameManager.gameScreenHeight * 2 / 3);
		sizeSlot = (placeholder.Width - 90) / 8;

		torchButton = new Button(new Rectangle(placeholder.X, placeholder.Y + 250, placeholder.Width-100, 50), "Explore with a torch", "secondaryButton");
		blindButton = new Button(new Rectangle(placeholder.X, torchButton.rect.Y + 70, placeholder.Width-100, 50), "Exlore in the darkness", "secondaryButton");
		leaveButton = new Button(new Rectangle(placeholder.X, blindButton.rect.Y + 70, placeholder.Width-100, 50), "Leave", "secondaryButton");
		torchButton.fontSize = 20;
		blindButton.fontSize = 20;
		leaveButton.fontSize = 20;
		buttonsFirstStep.AddButton(torchButton);
		buttonsFirstStep.AddButton(blindButton);
		buttonsFirstStep.AddButton(leaveButton);

		confirmButton = new Button(new Rectangle(placeholder.X, placeholder.Height, placeholder.Width * 2 / 5, 60), "Take all", "secondaryButton");
		cancelButton = new Button(new Rectangle(placeholder.X + confirmButton.rect.Width + 100, placeholder.Height, placeholder.Width * 2 / 5, 60), "Leave", "secondaryButton");
		confirmButton.fontSize = 20;
		cancelButton.fontSize = 20;
		buttonsScdStep.AddButton(confirmButton);
		buttonsScdStep.AddButton(cancelButton);

		if (!wasInitialized)
			InitializeCaveInventory();
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
			torchButton.isDisabled = true;
			blindButton.isDisabled = true;
			leaveButton.isDisabled = true;
			textureBg = ServiceLocator.GetService<GraphicsManager>().GetTexture("caveInside");
		}

		if (!inventory.stashDict.ContainsKey(inventory.torch))
			torchButton.isDisabled = true;
		
		if (torchButton.isClicked || blindButton.isClicked)
		{
			isSuccess = IsSuccess();

			// remove the torch item
			if(torchButton.isClicked)
				inventory.RemoveItemFromDict(inventory.stashDict, inventory.torch);
			
			// fail
			if (!isSuccess)
				RemoveRandomItem();

			firstChoice = true;
		}

        if (isSuccess)
		{
			buttonsScdStep.AddButton(confirmButton);
			cancelButton.rect = new Rectangle(placeholder.X + confirmButton.rect.Width + 100, placeholder.Height, placeholder.Width * 2 / 5, 60);

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
		}
		else
		{
			buttonsScdStep.buttonList.Remove(confirmButton);
			cancelButton.rect = new Rectangle(placeholder.X, placeholder.Height, placeholder.Width * 2 / 5, 60);
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
		string description = $"  Lorem ipsum odor amet, consectetuer \n\n" +
			$"adipiscing elit. Dolor per arcu nullam  aliquet \n\n" +
			$"ridiculus consectetur? Netus eget nibh class \n\n" +
			$"nostra mattis mi elementum elit. Donec primis \n\n" +
			$"malesuada massa maecenas mattis nibh pretium \n\n" +
			$"praesent parturient.";
		DrawTextEx(graphicsManager.GetFont("helvetica"), description, new Vector2(placeholder.X, placeholder.Y), 20, 4, Color.Black);
		
		// first step
		if(!firstChoice)
		{
			buttonsFirstStep.Draw();
		}
		// second step
		else if(firstChoice)
		{
			if (isSuccess)
			{
				if (goodsDict.Count <= 0)
					DrawTextEx(graphicsManager.GetFont("helvetica"), "There is nothing left here.", new Vector2(placeholder.X, placeholder.Y + 250), 20, 4, Color.Black);
				else
					DrawTextEx(graphicsManager.GetFont("helvetica"), "You found some loot :", new Vector2(placeholder.X, placeholder.Y + 250), 20, 4, Color.Black);

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

			}
			else
			{
				DrawTextEx(graphicsManager.GetFont("helvetica"), "You get lost in the cave and you lost an item", new Vector2(placeholder.X, placeholder.Y + 250), 20, 4, Color.Black);
			}
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

	public void InitializeCaveInventory()
	{
		List<ItemData> poolList = new List<ItemData>()
		{
			inventory.goldenPlate,
			inventory.goldenMask,
			inventory.goldenNecklace,
			inventory.goldenLama
		};

		Random? randomItem = new Random();
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
					placeholder.X + i * (sizeSlot - 2),
					placeholder.Y + 300,
					sizeSlot, sizeSlot);
				ItemSlot newItemSlot = new ItemSlot(slotRect, goodsDict.ElementAt(i).Value);
				goodsList.AddItem(newItemSlot, "goods");
			}
		}
		wasInitialized = true;
	}

	private bool IsSuccess()
	{
		Random random = new Random();
		int dice = random.Next(6);

		if ((blindButton.isClicked && dice < 2) || (torchButton.isClicked && dice == 0))
			return false;

		return true;
	}

	public void RemoveRandomItem()
	{
		Random randomItem = new Random();
		int index = randomItem.Next(inventory.stashDict.Keys.Count);

		inventory.RemoveItemFromDict(inventory.stashDict, inventory.stashDict.ElementAt(index).Key);
		inventory.UpdateInventoryStash();
	}
}