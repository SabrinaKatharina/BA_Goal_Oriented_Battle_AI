using UnityEngine;
using System.Collections;

public class Weapon{

    private string weaponName;
    private int weaponLevel;
    private int weaponExp;
    private int nextLevelUpCost;
    private int attackPower;
    private float attackSpeed;
    public enum ElementTypes { Fire, Ice, Water};
    private ElementTypes elementType;

    // getter und setter

    public string WeaponName {

        get { return weaponName; }
        set { weaponName = value; }
    }

    public int WeaponLevel
    {

        get { return weaponLevel; }
        set { weaponLevel = value; }
    }

    public int WeaponExp
    {

        get { return weaponExp; }
        set { weaponExp = value; }
    }

    public int NextLevelUpCost
    {

        get { return nextLevelUpCost; }
        set { nextLevelUpCost = value; }
    }

    public int AttackPower
    {

        get { return attackPower; }
        set { attackPower = value; }
    }

    public float AttackSpeed
    {

        get { return attackSpeed; }
        set { attackSpeed = value; }
    }

    public ElementTypes ElementType {

        get { return elementType; }
        set { elementType = value; }
    }

    // upgrade Waffe

    public void upgrade() {

        weaponLevel++;
        nextLevelUpCost *= 2;
        attackPower += 8;

    }

}
