using UnityEngine;
using System.Collections;

public class BowWeapon : Weapon {

    public BowWeapon() {

        WeaponName = "Basic Fire Bow";
        WeaponLevel = 1;
        NextLevelUpCost = 100;
        AttackPower = 5;
        AttackSpeed = 2.5f;
        ElementType = ElementTypes.Fire;

    }
}
