using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InventoryItem
{
	public ItemData data;
	public int stackSize;

	public InventoryItem(ItemData _newItemData)
	{
		data = _newItemData;
		AddStack();
	}

	public InventoryItem(ItemData data, int stackSize) : this(data)
	{
		this.stackSize = stackSize;
	}

	public void AddStack() => stackSize++;
	public void RemoveStack() => stackSize--;
}
