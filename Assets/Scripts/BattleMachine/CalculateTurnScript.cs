using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CalculateTurnScript{

    private Ability currentAbility;
    private Monster_Behaviour enemy;
    private PlayerInformation player;
    private bool hasEscaped;

    public CalculateTurnScript(Ability usedAbility, Monster_Behaviour enemy) {

        currentAbility = usedAbility;
        this.enemy = enemy;
        player = GameObject.Find("Player").GetComponent<PlayerInformation>();

        // entscheide anhand des AbilityTypes, was ausgeführt wird:
        Ability.AbilityTypes type = usedAbility.TypeName;
        switch (type) {

            case Ability.AbilityTypes.ATTACK: calculateEnergyCost(); calculateEnemyDamage(); break;
            case Ability.AbilityTypes.DEFEND: calculateEnergyCost();  break;
            case Ability.AbilityTypes.FLEE: calculateEnergyCost();  executeFleeAction();  break;
            case Ability.AbilityTypes.HEAL: calculateEnergyCost(); calculatePlayerHeal();  break;
            case Ability.AbilityTypes.ENEMYATTACK: calculatePlayerDamage(); break;
            case Ability.AbilityTypes.USE_ITEM: useItem(); break;
            case Ability.AbilityTypes.DEQUEUE: dequeueEnemy(); break;
            default: break;

        }

    }

    void calculateEnemyDamage() {

        int damage = currentAbility.Value;

        // überprüfe, ob die Waffe einen Vorteil gegenüber des Elements des Monsters hat
        if (hasElementAdvantage())
        {
            // bonus damage ist von dem Intellect Wert des Players abhängig
            damage += Mathf.FloorToInt(player.Intellect / 4);
            Debug.Log("BonusDamage: " + Mathf.FloorToInt(player.Intellect / 4));

            // rechne eine Wahrscheinlichkeit aus, ob die Energy um einen Betrag aufgefüllt wird (bonus)

            float probability = Random.Range(0.0f, 100.0f);

            if (probability >= 33.0f) {
                Debug.Log("Energy recharge: +5");
                player.Energy += 5;
                if(player.Energy > player.MaxEnergyValue)
                {

                    player.Energy = player.MaxEnergyValue;
                }
            }
           
        }


        // Schadensberechnung: monster.Stamina -= damage * (1- 0.5 * (monster.defense/100));

        int enemyStamina = enemy.getMonsterData().getAttributeValueAtIndex(0);
        int enemyEndurance = enemy.getMonsterData().getAttributeValueAtIndex(2);

        enemyStamina -= Mathf.FloorToInt(damage * (float)(1 - 0.5f * (enemyEndurance / 100.0f)));
        Debug.Log("CalculatedEnemyDamage: " + Mathf.FloorToInt(damage * (float)(1 - 0.5f * (enemyEndurance / 100.0f))));
        enemy.updateMonsterLog("CalculatedEnemyDamage: " + Mathf.FloorToInt(damage * (float)(1 - 0.5f * (enemyEndurance / 100.0f))));
        enemy.getMonsterData().setAttributeAtIndex(0, enemyStamina);


    }

    void calculatePlayerDamage()
    {

        int damage = currentAbility.Value;

        // Schadensberechnung: player.Stamina -= damage * (1- 0.5 * (player.defense/100));

        player.Stamina -= Mathf.FloorToInt(damage * (float)(1 - 0.5 * (player.Endurance / 100.0f)));
        //Debug.Log("CalculatedPlayerDamage: " + Mathf.FloorToInt(damage * (float)(1 - 0.5 * (player.Endurance / 100.0f))) + " PlayerEndurance " + player.Endurance);
        enemy.updateMonsterLog("CalculatedPlayerDamage: " + Mathf.FloorToInt(damage * (float)(1 - 0.5 * (player.Endurance / 100.0f))) + " PlayerEndurance " + player.Endurance);
    }

    bool hasElementAdvantage() {

        bool temp = false;
        
        if (player.UsedWeapon.ElementType == Weapon.ElementTypes.Fire && enemy.getMonsterData().getElementType() == Monster_Data.ElementTypes.Ice)
        {
            temp = true;
        }
        if (player.UsedWeapon.ElementType == Weapon.ElementTypes.Water && enemy.getMonsterData().getElementType() == Monster_Data.ElementTypes.Fire)
        {
            temp = true;
        }
        if (player.UsedWeapon.ElementType == Weapon.ElementTypes.Ice && enemy.getMonsterData().getElementType() == Monster_Data.ElementTypes.Water)
        {
            temp = true;
        }

        return temp;
    }

    public int getDefenseBonus(int currentEndurance) {

        // DefenseBonus ist 25% des aktuellen Endurance wert;
        return (int) (currentEndurance / 4);
    }

    void calculatePlayerHeal() {

        int healAmount = currentAbility.Value;

        player.Stamina += healAmount;
        //prüfen, ob es den maxStaminaVal übertrifft;
        if (player.Stamina >= player.MaxStaminaValue) {

            player.Stamina = player.MaxStaminaValue;
        }
    }

    void useItem() {

        if (enemy.GetComponent<ItemManager>().UsingItem != null)
        {
            ConsumableItem.ItemType type = enemy.GetComponent<ItemManager>().UsingItem.ItemTypeName;  // finde den ItemType heraus
            int value = currentAbility.Value;                                                         // finde den Value heraus

            // führe die Aktion je nach ItemType aus
            switch (type)
            {
                case ConsumableItem.ItemType.SMALL_HEAL:   // Stamina wird geheilt

                   int enemyStamina = enemy.getMonsterData().getAttributeValueAtIndex(0) + value;
                    if(enemyStamina > enemy.MaxStamina)
                    {
                        enemyStamina = enemy.MaxStamina;
                    }
                   enemy.getMonsterData().setAttributeAtIndex(0, enemyStamina);
                   break;

                case ConsumableItem.ItemType.BIG_HEAL:   // Stamina wird geheilt

                    int enemysStamina = enemy.getMonsterData().getAttributeValueAtIndex(0) + value;
                    if (enemysStamina > enemy.MaxStamina)
                    {
                        enemysStamina = enemy.MaxStamina;
                    }
                    enemy.getMonsterData().setAttributeAtIndex(0, enemysStamina);
                    break;

                case ConsumableItem.ItemType.STR_BONUS:  // ATK /Strength Wert wird erhöht

                    int enemyStrength = enemy.getMonsterData().getAttributeValueAtIndex(1) + value;
                    enemy.getMonsterData().setAttributeAtIndex(1, enemyStrength);
                    break;

                case ConsumableItem.ItemType.DEF_BONUS:  // DEF / Endurance Wert wird erhöht
                    int enemyDefense = enemy.getMonsterData().getAttributeValueAtIndex(2) + value;
                    enemy.getMonsterData().setAttributeAtIndex(2, enemyDefense);
                    break;

            }

            // zerstöre das Item
            enemy.GetComponent<ItemManager>().IsItemUsed = true;
        }
        else
        {
            Debug.Log("Trying to use item, but usingItem is null");
        }

    }

    void dequeueEnemy() {

        //Debug.Log("Dequeue enemy");
        enemy.updateMonsterLog("Dequeue enemy");
    }

    void calculateEnergyCost() {

        int cost = currentAbility.EnergyCost;

        player.Energy -= cost;

        //prüfe, ob der aktulle Wert unter 0 ist
        if (player.Energy < 0) {
            player.Energy = 0;
        }

    }

    void executeFleeAction() {

       
            int random = Random.Range(0, 50);
            if (random % 2 == 0)  // fliehen ist erfolgreich
            {

                Debug.Log("Escape was successful");
                hasEscaped = true;
             
            }

            else { // fliehen nicht erfolgreich

                Debug.Log("Escape failed.");
                hasEscaped = false;
               
            }
        
    }

    public bool hasPlayerEscaped() {
        return hasEscaped;
    }

    public bool isEnemyDead()
    {

        bool temp = false;
        if (enemy.getMonsterData().getAttributeValueAtIndex(0) <= 0)
        {
            temp = true;
        }
        return temp;

    }
}
