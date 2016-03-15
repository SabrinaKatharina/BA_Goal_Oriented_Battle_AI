using UnityEngine;
using System.Collections.Generic;
using System.Linq;


// Klasse ist für das Spawnen der Monster und verwaltet die Monster
public class GameMachine : MonoBehaviour {

    public static GameMachine gameMachine;

    /* State Machines*/
    private enum GameStates {
        SIMULATION,   // zeige die Goal-oriented KI Gegner
        BATTLE,       // Kämpfe gegen die KI Gegner
    
    };
    [SerializeField]
    private GameStates currentGameState;
    private TurnBasedCombatStateMachine combatStateMachine;
    private bool isBattleStateInitialized = false;
    private bool isSimulationStateInitialized = true;

    /* Player*/
    public PlayerInformation player;           // enthält die Attribut-Werte
    private GameObject playerSprite;            // das Sprite mit Circle Collider für ShowPlayerData-Script
    public GameObject[] playerPref; // falls man mehrere Sprites hat
    private Vector3 playerSpriteTargetPosition;

    /* Timers */
    private float timer;
    private int spawnTimer;
    private int spawnTime; // Zeitpunkt, wann ein Monster erscheint
    // Range, in der die spawnTime liegen darf
    private int minTimeToSpawn = 500;
    private int maxTimeToSpawn = 5000;

    /* Monsters */
    private CaveList caveList;
    public GameObject[] monsterPrefs;        // alle Prefabs der Monster
    public List<Monster_Behaviour> monsterList = new List<Monster_Behaviour>();
    private int numOcupiedCaves;            // Anzahl aller besetzten Caves

    public int NumOcupiedCaves
    {
        get { return numOcupiedCaves; }
        set { numOcupiedCaves = value; }
    }

    public Vector3 PlayerPosition
    {
        get { return playerSpriteTargetPosition; }
        set { playerSpriteTargetPosition = value; }
    }

    // Use this for initialization
    void Start () {
        gameMachine = this;

        currentGameState = GameStates.SIMULATION;
        timer = Time.time; // die zeit zu beginn des Frames (= 0)
		caveList = GameObject.Find ("CaveList").GetComponent<CaveList>();
        combatStateMachine = GameObject.Find("CombatStateMachine").GetComponent<TurnBasedCombatStateMachine>();
		spawnTimer = 0;
        spawnTime = Random.Range (minTimeToSpawn, maxTimeToSpawn);
       
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInformation>();
        }
        if (player != null)
        {
            player.UsedWeapon = new SwordWeapon();
            player.ownedWeapons.Add(player.UsedWeapon);
        }
		//Debug
		Debug.Log ("Next spawnTime: " + spawnTime);

