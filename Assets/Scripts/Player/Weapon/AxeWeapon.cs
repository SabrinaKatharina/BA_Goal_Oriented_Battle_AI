using UnityEngine;
using System.Collections;

public class AxeWeapon : Weapon {

    public AxeWeapon() {

        WeaponName = "Basic Water Axe";
        WeaponLevel = 1;
        NextLevelUpCost = 100;
        AttackPower = 15;
        AttackSpeed = 7.5f;
        ElementType = ElementTypes.Water;

    }
}
