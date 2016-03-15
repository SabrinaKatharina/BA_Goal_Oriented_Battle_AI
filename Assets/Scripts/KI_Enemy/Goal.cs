using UnityEngine;
using System.Collections;

// repräsentiert jede Action, welche die AI im Spiel verwenden kann (zB Use HumanMeat Item, Craft WoodenShield ...)
public class Goal {

    public enum GoalType { SmallHeal, BigHeal, DefendBonus, StrengthBonus, Attack, Flee }
    GoalType goalType;  // welche Aktion repräsentiert das Goal

    bool isActive; // jedes Goal hat einen Status aktiv oder inaktiv, solange das Goal inaktiv ist, wird es nicht berücksichtigt
    bool isSelected; // selected Goals werden ausgeführt
    float battlePriority; // priorität im aktuellen Zustand der Situation, allerdings ohne Berücksichtigung der Resourcen
    float currentPriority; // priorität mit Berücksichtigung der Resourcen
    Item.ItemType neededItemType;   // benötigter ItemType (falls man verschiedene benötigt müsste man ein Array mit allen ItemTypes speichern
    int neededItemCount;            // Anzahl wie viele Items benötigt werden 
    int resourcesCount;            // Anzahl, wie viele Ressourcen das Goal zugeteilt bekommen hat
    public int ExecuteCount { get {
            if (resourcesCount > 0) return (resourcesCount / neededItemCount);
            else return 0;
        } }


    public bool IsActive { get { return isActive; } set {isActive = value; } }
    public bool IsSelected { get { return isSelected; } set { isSelected = value; } }

    public float BattlePriority { get { return battlePriority; } set { battlePriority = value; } }
    public float CurrentPriority { get { return currentPriority; } set { currentPriority = value; } }

    public Item.ItemType NeededItemType { get { return neededItemType; } } 

    public int NeededItemCount { get { return neededItemCount; } }

    public int ResourcesCount { get { return resourcesCount; } set { resourcesCount = value; } }

    public GoalType CurrGoalType { get { return goalType; } }

    public Goal(GoalType goalType, Item.ItemType itemType, int itemCount)
    {
        this.goalType = goalType;
        isActive = true; // bei Erzeugung eines Goals ist es aktiv
        neededItemType = itemType;
        neededItemCount = itemCount;
    }

}
