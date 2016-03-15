using UnityEngine;
using System.Collections;

public class Monster_Behaviour : MonoBehaviour {

	public bool initialized;
    public bool inCombat;
	public static int monsterCount = 0;
	private Monster_Data monsterData;
    private string monsterLog; // speichert die Debug-Log-Anzeigen eines Monsters
	public Discontentment currentDiscontentment; // das aktuelle Discontentment
	public Action[] possibleActions; // Liste aller verfügbaren Actions
	/*  0: Rest => tired -40, love +5
	 * 	1: Eat =>  tired + 5, hunger -40
	 *  2: ExploreArea => love -20
	 *  3: FindPartner => love -50 
	 * 	  if monster has partner
	 *  3: Pair => tired + 10, love -45
	 * 	  if monster is a child
	 * 	3: Play => tired +5, love -50
	*/
	private MonsterCave cave;
	private int cavePosition;
    public Vector3 targetPosition;
    public int clock;
    [SerializeField]
    private int childRoundCounter; // counter für die Anzahl an Runden, die 1h dauern ; Child benötigt 5 Runden um erwachsen zu werden
    private int maxStamina;
    [SerializeField]
    private float combatReadinessFactor;

    public int MaxStamina { get { return maxStamina; } set { maxStamina = value; } }
	
    public bool IsChild { get { return cavePosition == 2; } }

    public string MonsterLog { get { return monsterLog; } }

    public void updateMonsterLog(string newLogData)
    {
        monsterLog += "\n" + newLogData;
        float screenHeightFactor = Screen.height / 608f;
        if(monsterLog.Length > (int)(1500*screenHeightFactor))
        {
            int cutLength = monsterLog.Length - (int)(1500*screenHeightFactor);
            int startIndex = monsterLog.IndexOf("\n");
            monsterLog = monsterLog.Remove(startIndex+1, cutLength);
        }
    }

	public void initialize(string race)
	{
		if (initialized)
			return;

        targetPosition = transform.position;
		initialized = true;
		monsterCount++;
		// clock initialisieren
		clock = 0;
        childRoundCounter = 0;
		// Mögliche Actions erzeugen
		possibleActions = new Action[4];
		// monsterData erzeugen
		monsterData = new Monster_Data (race);
        // Monster ist nicht im Kampf
        inCombat = false;
        Discontentment restDisc = new Discontentment();
		restDisc.setValueAtIndex(0, -40);
		restDisc.setValueAtIndex (1, +5);
		
		Discontentment eatDisc = new Discontentment();
		eatDisc.setValueAtIndex(0, +5);
		eatDisc.setValueAtIndex(1, -40);

		Discontentment exploreAreaDisc = new Discontentment ();
		exploreAreaDisc.setValueAtIndex (2, -20);
		
		Discontentment findPartnerDisc = new Discontentment();
		findPartnerDisc.setValueAtIndex(2,-50);
		
		possibleActions [0] = new Action ("Rest", 0.25, restDisc);
		possibleActions [1] = new Action ("Eat", 0.5, eatDisc);
		possibleActions [2] = new Action ("ExploreArea", 0.25, exploreAreaDisc);
		possibleActions [3] = new Action ("FindPartner", 0.0, findPartnerDisc);
		
		currentDiscontentment = new Discontentment ();
        maxStamina = monsterData.getAttributeValueAtIndex(0);
        combatReadinessFactor = getCombatReadinessFactor();
	}

