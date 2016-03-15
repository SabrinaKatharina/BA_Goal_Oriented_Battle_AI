using UnityEngine;
using System.Collections;

public class Ability{

    // die möglichen Abilities des Players: Attack, Heal, Defend, Flee
    // die möglichen Abilities des Enemys: EnemyAttack, Dequeue, UseItem
    public enum AbilityTypes { ATTACK, HEAL, DEFEND, FLEE, ENEMYATTACK, DEQUEUE, USE_ITEM};
    private AbilityTypes typeName;
    private int energyCost;
    private int value;  // für damage, heal oder defense power up

    public Ability(AbilityTypes type)
    {
        typeName = type;
        PlayerInformation player = GameObject.Find("Player").GetComponent<PlayerInformation>();
        // entscheide die EnergyCost und den Value der Ability
        switch (type)
        {
            case AbilityTypes.ATTACK:
                energyCost = 10;
                value = player.UsedWeapon.AttackPower + player.Strength;
                break;
            case AbilityTypes.DEFEND:
                energyCost = 8;
                value = player.Endurance;
                break;
            case AbilityTypes.FLEE:
                energyCost = 5;
                value = 0; // Aus dem Kampf fliehen
                break;
            case AbilityTypes.HEAL:
                energyCost = 15;
                value = player.Intellect;
                break;

            default: Debug.Log("Error: this AbilityType " + type.ToString() + " is not a player ability"); break;
        }
    }

    public Ability(AbilityTypes type, Monster_Behaviour monster)
    {
        typeName = type;
        energyCost = 0; // Energy wird bei Monstern nicht beachtet
       // entscheide den Value der Ability
        switch (type)
        {
            case AbilityTypes.ENEMYATTACK:
                value = monster.getMonsterData().getAttributeValueAtIndex(1); // ATK des Monsters
                break;
            case AbilityTypes.USE_ITEM:
                if (monster.GetComponent<ItemManager>().UsingItem != null)
                     value = monster.GetComponent<ItemManager>().UsingItem.Value;  // Wert des Items
                else
                {
                    Debug.Log("Try to use item, but the UsingItem is null");
                }
                break;
            case AbilityTypes.DEQUEUE:
                value = 0; // Monster stellt sich hinten an die Enemy-queue an
                break;


            default: Debug.Log("Error: this AbilityType " + type.ToString() + " is not a monster ability"); break;
        }

    }

    // getter

    public AbilityTypes TypeName
    {

        get { return typeName; }
    }

    public int EnergyCost {

        get { return energyCost; }
    }

    public int Value {

        get { return value; }
        set { this.value = value;  }
    }
}