        //füge das erste Monster in die erste Höhle ein
		createMonster (caveList.caves [0], false);
        numOcupiedCaves = 1;
	}
	
	// Update is called once per frame
	void Update () {
        
        if(playerSprite != null)
        {
            playerSprite.transform.position = Vector3.Lerp(playerSprite.transform.position, playerSpriteTargetPosition, Time.deltaTime);
        }

        // wechsle in den BattleState
        if ( timer >= 80.0f || 
            Input.GetKeyDown(KeyCode.B) && monsterList.Count != 0)
        {
            currentGameState = GameStates.BATTLE;
            timer = 0.0f;
        }
        // wechsle in den SimulationState
        else if (Input.GetKeyDown(KeyCode.S)) {
            currentGameState = GameStates.SIMULATION;
        }

        /* führe die jeweiligen Aktionen in dem State aus */
        if (currentGameState == GameStates.SIMULATION)
        {
            timer += Time.deltaTime; // zähle timer hoch
            if (!isSimulationStateInitialized) {
                initializeSimulationState();
            }
            spawnTimer++; // zähle spawnTimer hoch
            if (spawnTimer > spawnTime || Input.GetKeyDown(KeyCode.N))
            { // ein Monster soll spawnen
                spawnMonster();
            }
        }
    
        else if (currentGameState == GameStates.BATTLE)
        {
            if (!isBattleStateInitialized)
            {
                initializeBattleState();
            }

            // prüfe, ob die TurnBasedCombatStateMachine in den END State wechselt (dann ist der Kampf vorbei)
            if (combatStateMachine.currentState == TurnBasedCombatStateMachine.BattleStates.END) {

                Debug.Log("GameMachine: Battle over.");
                currentGameState = GameStates.SIMULATION;
            }

        }

    }

    // gibt die nächste freie MonsterCave zurück, damit diese von einem erwachsen gewordenen MonsterChild bezogen werden kann
    public MonsterCave getNextFreeMonsterCave()
    {
        if (numOcupiedCaves < 6)
            return caveList.caves[numOcupiedCaves];
        else
            return null;
    }

	// erzeugt ein neues Random Monster; falls es einer Familie hinzugefügt werden soll, family = true
	public void createMonster(MonsterCave c, bool family)
	{
		GameObject newMonster = null;
		int monsterId = -1; // der prefab-Index (alle möglichen Monster Rassen)

		// falls das Monster keine Familie hat (= das erste Monster der MonsterCave)
		if (!family) {
			// wähle eine Monster Rasse aus
			monsterId = Random.Range (0, monsterPrefs.Length);
			// instanziere das Monster
			newMonster = Instantiate (
				monsterPrefs [monsterId], 
				c.transform.position, // position der Cave
				Quaternion.identity	  // Standard-Ausrichtung
				) as GameObject;
			newMonster.AddComponent<Monster_Behaviour>();
			newMonster.GetComponent<Monster_Behaviour>().initialize(getRacebyPref(monsterId));
			string race = newMonster.GetComponent<Monster_Behaviour>().getMonsterData().getRace();
			newMonster.gameObject.name = race + " " + newMonster.GetComponent<Monster_Behaviour>().getMonsterCount();
			newMonster.GetComponent<Monster_Behaviour>().setMonsterCave(c);
			newMonster.GetComponent<Monster_Behaviour>().setCavePosition(0);
			c.monster [0] = newMonster;   // füge es der Cave an Stelle [0] hinzu
			c.occupied = true; // MonsterCave c ist nun besetzt
            numOcupiedCaves++; // eine MonsterCave weniger ist frei
            newMonster.AddComponent<Goal_Manager>();

			monsterList.Add (newMonster.GetComponent<Monster_Behaviour> ()); // füge es der monsterList hinzu
		} 
		// falls das Monster einer Familie hinzugefügt wird ( = Partner oder Child)
		else {
			Vector3 vec = new Vector3(0.0f, 0.0f, 0.0f); //Verschiebungsvektor für die position der Monster

			GameObject monster = null;
			int cavePosition = -1; // die Position im Cave, zu der das neue Monster hinzugefügt wird
			// das monster[1] bekommt ein Partner; ist falls das monster[0] gelöscht wurde
			if(c.monster[0] == null){
				if(c.monster[1] != null){
					monster = c.monster[1];
					cavePosition = 0;
					vec = new Vector3(0.0f,0.0f,0.0f); 
				}
			}
			//das monster[0] bekommt ein Partner; normalfall
			else if(c.monster[1] == null){
				if(c.monster[0] != null){
					monster = c.monster[0];
					cavePosition = 1;
					vec = new Vector3(15.0f,0.0f,0.0f); 
				}
			}
			// das monster soll ein Kind bekommen
			else if(c.monster[0] != null && c.monster[1] != null && c.monster[2] == null){

				monster = c.monster[0];
				cavePosition = 2;
				vec = new Vector3(-15.0f,-3.0f,0.0f); 
			}
            else if(c.monster[0] != null && c.monster[1] != null && c.monster[2] != null) { Debug.Log("want create child but it is not null"); }
			if(monster == null){

				Debug.Log("Fehler bei create Partner/Child: monster = null");

			}
			// welche Rasse wohnt in der Cave
			monsterId = getPrefbyRace(monster.GetComponent<Monster_Behaviour>());
			//instanziere das Monster
			newMonster = Instantiate(
				monsterPrefs[monsterId],
				c.transform.position + vec,
				Quaternion.identity
				) as GameObject;
			newMonster.AddComponent<Monster_Behaviour>();
			newMonster.GetComponent<Monster_Behaviour>().initialize(getRacebyPref(monsterId));
			string race = newMonster.GetComponent<Monster_Behaviour>().getMonsterData().getRace();
			newMonster.gameObject.name = race + " " + newMonster.GetComponent<Monster_Behaviour>().getMonsterCount();
			newMonster.GetComponent<Monster_Behaviour>().setMonsterCave(c);
			newMonster.GetComponent<Monster_Behaviour>().setCavePosition(cavePosition);
			// unterscheide das newMonster
			switch(cavePosition){

			case 0: 
				c.monster[0] = newMonster;// füge es der Cave an Stelle [0] hinzu
				// füge Referenz partner hinzu
				newMonster.GetComponent<Monster_Behaviour> ().getMonsterData().setPartner(monster.GetComponent<Monster_Behaviour>());
				// setze statt der FindPartnerAction die PairAction
				newMonster.GetComponent<Monster_Behaviour> ().addPairAction();
				break;
			case 1:
				c.monster [1] = newMonster;   // füge es der Cave an Stelle [1] hinzu
				// füge Referenz partner hinzu
				newMonster.GetComponent<Monster_Behaviour> ().getMonsterData().setPartner(monster.GetComponent<Monster_Behaviour>());
				// setze statt der FindPartnerAction die PairAction
				newMonster.GetComponent<Monster_Behaviour> ().addPairAction();
				break;
			case 2:
				newMonster.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f); // verkleinere den Sprite
				c.monster [2] = newMonster;   // füge es der Cave an Stelle [2] hinzu
				// setze statt der FindPartnerAction die PlayAction
				newMonster.GetComponent<Monster_Behaviour> ().addPlayAction();
				break;
			}

            newMonster.AddComponent<Goal_Manager>();
			monsterList.Add (newMonster.GetComponent<Monster_Behaviour> ()); // füge es der monsterList hinzu

			}

	}

	public void destroyMonster(MonsterCave c, Monster_Behaviour monster, int index){
		Debug.Log (monster.name + " is destroyed");
        
                                    
        c.monster [index] = null; // entferne das Monster aus der Cave
                                  
        //wird später wieder auf true gesetzt, falls ein Monster in der Cave gefunden wurde
        c.occupied = false;
        numOcupiedCaves--;
		// setze die possibleActions[3] neu wenn Partner gelöscht wurde

		if (c.monster [(index + 1) % 3] != null || c.monster [(index + 2) % 3] != null && index != 2) {
			switch (index) {
			// das erste Monster wurde gelöscht => Partner für c.monster[1] fehlt
			case 0:
				if(c.monster[1] != null){
					c.monster[1].GetComponent<Monster_Behaviour>().getMonsterData().setPartner(null);
					c.monster[1].GetComponent<Monster_Behaviour>().getCurrDisc().addValueAtIndex(2, -60);
					c.monster [1].GetComponent<Monster_Behaviour> ().addFindPartnerAction ();}
				break;
			// das zweite Monster wurde gelöscht => Partner für c.monster[0] fehlt
			case 1:
				if(c.monster[0] != null){
					c.monster[0].GetComponent<Monster_Behaviour>().getMonsterData().setPartner(null);
					c.monster[0].GetComponent<Monster_Behaviour>().getCurrDisc().addValueAtIndex(2, -60);
					c.monster [0].GetComponent<Monster_Behaviour> ().addFindPartnerAction ();}
				break;
			case 2:
				if(c.monster [(index + 1) % 3] != null ){
					c.monster[(index + 1) % 3].GetComponent<Monster_Behaviour>().getMonsterData().setChild(null);
					c.monster[(index + 1) % 3].GetComponent<Monster_Behaviour>().getCurrDisc().addValueAtIndex(2, -40);}
				if(c.monster [(index + 2) % 3] != null ){
					c.monster[(index + 2) % 3].GetComponent<Monster_Behaviour>().getMonsterData().setChild(null);
					c.monster[(index + 2) % 3].GetComponent<Monster_Behaviour>().getCurrDisc().addValueAtIndex(2, -40);}
				break;
			}

		}
		
		// prüfe , ob noch ein Monster in der Cave ist
		for (int i = 0; i < c.monster.Length; ++i) {
			if(c.monster[i] != null){
				c.occupied = true;
                numOcupiedCaves++;
				break;
			}

		}

        monsterList.Remove(monster); // entferne es aus der monsterList
        Destroy(monster.gameObject);

    }

	// erhalte den pref-Index anhand der Rasse
	public int getPrefbyRace(Monster_Behaviour monster){

		int monsterId = -1;
		string monsterRace = monster.getMonsterData ().getRace ();
		switch (monsterRace) {
		case "Mooty" : monsterId = 0;
			break;
		case "Pajaro" : monsterId = 1;
			break;
        case "Frostus": monsterId = 2;
              break;
		}
		return monsterId;
	}

	// erhalte die Race anhand den pref-Index
	public string getRacebyPref(int prefId){

		string race = "";
		switch (prefId) {
		case 0: race = "Mooty";
			break;
		case 1: race = "Pajaro";
			break;
        case 2: race = "Frostus";
            break;
		}
		return race;
	}

    public List<Monster_Behaviour> getCombatReadinessSortedList(List<Monster_Behaviour> list)
    {
        
       return list.OrderBy(monster => monster.CombatReadinessFactor).ToList();
    }
    

    public void getEnemyQueue() {
        Queue<Monster_Behaviour> q = new Queue<Monster_Behaviour>();

        // rufe die Sort-Funktion auf, welche die Gegner nach der höchsten Kampfbereitschaft in die Queue sortiert

        List<Monster_Behaviour> sortedMonsterList = getCombatReadinessSortedList(monsterList);

        foreach (Monster_Behaviour monster in sortedMonsterList) {
            if (!monster.IsChild)
            {
                q.Enqueue(monster); // Füge die sortierten Monster nacheinander der Schlange hinzu
            }
        }

        combatStateMachine.enemyQueue = q; // speichere die enemyQueue

        // verschiebe die Monster zu einer Schlange

        Monster_Behaviour[] temp = new Monster_Behaviour[q.Count];

        q.CopyTo(temp, 0);

        GameObject queueStartPosObject = GameObject.Find("QueuePositionObject");
        Vector3 startPos = queueStartPosObject.transform.position;
        float offset = 25.0f;

        for (int i = 0; i < temp.Length; i++) {
            Vector3 newPos = startPos;
            newPos.x += (i * offset);
            temp[i].targetPosition = newPos;
        }



    }

    private void monsterReturnIntoCaves() {

        foreach (Monster_Behaviour monster in monsterList)
        {
            monster.inCombat = false;
            Vector3 monsterPos = monster.getMonsterCave().transform.position;
            
            if (monster.getCavePosition() == 1)
            {
                monsterPos += new Vector3(15, 0, 0);
            }
            else if (monster.getCavePosition() == 2) {
                monsterPos += new Vector3(-15, -3, 0);
            }
            monster.targetPosition = monsterPos;


        }
    }

    // wird einmalig ausgeführt, wenn der aktuelle gameState == BattleState ist
    private void initializeBattleState() {

        isSimulationStateInitialized = false;  // SimulationState ist nicht mehr instanziiert
        int playerPrefId = 0;
        if (player.IsMale)
            playerPrefId = 1;

        /* Instanziiere den PlayerSprite*/
        Debug.Log("Instantiate PlayerSprite");
        GameObject queueStartPosObject = GameObject.Find("QueuePositionObject");
        playerSprite = Instantiate(
            playerPref[playerPrefId],
            new Vector3(queueStartPosObject.transform.position.x - 200f, queueStartPosObject.transform.position.y, -2f),
            Quaternion.identity   // Standard-Ausrichtung
            ) as GameObject;
        playerSprite.name = "Player Sprite";
        playerSpriteTargetPosition = queueStartPosObject.transform.position - new Vector3(30f, 0f, 0f);

        // es müssen alle Monster inCombat gesetzt werden, auch wenn sie eventuell nicht in der Queue sind (zB Kinder); damit die Simulation nicht weiterläuft
        foreach (Monster_Behaviour monster in monsterList) 
        {
            monster.inCombat = true;
        }

        // starte die TurnBasedCombatStateMachine
        combatStateMachine.currentState = TurnBasedCombatStateMachine.BattleStates.START;

        getEnemyQueue();  // erzeugt die Monster-Queue gegen die der Spieler kämpfen wird
        isBattleStateInitialized = true; // damit es nur einmal ausgeführt wird



    }

    // wird einmalig ausgeführt, wenn der aktuelle gameState == SimulationState ist
    private void initializeSimulationState() {
        isBattleStateInitialized = false;
        isSimulationStateInitialized = true;
     
        playerSprite.GetComponent<ShowPlayerData>().deleteText();  // playerUi wird nicht mehr angezeigt, da das Sprite anschließend zerstört wird 
        Camera.main.GetComponent<CameraMovement>().inBattle = false;
        Destroy(playerSprite);

        /* zerstöre alle besiegten Monster und lösche sie auch aus der monsterList */
        Queue<Monster_Behaviour> q = combatStateMachine.getEnemyQueue(); // hole die aktulle enemyQueue die alle überlebenden Monster enthält

        
        // zerstöre alle Monster aus der MonsterListe
        // to-do: überlegen wegen Monster Childs
        while (combatStateMachine.DeadEnemies.Count != 0)
        {
            Monster_Behaviour monster = combatStateMachine.DeadEnemies[0];
            monster.GetComponent<ShowMonsterData>().doNotShowData();
            combatStateMachine.DeadEnemies.Remove(monster);
            destroyMonster(monster.getMonsterCave(), monster, monster.getCavePosition());
        }

        //prüfe, ob die enemyQueue überhaupt noch Monster hat
        if (q.Count > 0)
        {
            combatStateMachine.clearEnemyQueue();                                // lösche die enemyQueue ( sie wird in jedem Battle neu erstellt)
            monsterReturnIntoCaves(); // die überlebenden Monster werden in ihre ursprüngliche Höhlenposition gebracht
        }


    }

    // spawne ein Monster
    private void spawnMonster() {

        //entscheide, welche Cave frei ist
        MonsterCave freeCave = null; // die mögliche, besetzbare Cave (Kein Monster ist drin)
                                     // Laufe die CaveList durch
        foreach (MonsterCave c in caveList.caves)
        {
            if (!c.occupied)
            {
                freeCave = c; //Speichere die MonsterCave
                break;
            }
        }

        // falls eine freeCave gefunden wurde
        if (freeCave != null)
        {
            createMonster(freeCave, false);
        }
        else {
            Debug.Log("All caves are occupied.");
        }
        spawnTimer = 0; //Timer wird neu gestartet
        spawnTime = Random.Range(minTimeToSpawn, maxTimeToSpawn); // neue spawnTime wird ermittelt für das nächste Monster
        Debug.Log("Next spawnTime: " + spawnTime);
    }
}
