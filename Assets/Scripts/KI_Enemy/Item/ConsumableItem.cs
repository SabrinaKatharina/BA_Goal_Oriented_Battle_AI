using UnityEngine;
using System.Collections;

public class ConsumableItem : Item{

    private Item.ItemType itemType;
    private string itemName;
    private int value;

	public ConsumableItem(string name)
    {
        itemName = name;

        switch (name)
        {
            // Type Heal:
            case "HumanMeat": itemType = Item.ItemType.BIG_HEAL; value = 50; break;
            case "ChickenWing": itemType = Item.ItemType.SMALL_HEAL; value = 15; break;
            case "WoodenShield": itemType = Item.ItemType.DEF_BONUS; value = 15; break;
            case "WoodenStake": itemType = Item.ItemType.STR_BONUS; value = 10; break;

        }


    }

    public ConsumableItem(Goal.GoalType type)
    {
        switch (type)
        {
            // Type Heal:
            case Goal.GoalType.BigHeal: itemType = ItemType.BIG_HEAL; itemName = "HumanMeat"; value = 50; break;
            case Goal.GoalType.SmallHeal: itemType = ItemType.SMALL_HEAL; itemName = "ChickenWing"; value = 15; break;
            case Goal.GoalType.DefendBonus: itemType = ItemType.DEF_BONUS; itemName = "WoodenShield"; value = 5; break;
            case Goal.GoalType.StrengthBonus: itemType = ItemType.STR_BONUS; itemName = "WoodenStake"; value = 3; break;

        }


    }



    public ItemType ItemTypeName { get { return itemType; } }

    public string ItemName { get { return itemName; } }

    public int Value { get { return value; } }
}
