using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Goal_Manager : MonoBehaviour {

    public ItemManager itemManager;
    public Monster_Behaviour monster;
    public List<Goal> goals;
    public List<Goal> activeGoals;
    private int amountOfMeat;
    private int amountOfWood;
    private int amountOfStr;
    private int amountOfDef;
    private int amountOfSmallHeal;
    private int amountOfBigHeal;
    private Ability chosenBattleAbility;

    public Ability ChosenBattleAbility { get { return chosenBattleAbility; } }


    // Use this for initialization
    void Start () {

        itemManager = this.GetComponentInParent<ItemManager>();
        monster = this.GetComponentInParent<Monster_Behaviour>();
        goals = new List<Goal>();
        activeGoals = new List<Goal>();
        createGoals();
        setAllPossibleGoalsActive(false); // zu Beginn ist das Monster nicht im Kampf, daher wird die Flee Action nicht berücksichtigt
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void battleThinkCycle(bool isMonsterLastInQueue)
    {
        setAllPossibleGoalsActive(isMonsterLastInQueue);
        calculateBattlePriority();
        // entscheide, welche Aktion ausgeführt werden soll anhand der Priorität
        findBestBattleAbility();
        // lösche die Änderungen der Goals
        clearAllActiveGoals();

    }

    void findBestBattleAbility()
    {
        // Sortiere die Goals nach absteigender Priorität
        activeGoals.Sort((a, b) => b.BattlePriority.CompareTo(a.BattlePriority));
        for (int i = 0; i < activeGoals.Count; i++)
        {
            monster.updateMonsterLog("ActiveGoal " + activeGoals[i].CurrGoalType.ToString() + " its battlePrio is: " + activeGoals[i].BattlePriority);
           // Debug.Log("ActiveGoal " + activeGoals[i].CurrGoalType.ToString() + " its Prio is: " + activeGoals[i].BattlePriority);
        }

        // das Goal mit der höchsten Priorität wird die ausgwählte Ability
        Goal.GoalType type = activeGoals[0].CurrGoalType;

        switch (type)
        {
            case Goal.GoalType.SmallHeal:
                // das Using Item wird von der consumableItems liste genommen
                itemManager.UsingItem = itemManager.consumableItems.Find(item => item.ItemTypeName == Item.ItemType.SMALL_HEAL);
                chosenBattleAbility = new Ability(Ability.AbilityTypes.USE_ITEM, monster);
                break;

            case Goal.GoalType.BigHeal:
                // das Using Item wird von der consumableItems liste genommen
                itemManager.UsingItem = itemManager.consumableItems.Find(item => item.ItemTypeName == Item.ItemType.BIG_HEAL);
                chosenBattleAbility = new Ability(Ability.AbilityTypes.USE_ITEM, monster);
                break;

            case Goal.GoalType.StrengthBonus:
                // das Using Item wird von der consumableItems liste genommen
                itemManager.UsingItem = itemManager.consumableItems.Find(item => item.ItemTypeName == Item.ItemType.STR_BONUS);
                chosenBattleAbility = new Ability(Ability.AbilityTypes.USE_ITEM, monster);
                break;

            case Goal.GoalType.DefendBonus:
                // das Using Item wird von der consumableItems liste genommen
                itemManager.UsingItem = itemManager.consumableItems.Find(item => item.ItemTypeName == Item.ItemType.DEF_BONUS);
                chosenBattleAbility = new Ability(Ability.AbilityTypes.USE_ITEM, monster);
                break;

            case Goal.GoalType.Attack:
                chosenBattleAbility = new Ability(Ability.AbilityTypes.ENEMYATTACK, monster);
                break;
            case Goal.GoalType.Flee:
                chosenBattleAbility = new Ability(Ability.AbilityTypes.DEQUEUE, monster);
                break;
        }


    }

    void clearAllActiveGoals()
    {

        activeGoals.Clear();
        for(int i = 0; i < goals.Count; i++)
        {
            goals[i].IsSelected = false;
            goals[i].BattlePriority = 0;
            goals[i].CurrentPriority = 0;
            goals[i].ResourcesCount = 0;
        }
    }

    void createGoals()
    { 
        // erzeuge die Crafting Goals
        // erzeuge ein CraftChickenWing Goal
        goals.Add(new Goal(Goal.GoalType.SmallHeal, Item.ItemType.MEAT, 1)); //Goal 0
        // erzeuge ein CraftHumanMeat Goal
        goals.Add(new Goal(Goal.GoalType.BigHeal, Item.ItemType.MEAT, 3)); // Goal 1

        //erzeuge ein CraftWoodenStake und Shield Goal  
        goals.Add(new Goal(Goal.GoalType.DefendBonus, Item.ItemType.WOOD, 2));  // Goal 2
        goals.Add(new Goal(Goal.GoalType.StrengthBonus, Item.ItemType.WOOD, 2));  // Goal 3

        // erzeuge die UsingItem Goals ; deren itemCount ist immer 1 (da im Kampf nur ein einzelnes Item verwendet werden darf)

        goals.Add(new Goal(Goal.GoalType.SmallHeal, Item.ItemType.SMALL_HEAL, 1)); //Goal 4
        goals.Add(new Goal(Goal.GoalType.BigHeal, Item.ItemType.BIG_HEAL, 1)); //Goal 5

        goals.Add(new Goal(Goal.GoalType.DefendBonus, Item.ItemType.DEF_BONUS, 1)); // Goal 6
        goals.Add(new Goal(Goal.GoalType.StrengthBonus, Item.ItemType.STR_BONUS, 1)); // Goal 7

        // restlichen Combat Goals

        goals.Add(new Goal(Goal.GoalType.Attack, Item.ItemType.DEFAULT, 0)); // Goal 8
        goals.Add(new Goal(Goal.GoalType.Flee, Item.ItemType.DEFAULT, 0));  // Goal 9
    }

    void setAllPossibleGoalsActive(bool isEscapeNotPossible)
    {
        // prüfe, ob die Anzahl der CollectableItems (bzw. ConsumableItems) mit der neededItemCount übereinstimmt; falls nicht wird das Goal inaktiv
        // zudem , prüfe ob das Monster inCombat ist; somit fallen alle Crafting Goals heraus bzw. falls nicht, fallen alle Battle Goals heraus

        if (!monster.inCombat)  // Monster ist nicht im Kampf
        {
            // alle Using Item Goals werden inaktiv gesetzt
            for (int i = 4; i < goals.Count; i++)
            {

                goals[i].IsActive = false;
            }

            amountOfWood = itemManager.collectableItems.Count(item => item.itemTypeName == Item.ItemType.WOOD);
            amountOfMeat = itemManager.collectableItems.Count(item => item.itemTypeName == Item.ItemType.MEAT);

            if (amountOfMeat >= 1)
            {
                goals[0].IsActive = true;

                if (amountOfMeat >= 3)
                {
                    goals[1].IsActive = true;
                }
                else
                {
                    goals[1].IsActive = false;
                }
            }
            else
            {
                goals[0].IsActive = false;
                goals[1].IsActive = false;

            }
            if (amountOfWood >= 2)
            {
                goals[2].IsActive = true;
                goals[3].IsActive = true;
            }
            else
            {
                goals[2].IsActive = false;
                goals[3].IsActive = false;

            }

        }
        else // Monster ist im Kampf
        {
            // alle Crafting Goals werden inaktiv gesetzt
            for (int i = 0; i < 4; i++)
            {

                goals[i].IsActive = false;
            }

            // ermittle die Anzahl der Items jedes Types
            amountOfSmallHeal = itemManager.consumableItems.Count(item => item.ItemTypeName == Item.ItemType.SMALL_HEAL);
            amountOfBigHeal = itemManager.consumableItems.Count(item => item.ItemTypeName == Item.ItemType.BIG_HEAL);
            amountOfDef = itemManager.consumableItems.Count(item => item.ItemTypeName == Item.ItemType.DEF_BONUS);
            amountOfStr = itemManager.consumableItems.Count(item => item.ItemTypeName == Item.ItemType.STR_BONUS);

            //Debug.Log("Item Amounts: SH " + amountOfSmallHeal+ ", BH " + amountOfBigHeal + " , Def " + amountOfDef + " , Str " + amountOfStr);
            monster.updateMonsterLog("Item Amounts: SH " + amountOfSmallHeal + ", BH " + amountOfBigHeal + " , Def " + amountOfDef + " , Str " + amountOfStr);

            if (amountOfSmallHeal > 0)
            {
                goals[4].IsActive = true;
            }
            else
            {
                goals[4].IsActive = false;
            }
            if (amountOfBigHeal > 0)
            {
                goals[5].IsActive = true;
            }
            else
            {
                goals[5].IsActive = false;
            }
            if (amountOfDef > 0)
            {
                goals[6].IsActive = true;
            }
            else
            {
                goals[6].IsActive = false;
            }
            if (amountOfStr > 0)
            {
                goals[7].IsActive = true;
            }
            else
            {
                goals[7].IsActive = false;
            }

            for (int i = 8; i < goals.Count; i++)
            {
                goals[i].IsActive = true;
            }

            if (isEscapeNotPossible)  // Flee ist nicht möglich
            {
                goals[9].IsActive = false;
            }
        }

        // initialisiere activeGoals
        activeGoals = goals.FindAll(goal => goal.IsActive == true);
        //Debug.Log("ActiveGoals Count: " + activeGoals.Count);
    }

    void calculateBattlePriority()
    {
        for (int i = 0; i < activeGoals.Count; i++)
        {
            if (activeGoals[i].CurrGoalType == Goal.GoalType.SmallHeal)
            {
                activeGoals[i].BattlePriority = ( ( -1.0f / 24.0f) * (float) ( monster.getMonsterData().getAttributeValueAtIndex(0) * (monster.getMonsterData().getAttributeValueAtIndex(0) - monster.MaxStamina)) ) - 50.0f; // Parabelgleichung mit 2 Nullstellen und ein Punkt
            }
            else if (activeGoals[i].CurrGoalType == Goal.GoalType.BigHeal)
            {
                activeGoals[i].BattlePriority = (monster.MaxStamina - monster.getMonsterData().getAttributeValueAtIndex(0))  -20 + Random.Range(0, 31) ;

            }
            else if (activeGoals[i].CurrGoalType == Goal.GoalType.DefendBonus)
            {
                activeGoals[i].BattlePriority = (GameMachine.gameMachine.player.Strength - monster.getMonsterData().getAttributeValueAtIndex(2)) * 3;

            }
            else if (activeGoals[i].CurrGoalType == Goal.GoalType.StrengthBonus)
            {
                activeGoals[i].BattlePriority = (GameMachine.gameMachine.player.Endurance - monster.getMonsterData().getAttributeValueAtIndex(1)) * 3;
            }
            else if (activeGoals[i].CurrGoalType == Goal.GoalType.Attack)
            {
                activeGoals[i].BattlePriority = (monster.getMonsterData().getAttributeValueAtIndex(6) + monster.getMonsterData().getAttributeValueAtIndex(4) - monster.getMonsterData().getAttributeValueAtIndex(7)) / 2 + Random.Range(0,51);
            }

            else if(activeGoals[i].CurrGoalType == Goal.GoalType.Flee)
            {

                activeGoals[i].BattlePriority = monster.getMonsterData().getAttributeValueAtIndex(5) / 2 + Random.Range(0, 51); // Anxiety-Value halbieren und ein Random-Wert dazu addieren (=> Prioritäts-Wert von maxmimal 100)
            }

        }

    }

    void calculateCurrentPriority()
    {
         activeGoals = goals.FindAll(goal => goal.IsActive == true); // verwende nur die aktiven Goals (= alle möglichen Goals)

        for (int i = 0; i < activeGoals.Count; i++)
        {
            // berechne die Priority, falls alle vorhandenen, gecrafteten Resourcen im Kampf verwendet wurden um das nächst notwendige Craft Item herauszufinden
             
            ConsumableItem consumableItem = new ConsumableItem(activeGoals[i].CurrGoalType); // um welches Item handelt es sich
            // wie viele Item dieses Types hat das Monster bereits + wie viele Items könnte es dazu craften
            int amountOfItems = itemManager.consumableItems.Count(item => item.ItemTypeName == consumableItem.ItemTypeName) + activeGoals[i].ExecuteCount;
            monster.updateMonsterLog("Item: " + activeGoals[i].CurrGoalType + "  " + activeGoals[i].ExecuteCount + " times crafted and monster has " + amountOfItems + " copies in total.");
           // Debug.Log("Item: " + activeGoals[i].CurrGoalType + "  " + activeGoals[i].ExecuteCount + " times gecrafted and monster has "+ amountOfItems + " copies in total.");
            if (activeGoals[i].CurrGoalType == Goal.GoalType.SmallHeal)
            {
                int newEnemyStamina = monster.getMonsterData().getAttributeValueAtIndex(0);
                newEnemyStamina += consumableItem.Value * amountOfItems; // addiere den Value Wert des Items * die Anzahl der Items
                if(newEnemyStamina > monster.MaxStamina) // prüfe, ob die Max Stamina überschritten sind
                {
                    newEnemyStamina = monster.MaxStamina;
                }

                activeGoals[i].CurrentPriority = ((-1.0f / 24.0f) * (float)( newEnemyStamina * (newEnemyStamina - monster.MaxStamina))) - 50.0f; // Parabelgleichung mit 2 Nullstellen und ein Punkt
            }
            else if (activeGoals[i].CurrGoalType == Goal.GoalType.BigHeal)
            {
                int newEnemyStamina = monster.getMonsterData().getAttributeValueAtIndex(0);
                newEnemyStamina += consumableItem.Value * amountOfItems; // addiere den Value Wert des Items
                if (newEnemyStamina > monster.MaxStamina) // prüfe, ob die Max Stamina überschritten sind
                {
                    newEnemyStamina = monster.MaxStamina;
                }
                activeGoals[i].CurrentPriority = (monster.MaxStamina - newEnemyStamina) - 20 + Random.Range(0, 31);


            }
            else if (activeGoals[i].CurrGoalType == Goal.GoalType.DefendBonus)
            {
                int newEnemyDefense = monster.getMonsterData().getAttributeValueAtIndex(2);
                newEnemyDefense += consumableItem.Value * amountOfItems; // addiere den Value Wert des Items
                activeGoals[i].CurrentPriority = (GameMachine.gameMachine.player.Strength - newEnemyDefense) * 3;

            }
            else if (activeGoals[i].CurrGoalType == Goal.GoalType.StrengthBonus)
            {
                int newEnemyStrength = monster.getMonsterData().getAttributeValueAtIndex(1);
                newEnemyStrength += consumableItem.Value * amountOfItems; // addiere den Value Wert des Items
                activeGoals[i].CurrentPriority = (GameMachine.gameMachine.player.Endurance - newEnemyStrength) * 3;
            }

        }

    }


    void distributeResources()
    {
      
        //sortiere die activeGoals Liste absteigend nach der CurrentPriority
        activeGoals.Sort((a, b) => b.CurrentPriority.CompareTo(a.CurrentPriority));
        for (int i = 0; i < activeGoals.Count; i++)
           {
              monster.updateMonsterLog("ActiveGoal " + activeGoals[i].CurrGoalType.ToString() + " its CurrPrio is: " + activeGoals[i].CurrentPriority);
              // Debug.Log("ActiveGoal " + activeGoals[i].CurrGoalType.ToString() + " its CurrPrio is: " + activeGoals[i].CurrentPriority);
           }
        // bool, die speichert ob das Goal mit der höchsten Priorität des jeweiligen Types bereits Resourcen erhalten hat; 
        bool isFirstMeatGoalDone = false;
        bool isFirstWoodGoalDone = false;

        // zuerst bekommen die Goals die Resourcen zugeteilt, in der Reihenfolge nach der Priorität
        for (int i = 0; i < activeGoals.Count; i++)
        {
            // prüfe, ob die CurrentPriority größer 0 ist; falls nicht werden keine Resourcen zugeteilt und das Goal wird nicht selektiert und ausgeführt und inaktiv gesetzt
            if (activeGoals[i].CurrentPriority > 0) {
                // bestimme den ItemType und die benötigte Anzahl der Items
                Item.ItemType itemType = activeGoals[i].NeededItemType;
                int count = activeGoals[i].NeededItemCount;

                // falls ein Goal die nötigen Ressourcen nicht zugeteilt bekommt, wird es nicht selektiert
                // falls ein Goal die nötigen Ressourcen zugeteilt bekommen hat, wird es selektiert
                switch (itemType)
                {
                    case Item.ItemType.MEAT:
                        // prüfe, ob es noch so viele Flesh Items gibt, wie gebraucht werden
                        if (amountOfMeat >= count && !isFirstMeatGoalDone)
                        {
                            for (int j = 0; j < count; j++)
                            {
                                // teile die Resource dem Goal hinzu
                                activeGoals[i].ResourcesCount++;
                                // ein Meat Item ist nun weniger vorhanden
                                amountOfMeat--;
                            }
                            activeGoals[i].IsSelected = true;
                            monster.updateMonsterLog("ActiveGoal: " + activeGoals[i].CurrGoalType + " is selected.");
                            //Debug.Log("ActiveGoal: " + activeGoals[i].CurrGoalType + " is selected.");
                            isFirstMeatGoalDone = true;
                        }
                       else
                        {
                            activeGoals[i].IsSelected = false;
                            monster.updateMonsterLog("ActiveGoal: " + activeGoals[i].CurrGoalType + " is not selected anymore.");
                            // Debug.Log("ActiveGoal: " + activeGoals[i].CurrGoalType + " is not selected anymore.");
                        }
                        break;

                    case Item.ItemType.WOOD:
                        // prüfe, ob es noch so viele Wood Items gibt, wie gebraucht werden
                        if (amountOfWood >= count && !isFirstWoodGoalDone)
                        {
                            for (int j = 0; j < count; j++)
                            {
                                // teile die Resource dem Goal hinzu
                                activeGoals[i].ResourcesCount++;
                                // ein Wood Item ist nun weniger vorhanden
                                amountOfWood--;

                            }
                            activeGoals[i].IsSelected = true;
                            monster.updateMonsterLog("ActiveGoal: " + activeGoals[i].CurrGoalType + " is selected.");
                            //Debug.Log("ActiveGoal: " + activeGoals[i].CurrGoalType + " is selected.");
                            isFirstWoodGoalDone = true;
                        }
                        else
                        {
                            activeGoals[i].IsSelected = false;
                            monster.updateMonsterLog("ActiveGoal: " + activeGoals[i].CurrGoalType + " is not selected anymore.");
                            //Debug.Log("ActiveGoal: " + activeGoals[i].CurrGoalType + " is not selected anymore.");

                        }
                        break;

                }
            }
            else
            {
                monster.updateMonsterLog("ActiveGoal " + activeGoals[i].CurrGoalType.ToString() + " BasePrio is < 0. It is not selected anymore");
               // Debug.Log("ActiveGoal " + activeGoals[i].CurrGoalType.ToString() + " BasePrio is < 0. It is not selected anymore");
                activeGoals[i].IsSelected = false;

            }
        }

        
    }


    public void craftingThinkCycle()
    {
        monster.updateMonsterLog("Items: Meat: " + amountOfMeat + " , Wood: " + amountOfWood);
        //Debug.Log("Items: F: " + amountOfMeat + " , W: " + amountOfWood);
        setAllPossibleGoalsActive(false);
        calculateCurrentPriority();
        distributeResources();
        int selectedGoalCount = activeGoals.Count(item => item.IsSelected);
        monster.updateMonsterLog("Items: Meat: " + amountOfMeat + " , Wood: " + amountOfWood + " , " + selectedGoalCount);
       // Debug.Log("Items: F: " + amountOfMeat + " , W: " + amountOfWood + " , " + selectedGoalCount);
        // falls noch Resourcen übrig sind und es noch aktive Goals gibt, berechne die currentPriority aller selektierten Goals 
         while ((amountOfMeat > 0 || amountOfWood > 0) && selectedGoalCount > 0)
          {
              calculateCurrentPriority();
              distributeResources();
            monster.updateMonsterLog("Items: Meat: " + amountOfMeat + " , Wood: " + amountOfWood + " , " + selectedGoalCount);
            // Debug.Log("Items: F: " + amountOfMeat + " , W: " + amountOfWood + " , " + selectedGoalCount );
            selectedGoalCount = activeGoals.Count(item => item.IsSelected);

        }

        // craft all selected Goals
        craftAllSelectedGoals();

        monster.updateMonsterLog("Crafting Think Cycle ended. ActiveGoals cleared.");
        //Debug.Log("Crafting Think Cycle ended. ActiveGoals cleared.");
        clearAllActiveGoals();
    }

    void craftAllSelectedGoals()
    {
        activeGoals = activeGoals.FindAll(goal => goal.ExecuteCount > 0);
        for(int i = 0; i < activeGoals.Count; i++)
        {
            Goal.GoalType type = activeGoals[i].CurrGoalType;  // um welches Goal handelt es sich 
            float count = activeGoals[i].ExecuteCount; //wie viele Male soll es das Item craften
            switch (type)
            {
                case Goal.GoalType.SmallHeal:
                    for (int j = 0; j < count; j++)
                    {
                        itemManager.craftChickenWing();
                    }
                    break;

                case Goal.GoalType.BigHeal:
                    for (int j = 0; j < count; j++)
                    {

                        itemManager.craftHumanMeat();
                    }
                    break;

                case Goal.GoalType.StrengthBonus:
                    for (int j = 0; j < count; j++)
                    {

                        itemManager.crafWoodenStake();
                    }
                    break;

                case Goal.GoalType.DefendBonus:
                    for (int j = 0; j < count; j++)
                    {

                        itemManager.crafWoodenShield();
                    }
                    break;
            }

            activeGoals[i].IsSelected = false;
        }

    }
}
