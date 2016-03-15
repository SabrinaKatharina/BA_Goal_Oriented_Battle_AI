using UnityEngine;
using System.Collections;

public class Monster_Data{

	private string race;
	private Monster_Behaviour partner;
	private Monster_Behaviour child;
    public enum ElementTypes{ Fire, Ice, Water};
    private ElementTypes elementType;
	private int[] monsterAttributs;
	// 0 = HP
	// 1 = ATK
	// 2 = DEF
	// 3 = HUMOR
	// 4 = Naivity
	// 5 = ANXIETY
	// 6 = INDEPENDENCY
	// 7 = VANITY

	public Monster_Data(string race){
		this.race = race;
        //entscheide anhand der Rasse den elementType
        if (race.Equals("Pajaro"))
        {
            elementType = ElementTypes.Fire;
        }
        else if (race.Equals("Mooty"))
        {
            elementType = ElementTypes.Water;

        }
        else {
            elementType = ElementTypes.Ice;
        }
		partner = null;
		child = null;
		monsterAttributs = new int[8];

        // HP, Atk und Def werden über ein eigenes Script berechnet
        CalculateNewEnemyStats newStatsScript = new CalculateNewEnemyStats();
        //speichere den Player Level
        int playerLevel = GameMachine.gameMachine.player.PlayerLevel;
        monsterAttributs[0] = newStatsScript.calculateStat(Random.Range(80,120), CalculateNewEnemyStats.StatType.HP, playerLevel); // die BaseHP liegen zwischen 80 und 120
        monsterAttributs[1] = newStatsScript.calculateStat(Random.Range(15, 20), CalculateNewEnemyStats.StatType.ATK, playerLevel); // die BaseAtk liegt zwischen 15 und 20
        monsterAttributs[2] = newStatsScript.calculateStat(Random.Range(15, 20), CalculateNewEnemyStats.StatType.DEF, playerLevel); // die BaseDef liegt zwischen 15 und 20

        //Die restlichen Attribute werden randomisiert. Bei Vererbung (wenn Child) werden die Attribute später neu gesetzt
        for (int i = 3; i < monsterAttributs.Length; ++i) {
			monsterAttributs[i] = (int) (100.0f * Random.Range(0.0f, 1.0f));
		}
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public string getRace(){

		return race;
	}

    public ElementTypes getElementType() {
        return elementType;
    }
	public Monster_Behaviour getPartner(){

		return partner;
	}

	public bool hasPartner(){

		return partner != null;
	}

	public void setPartner(Monster_Behaviour partner){
		this.partner = partner;
	}

	public Monster_Behaviour getChild(){
		
		return child;
	}

	public bool hasChild(){
		return child != null;
	}
	
	public void setChild(Monster_Behaviour child){
		this.child = child;
	}

	public int getAttributeValueAtIndex(int index){

		return monsterAttributs [index];
	}

	public void setAttributeAtIndex(int index, int value){

		monsterAttributs [index] = value;
	}

    



}
