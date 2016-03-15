using UnityEngine;
using System.Collections;

public class CollectableItem : Item{

    private Item.ItemType itemType;

    public CollectableItem(Item.ItemType type)
    {

        itemType = type;
    }

    public Item.ItemType itemTypeName { get { return itemType; } }
}
