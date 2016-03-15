using UnityEngine;
using System.Collections;

public class SwordWeapon : Weapon {

    public SwordWeapon() {

        WeaponName = "Basic Ice Sword";
        WeaponLevel = 1;
        NextLevelUpCost = 100;
        AttackPower = 10;
        AttackSpeed = 5.0f;
        ElementType = ElementTypes.Ice;
    }
}
