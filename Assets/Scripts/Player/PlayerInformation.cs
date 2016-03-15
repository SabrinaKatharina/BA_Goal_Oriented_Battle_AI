using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerInformation : MonoBehaviour {

    // todo : Variablen müssen zum serializieren public sein (Player Pref Variante)
    private bool isInstantiated = false;   // damit nur ein Player gibt, der nicht überschrieben wird
    [SerializeField]
         private string playerName;

    private bool isMale;      // true oder false

    [SerializeField]
         private int playerLevel; // Man startet von Level 1
    [SerializeField]
         private int playerExp;   // Experience Points
    [SerializeField]
         private int nextLevelUpAmount;   // Wie viel Exp der Player zum nächsten Levelup braucht
    [SerializeField]
         private int moneyAmount;         // Wie viel Geld der Player besitzt

    private int maxStaminaValue;
    private int maxEnergyValue;
    [SerializeField]
         private int stamina;    // Anzahl Health Points, bis der Player besiegt wird
    [SerializeField]
         private int energy;     // Anzahl Energy Points, die der Player für Abilities einsetzen kann
    [SerializeField]
         private int endurance;   // physischer Verteidigungswert
    [SerializeField]
         private int intellect;   // Magieangriffswert
    [SerializeField]
        private int strength;    // physischer Angriffswert

    private Weapon usedWeapon;   // angelegte Waffe
    public List<Weapon> ownedWeapons = new List<Weapon>();  //Liste der besitzende Waffen ; können gekauft werden oder verkauft


    void Awake()
    {
        if (!isInstantiated)
        {
            DontDestroyOnLoad(gameObject); // das gameObject wird bei Szenen-Wechsel nicht zerstört
            isInstantiated = true;
        }
        else //der Player hat bereits eine Instanz
        {

            Destroy(gameObject); // zerstöre das erzeugte Player-gameObject
        }

    }

    // Konstruktor

    public PlayerInformation() {

        playerName = "Your Name";
        playerLevel = 1;
        playerExp = 0;
        nextLevelUpAmount = 10;
        maxStaminaValue = 100;
        maxEnergyValue = 150;
        stamina = maxStaminaValue;
        energy = maxEnergyValue;
        endurance = 20;
        intellect = 20;
        strength = 20;
        moneyAmount = 0;

        

    }

    /* Getter und Setter */

    public string PlayerName {

        get { return playerName; }
        set { playerName = value; }
    }

    public bool IsMale {

        get{ return isMale; }
        set { isMale = value; }
    }

    public int PlayerLevel
    {

        get { return playerLevel; }
        set { playerLevel = value; }
    }

    public int PlayerExp
    {

        get { return playerExp; }
        set { playerExp = value; }
    }

    public int NextLevelUpAmount {

        get { return nextLevelUpAmount; }
        set { nextLevelUpAmount = value; }
    }

    public int MoneyAmount
    {
        get { return moneyAmount; }
        set { moneyAmount = value; }

    }

    public int MaxStaminaValue
    {

        get { return maxStaminaValue; }
        set { maxStaminaValue = value; }
    }

    public int MaxEnergyValue
    {
        get { return maxEnergyValue; }
        set { maxEnergyValue = value; }
    }


    public int Stamina
    {

        get { return stamina; }
        set { stamina = value; }
    }

    public int Energy
    {
        get { return energy; }
        set { energy = value; }
    }

    public int Endurance
    {

        get { return endurance; }
        set { endurance = value; }
    }

    public int Intellect
    {

        get { return intellect; }
        set { intellect = value; }
    }

    public int Strength
    {

        get { return strength; }
        set { strength = value; }
    }

    public Weapon UsedWeapon
    {

        get { return usedWeapon; }
        set { usedWeapon = value; }
    }

    public void levelUp() {

        playerLevel++;
        nextLevelUpAmount *= (int) 1.5f;
        maxStaminaValue += 5;
        maxEnergyValue += 5;
        stamina = maxStaminaValue;
        energy = maxEnergyValue;
        endurance += 2;
        intellect += 2;
        strength += 2;

    }

    public void loseBattle()
    {
        // Stamina und Energy werden für den nächsten Kampf aufgefüllt
        stamina = maxStaminaValue;
        energy = maxEnergyValue;
        playerExp /= 2; // die Exp wird halbiert
    }

}
