using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : MonoBehaviour {

    public List<ConsumableItem> consumableItems;   // Items, die das Monster im Kampf verwenden kann
    public List<CollectableItem> collectableItems; // Items, die das Monster während der Simulation sammeln kann
    private ConsumableItem usingItem;              // Item, welches das Monster verwendet im Kamp
    private bool isItemUsed;
   

	// Use this for initialization
	void Start () {

        consumableItems = new List<ConsumableItem>();
        collectableItems = new List<CollectableItem>();
    /*  for (int i = 0; i < 2; ++i)
        {

            consumableItems.Add(new ConsumableItem("HumanMeat"));
            consumableItems.Add(new ConsumableItem("WoodenShield"));

        }*/
        usingItem = null;
        isItemUsed = false;
	}
	
	// Update is called once per frame
	void Update () {

        // prüfe, ob das ausgewählte Item im Kampf verwendet wurde; falls ja, lösche es aus der Liste
        if (isItemUsed)
        {
            consumableItems.Remove(usingItem);
            isItemUsed = false; // reset
            usingItem = null;
        }
	}

    public ConsumableItem UsingItem { get { return usingItem; } set { usingItem = value; } }

    public bool IsItemUsed { get { return isItemUsed; }  set { isItemUsed = value; } }

    public void craftHumanMeat()
    {
        // prüfe, ob in collectable Items mindestens 3 Flesh-Items vorhanden sind
        int amountOfFlesh = collectableItems.Count(item => item.itemTypeName == CollectableItem.ItemType.MEAT);
        if(amountOfFlesh >= 3)
        {
            for(int i = 0; i < 3; ++i)
            {
                CollectableItem neededFlesh = collectableItems.Find(item => item.itemTypeName == CollectableItem.ItemType.MEAT); // finde das erste Flesh-Item aus der Liste
                collectableItems.Remove(neededFlesh); // lösche das Flesh-Item aus der Liste
                
            }

            // erzeuge ein Human Flesh

            consumableItems.Add(new ConsumableItem("HumanFlesh"));
            this.GetComponentInParent<Monster_Behaviour>().updateMonsterLog(this.GetComponentInParent<Monster_Behaviour>().name + " crafted HumanFlesh.");
            //Debug.Log(this.GetComponentInParent<Monster_Behaviour>().name + " crafted HumanFlesh.");
        }
        else
        {

            Debug.Log(this.GetComponentInParent<Monster_Behaviour>().name + ": Error, there are no 3 pieces of Flesh. Cannot create HumanMeat.");
        }
    }

    public void craftChickenWing()
    {
        // prüfe, ob in collectable Items mindestens 1 Flesh-Item vorhanden sind
        int amountOfFlesh = collectableItems.Count(item => item.itemTypeName == CollectableItem.ItemType.MEAT);
        if (amountOfFlesh >= 1)
        {
           
            CollectableItem neededFlesh = collectableItems.Find(item => item.itemTypeName == CollectableItem.ItemType.MEAT); // finde das erste Flesh-Item aus der Liste
            collectableItems.Remove(neededFlesh); // lösche das Flesh-Item aus der Liste
            // erzeuge ein Chicken Wing
            consumableItems.Add(new ConsumableItem("ChickenWing"));
            //Debug.Log(this.GetComponentInParent<Monster_Behaviour>().name + " crafted ChickenWing.");
            this.GetComponentInParent<Monster_Behaviour>().updateMonsterLog(this.GetComponentInParent<Monster_Behaviour>().name + " crafted ChickenWing.");
        }
        else
        {

            Debug.Log(this.GetComponentInParent<Monster_Behaviour>().name + ": Error, there are no pieces of Flesh. Cannot create ChickenWing.");
        }

    }

    public void crafWoodenShield()
    {
        // prüfe, ob in collectable Items mindestens 2 Wood-Items vorhanden sind
        int amountOfWood = collectableItems.Count(item => item.itemTypeName == CollectableItem.ItemType.WOOD);
        if (amountOfWood >= 2)
        {
            for (int i = 0; i < 2; i++)
            {
                CollectableItem neededWood = collectableItems.Find(item => item.itemTypeName == CollectableItem.ItemType.WOOD); // finde das erste Wood-Item aus der Liste
                collectableItems.Remove(neededWood); // lösche das Wood-Item aus der Liste
            }
            // erzeuge ein WoodenShield
            consumableItems.Add(new ConsumableItem("WoodenShield"));
            //Debug.Log(this.GetComponentInParent<Monster_Behaviour>().name + " crafted WoodenShield.");
            this.GetComponentInParent<Monster_Behaviour>().updateMonsterLog(this.GetComponentInParent<Monster_Behaviour>().name + " crafted ChickenWing.");
        }
        else
        {

            Debug.Log(this.GetComponentInParent<Monster_Behaviour>().name + ": Error, there are no 2 pieces of Wood. Cannot create WoodenShield.");
        }
    }

    public void crafWoodenStake()
    {
        // prüfe, ob in collectable Items mindestens 2 Wood-Items vorhanden sind
        int amountOfWood = collectableItems.Count(item => item.itemTypeName == CollectableItem.ItemType.WOOD);
        if (amountOfWood >= 2)
        {
            for (int i = 0; i < 2; i++)
            {
                CollectableItem neededWood = collectableItems.Find(item => item.itemTypeName == CollectableItem.ItemType.WOOD); // finde das erste Wood-Item aus der Liste
                collectableItems.Remove(neededWood); // lösche das Wood-Item aus der Liste
            }
            // erzeuge ein WoodenStake
            consumableItems.Add(new ConsumableItem("WoodenStake"));
            //Debug.Log(this.GetComponentInParent<Monster_Behaviour>().name + " crafted WoodenStake.");
            this.GetComponentInParent<Monster_Behaviour>().updateMonsterLog(this.GetComponentInParent<Monster_Behaviour>().name + " crafted WoodenStake.");
        }
        else
        {

            Debug.Log(this.GetComponentInParent<Monster_Behaviour>().name + ": Error, there are no 2 pieces of Wood. Cannot create WoodenStake.");
        }
    }
}