	// Update is called once per frame
	void Update () {
        combatReadinessFactor = getCombatReadinessFactor();
        if (Input.GetKeyDown(KeyCode.O))
            {
            Debug.Log("Stamina: " +(float) (getMonsterData().getAttributeValueAtIndex(0) / (float) maxStamina) + " * Angst: " + (float)(getMonsterData().getAttributeValueAtIndex(5) / 100f));
        }
        if (!inCombat)
        {
            clock++;
            // erhöhe den currentDiscontentment des Monster nach einer gewissen Zeit
            // Hunger => + 10 / hour
            // Love =>   + 20 / hour

            if (clock >= 600)
            {

                clock = 0;
                currentDiscontentment.addValueAtIndex(1, 10);
                currentDiscontentment.addValueAtIndex(2, 60);
                 
                if (!IsChild)  // nur falls es kein Child ist
                {
                    checkLoveValue();
                }
                else
                {
                    childRoundCounter++;
                }
               
            }
            else if (currentDiscontentment.getTotalDiscontentment(0.0) >= 10000)
            {

                findBestAction();
                GetComponentInParent<Goal_Manager>().craftingThinkCycle();


            }

            // prüfe, ob es ist ein Child
            if (IsChild) { 
                if (childRoundCounter >= 5 && GameMachine.gameMachine.NumOcupiedCaves < 6) // prüfe ob es erwachsen werden und ausziehen kann und ob mindestens eine MonsterCave frei ist
                {
                    //Debug.Log(this.name + " grows up.");
                    this.updateMonsterLog(this.name + " grows up.");
                    childRoundCounter = 0;  // wird auf 0 gesetzt, der unveränderliche Wert der erwaschsenen Monster
                    growUp();
                }
            }
        } // Ende: if(!inCombat)


        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime); // lerpe an die TargetPosition
	}

	public void setMonsterCave(MonsterCave c){
		cave = c;
	}

    public MonsterCave getMonsterCave() {
        return cave;
    }

	public void setCavePosition(int pos){
		cavePosition = pos;
       
	}

    public int getCavePosition()
    {
        return cavePosition;
    }
    public int getMonsterCount() {
		return monsterCount;
	}

	// für den UI-Text, damit jeder Wert ausgelesen werden kann
	public int getCurrDiscValueAtIndex(int index){
		
		return currentDiscontentment.getValueAtIndex(index);
	}

	// Partner: statt FindPartner kann nun die Pair Action aufgerufen werden (ebenfalls beeinflusst durch Love)
	public void addPairAction(){
		
		Discontentment pairDisc = new Discontentment();
		pairDisc.setValueAtIndex (0, +10);
		pairDisc.setValueAtIndex (2, -45);
		possibleActions [3] = new Action ("Pair", 0.25, pairDisc);
	}

	// Child: statt FindPartner kann nun die Play Action aufgerufen werden (ebenfalls beeinflusst durch Love)
	public void addPlayAction(){
		Discontentment playDisc = new Discontentment ();
		playDisc.setValueAtIndex (0, +5);
		playDisc.setValueAtIndex (2, -50);
		possibleActions [3] = new Action ("Play", 0.5, playDisc);
	}

	public void addFindPartnerAction(){
		Discontentment findPartnerDisc = new Discontentment();
		findPartnerDisc.setValueAtIndex(2,-50);
		possibleActions [3] = new Action ("Find", 0.0, findPartnerDisc);
	}

	public Discontentment getCurrDisc()
	{
		return currentDiscontentment;
	}


	public int getClock(){
		return clock;
	}

	public Monster_Data getMonsterData(){
		return monsterData;
	}

	void findBestAction(){

		Action bestAction = null;
		Discontentment bestDisc = new Discontentment (100);
		double bestDiscTotal = bestDisc.getTotalDiscontentment (0.0);

		int endOfPossibleActions = possibleActions.Length-1; // da FindPartnerAction und PairAction durch andere Werte ausgeführt werden sollen
		// falls Child, soll es die PlayAction aufrufen dürfen
		if (possibleActions [3].getName ().Equals ("Play")) {
			endOfPossibleActions = possibleActions.Length;
		}
		for(int i = 0; i < endOfPossibleActions; ++i)
			{
			Discontentment tempDisc = new Discontentment(currentDiscontentment); // Speichere den aktuellen Discontentment des Monsters

			// der Veränderungswert jeder der möglichen Aktionen
			//werden hinzugefügt
			tempDisc.addDiscontentment(possibleActions[i].getDeltaDisc()); 

			double durationOfAction = possibleActions[i].getDuration();
			double newDiscTotal = tempDisc.getTotalDiscontentment(durationOfAction); //Speichere den Totalen Wert

			if(newDiscTotal < bestDiscTotal)
				{
				bestDiscTotal = newDiscTotal;
				bestAction = possibleActions[i];
				}
			}

        //Debug.Log (this.name + ": bestAction: " + bestAction.getName ());
        this.updateMonsterLog(this.name + ": bestAction: " + bestAction.getName());
		// führe bestAction aus:
		currentDiscontentment.addDiscontentment (bestAction.getDeltaDisc ());
        // prüfe, ob das Monster die exploreAction ausgeführt hat; dann füge eine Random Anzahl an CollectableItems hinzu
        if (bestAction.getName().Equals("ExploreArea")) {

            int randomVal = (Random.Range(0, 100) % 2);
            int itemAmount = Random.Range(1, 5);
            Item.ItemType itemType = Item.ItemType.DEF_BONUS;
            if(randomVal == 0)
            {
                itemType = Item.ItemType.MEAT;
            }
            else if(randomVal == 1)
            {
                itemType = Item.ItemType.WOOD;
            }

            //Debug.Log("Add Items: " + itemType.ToString() + " " + itemAmount);
            this.updateMonsterLog("Add Items: " + itemType.ToString() + " " + itemAmount);
            for (int i = 0; i < itemAmount; i++)
            {
                this.GetComponentInParent<ItemManager>().collectableItems.Add(new CollectableItem(itemType));
            }
        }
	}

	void makeSinglePointCrossover(MonsterCave c){
		Monster_Data parent0 = c.monster [0].GetComponent<Monster_Behaviour> ().getMonsterData ();
		Monster_Data parent1 = c.monster [1].GetComponent<Monster_Behaviour> ().getMonsterData ();
		Monster_Data child   = c.monster [2].GetComponent<Monster_Behaviour> ().getMonsterData ();
		
		// Single Point Crossover Blöcke auswählen; die Monster-Attribute sind int-Werte
		
		int[] blocks = new int[7];
		// initalisiere die block-werte mit den Werten der Eltern
		int randomCrossoverPoint = Random.Range (1, 7);
		for (int i = 0; i < blocks.Length; ++i) {
			// nehme die Attribute von parent0 bis zum crossoverPoint
			if(i < randomCrossoverPoint){
				blocks[i] = parent0.getAttributeValueAtIndex(i+1); // i+1, da das erste Attribut die HP-Anzahl ist
				// füge ein mögliche Variantion zu dem Wert hinzu
				blocks[i] += getPossibleVariation();
				if (blocks[i] > 100) {
					blocks[i] = 100;
				}
			}
			// nehme die restlichen Attribute von parent1
			else{
				blocks[i] = parent1.getAttributeValueAtIndex(i+1);
				// füge ein mögliche Variantion zu dem Wert hinzu
				blocks[i] += getPossibleVariation();
				if (blocks[i] > 100) {
					blocks[i] = 100;
				}
			}
		}
		// Kind bekommt die Werte der blocks
		for (int i = 0; i < blocks.Length; ++i) {
			child.setAttributeAtIndex(i+1, blocks[i]);
		}

	}

	void makeMultiPointCrossover(MonsterCave c){
		Monster_Data parent0 = c.monster [0].GetComponent<Monster_Behaviour> ().getMonsterData ();
		Monster_Data parent1 = c.monster [1].GetComponent<Monster_Behaviour> ().getMonsterData ();
		Monster_Data child   = c.monster [2].GetComponent<Monster_Behaviour> ().getMonsterData ();

		// Multi Point Crossover Blöcke auswählen; die Monster-Attribute sind int-Werte

		int[] blocks = new int[7];
		// initalisiere die block-werte mit den Werten der Eltern
		for (int i = 0; i < blocks.Length; ++i) {
			// random auswählen, welcher Block von welchem Elternteil kommt
			int randomVal = Random.Range (0, 100);
			// Block von parent0
			if(randomVal % 2 == 0){

				blocks[i] = parent0.getAttributeValueAtIndex(i+1); // i+1, da das erste Attribut die HP-Anzahl ist
				// füge ein mögliche Variantion zu dem Wert hinzu
				blocks[i] += getPossibleVariation();
				if (blocks[i] > 100) {
					blocks[i] = 100;
				}


			}
			// Block von parent1
			else if(randomVal % 2 == 1){
				blocks[i] = parent1.getAttributeValueAtIndex(i+1);
				// füge ein mögliche Variantion zu dem Wert hinzu
				blocks[i] += getPossibleVariation();
				if (blocks[i] > 100) {
					blocks[i] = 100;
				}
			} else {Debug.Log ("multiPointCrossover : RandomVal Error");}
		}
		// Kind bekommt die Werte der blocks
		for (int i = 0; i < blocks.Length; ++i) {
			child.setAttributeAtIndex(i+1, blocks[i]);
		}
	}

	int getPossibleVariation(){
		int variation = 0;
		//zufällige Variation zu 30%
		float variationPossibility = Random.Range(0.0f, 1.0f);
		if(variationPossibility >= 0.7f){
			// Variation liegt zwischen 1 und 10
			variation = Random.Range(1,11);
		}
		return variation;
	}

	int getAttractiveness(Monster_Data data){
		// Humor + Naivität/2 - Ängstlichkeit + Selbstständigkeit - Eitelkeit = Anziehungskraft
		return (int) (data.getAttributeValueAtIndex(3) + 0.5 * data.getAttributeValueAtIndex(4) - data.getAttributeValueAtIndex(5)
			+ data.getAttributeValueAtIndex(6)  - data.getAttributeValueAtIndex(7));
	}

    public float CombatReadinessFactor
    {
        get
        {
           return combatReadinessFactor;
        }
    }

    float getCombatReadinessFactor()
    {
        return (float)(getMonsterData().getAttributeValueAtIndex(0) / (float)maxStamina) * (float)(getMonsterData().getAttributeValueAtIndex(5) / 100f);
       
    }

    void executeFindPartnerAction(){
		currentDiscontentment.addDiscontentment (possibleActions[3].getDeltaDisc ());
		int cavePosPartner = -1;
		// falls das erste Monster nicht da ist, bekommt monster[1] einen Partner
		if(this.cave.monster[0] == null)
		{
			if(this.cave.monster[1] != null){
				cavePosPartner = 1;
			}
		
		}
		else if(this.cave.monster[1] == null){
			if(this.cave.monster[0] != null){
				cavePosPartner = 0;
			}
		}

		//Debug.Log ("create partner");
        this.updateMonsterLog("create partner");
        // erzeuge den Partner
        GameMachine.gameMachine.createMonster (this.cave, true);
		// setze Partner
		Monster_Behaviour partner = this.cave.monster[cavePosPartner].GetComponent<Monster_Behaviour> ();
		this.cave.monster [cavePosition].GetComponent<Monster_Behaviour> ().addPairAction ();
		this.cave.monster [cavePosition].GetComponent<Monster_Behaviour> ().getMonsterData ().setPartner (partner);

			

	}

	void executePairAction(){

		this.currentDiscontentment.addDiscontentment (possibleActions[3].getDeltaDisc ());
		Monster_Behaviour partner = this.getMonsterData ().getPartner ();
		partner.getCurrDisc().addDiscontentment (possibleActions [3].getDeltaDisc ());

		// prüfe, ob kein Kind bereits existiert (Monster können nur ein Kind haben)
		if (this.cave.monster[2] == null) {
			// Wahrscheinlichkeit , dass ein Kind erzeugt wird
			float probability = Random.Range (0.0f, 1.0f);
			//Debug.Log ("probability to get a child: " + probability);
            this.updateMonsterLog("probability to get a child: " + probability);
            if (probability <= 0.33f) {

				if (this.cave.monster [0] != null && this.cave.monster [1] != null) { // nochmal prüfen, ob beide Eltern existieren
					Monster_Behaviour monster = this.cave.monster [0].GetComponent<Monster_Behaviour> ();
					Monster_Behaviour partnerMonster = this.cave.monster [1].GetComponent<Monster_Behaviour> ();
					if (this == monster || this == partnerMonster) { //es ist egal welcher Partner die pairAction ausführt (sie wird nur einmal für beide ausgeführt)
						// erzeuge das Child
						GameMachine.gameMachine.createMonster (this.cave, true);
						// setze bei beiden Eltern das newMonster als child
						Monster_Behaviour child = this.cave.monster [2].GetComponent<Monster_Behaviour> ();
						monster.getMonsterData ().setChild (child);
						this.cave.monster [1].GetComponent<Monster_Behaviour> ().getMonsterData ().setChild (child);
						// vererbe die Child-Attributes mit Multi Point Crossover-Prinzip und Variation zu 30%
						// oder mit SinglePoint Crossover-Prinzip mit Variation zu 30%
						int randomVal = Random.Range (0, 100);
						if (randomVal % 2 == 0) {
							makeMultiPointCrossover (this.cave);
							//Debug.Log ("child created with Multi Point Crossover");
                            this.updateMonsterLog("child created with Multi Point Crossover");
                        } else if (randomVal % 2 == 1) {
							makeSinglePointCrossover (this.cave);
							//Debug.Log ("child created with Single Point Crossover");
                            this.updateMonsterLog("child created with Single Point Crossover");
                        }
					}
				
				}
			}
		}
	}

    void growUp()
    {
        this.cave.monster[2] = null; // das Kind wird freigegeben
        // prüfe, ob das Kind Eltern hat; falls ja muss es in eine andere Höhle ziehen
        if (this.cave.monster[0] != null || this.cave.monster[1] != null)
        {
            // das Kind besetzt die nächste freie MonsterCave
            this.setMonsterCave(GameMachine.gameMachine.getNextFreeMonsterCave());
            this.cave.occupied = true;
            GameMachine.gameMachine.NumOcupiedCaves++;
           
        }
        // das Kind kann zu Position 0 wandern (von neuer Cave oder falls es keine Eltern hat, von der jetzigen MonsterCave)
        this.setCavePosition(0);
        this.cave.monster[0] = this.gameObject;
        // neue Position und Sprite-Scale Wert
        this.targetPosition = (this.cave.transform.position);
        int newLocalScale = 4;  // vergrößere das Sprite um den scale-Wert 4
        if (this.monsterData.getRace().Equals("Frostus"))
            newLocalScale = 3; // Frostus nur um 3
            
        this.transform.localScale = new Vector3(newLocalScale, newLocalScale);
        this.addFindPartnerAction(); // füge die FindPartnerAction hinzu statt PlayAction

    }

    void checkLoveValue()
    {
        // falls der Love-Wert über 50 liegt
        if (currentDiscontentment.getValueAtIndex(2) >= 50)
        {
            // falls Monster noch keinen Partner hat, prüfe ob es einen Partner bekommen soll
            if (this.getMonsterData().getPartner() == null)
            {
                //Debug.Log(this.name + ": attractiveness: " + getAttractiveness(this.getMonsterData()));
                this.updateMonsterLog(this.name + ": attractiveness: " + getAttractiveness(this.getMonsterData()));
                float probabilityOfFindPartner = Random.Range(0.0f, 1.0f);
                if (getAttractiveness(this.getMonsterData()) > 0)
                {
                    if (probabilityOfFindPartner >= 0.25f)
                    {
                        executeFindPartnerAction();
                    }
                    else {
                       // Debug.Log(this.name + " is too unattractive to find a partner");
                       this.updateMonsterLog(this.name + " is too unattractive to find a partner");
                    }
                }
                else {
                    if (probabilityOfFindPartner >= 0.8f)
                    {
                        executeFindPartnerAction();
                    }
                    else {
                        //Debug.Log(this.name + " is too unattractive to find a partner");
                        this.updateMonsterLog(this.name + " is too unattractive to find a partner");
                    }
                }
            }
            else {

                // ermittle den Love-Wert von beiden Partnern und prüfe ob bei beiden der Love-Wert >= 50 ist
                Monster_Behaviour partner = this.getMonsterData().getPartner();

                if (partner.getCurrDiscValueAtIndex(2) >= 50)
                {
                    //Debug.Log(this.name + " executes PairAction");
                    this.updateMonsterLog(this.name + " executes PairAction");
                    executePairAction();

                }
            }
        }

    }

	public void destroy(){

		Destroy (this.gameObject);
	}


}
