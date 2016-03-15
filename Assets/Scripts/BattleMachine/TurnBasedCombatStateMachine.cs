using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnBasedCombatStateMachine : MonoBehaviour {

    // alle States eines Kampfes
    public enum BattleStates {
        INACTIVE,                        // BattleState ist unaktiv
        START,                           // Kampf vorbereiten
        PLAYERCHOICE,                    // Spieler wählt Aktion aus
        ENEMYCHOICE,                     // KI wählt Aktion aus
        CALCULATE_TURN,                  // Runden-Berechnung, zB. ausgeteilter Schaden, Anzahl heilender HP's 
        CALCULATE_PLAYER_WIN,             // berechne die erhaltenen Exp und erhaltenes Geld
        LOSE,                            // Spieler wurde besiegt
        END,                             // Battle ist abgeschlossen
    }; 

    public BattleStates currentState;   // Aktueller State
    public Queue<Monster_Behaviour> enemyQueue;
    private List<Monster_Behaviour> deadEnemies;
    public BattleStartScript battleStartScript;
    public PlayerChoiceScript playerChoiceScript;
    public EnemyChoiceScript enemyChoiceScript;
    public CalculateTurnScript calculateTurnScript;
    public CaluclatePlayerWinScript calculatePlayerWinScript;
    public Ability chosenAbility; // von Player oder von Enemy (Erster Gegner der Queue)

    public List<Monster_Behaviour> DeadEnemies { get { return deadEnemies; } }

    public AnimationCurve healPotionEffector;

    // bool Hilfsvariablen
    private bool isAlreadyCalculated = false;
    private bool isPlayerTurnOver = false;
    private bool isEnemyTurnOver = false;

    // Hilfsvariablen

    private int gainedExpCount;
    private int gainedMoneyCount;
    private int defendAbilityCounter;
    private int calculatedPlayerEndurance;
    private float endStateCounter;

    public GameObject battleButtons;
    public GameObject playerChoiceCharger;
    public GameObject enemyChoiceCharger;
    public Text playerLoseText;
    private PlayerInformation player;



	// Use this for initialization
	void Start () {

        enemyQueue = new Queue<Monster_Behaviour>();
        deadEnemies = new List<Monster_Behaviour>();
        currentState = BattleStates.INACTIVE;
      
       

        if (battleButtons == null)
        {
            battleButtons = GameObject.Find("BattleButtons");
        }
        battleButtons.SetActive(false);

        if (playerChoiceCharger == null) {
            playerChoiceCharger = GameObject.Find("PlayerChoiceCharger");
        }

        if(enemyChoiceCharger == null)
        {

            enemyChoiceCharger = GameObject.Find("EnemyChoiceCharger");
        }
        playerChoiceScript = this.GetComponent<PlayerChoiceScript>();
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerInformation>();
        }
    }

    void EnemyChoice()
    {
        enemyChoiceScript = new EnemyChoiceScript(enemyQueue.Peek(), enemyQueue.Count);
        chosenAbility = enemyChoiceScript.ChosenAbility;
        enemyChoiceCharger.SetActive(true);
        isAlreadyCalculated = true;

    }

    void BattleStart()
    {
        currentState = BattleStates.ENEMYCHOICE;
    }

    // Update is called once per frame
    void Update () {

        // führe die jeweiligen Aktionen in einem State aus

        switch (currentState)
        {
            case BattleStates.INACTIVE:
                break;

            case BattleStates.START:
                // reset der Hilfsvariablen
                gainedExpCount = 0;
                gainedMoneyCount = 0;
                defendAbilityCounter = 0;
                calculatedPlayerEndurance = player.Endurance;
                isAlreadyCalculated = false;
                isPlayerTurnOver = false;
                isEnemyTurnOver = false;


                 // Bereite den Kampf vor: entscheide, welche Partei zuerst angreifen darf
                battleStartScript = new BattleStartScript();
                int whoIsFirst = battleStartScript.decideWhoGoesFirst(); 
                if (whoIsFirst == 0)
                    currentState = BattleStates.PLAYERCHOICE;
                    
                else if (whoIsFirst == 1)
                {
                    currentState = BattleStates.INACTIVE;
                    Invoke("BattleStart", 4f);
                }
                   
               
                break;

            case BattleStates.PLAYERCHOICE:

                if (player.Energy >= 5)
                {
                    isAlreadyCalculated = false;   // eine Ability kann ausgeführt und berechnet werden
                    battleButtons.SetActive(true); // aktiviere die 4 Battle Buttons

                    // sobald ein Button gedrückt wurde
                    if (playerChoiceScript.IsChoiceDone)
                    {
                        battleButtons.SetActive(false);  // deaktiviere die 4 Battle Buttons
                        playerChoiceCharger.SetActive(true);
                        playerChoiceCharger.GetComponent<Slider>().value++; //fülle den Charger
                    }

                    // sobald der Ability Charger voll geladen ist, wechsle in den Calculate Turn State 
                    if (playerChoiceCharger.GetComponent<Slider>().value == playerChoiceCharger.GetComponent<Slider>().maxValue)
                    {

                        isPlayerTurnOver = true;
                        playerChoiceScript.IsChoiceDone = false;
                        playerChoiceCharger.GetComponent<Slider>().value = 0;
                        playerChoiceCharger.SetActive(false);
                        currentState = BattleStates.CALCULATE_TURN;
                    }
                }
                else
                {
                    isAlreadyCalculated = false;
                    Debug.Log("Player has no energy.");
                    currentState = BattleStates.CALCULATE_PLAYER_WIN;

                }

                break;

            case BattleStates.ENEMYCHOICE:

                if (!isAlreadyCalculated)
                {
                    // setze die chosenAbility
                    if (!IsInvoking("EnemyChoice"))
                        Invoke("EnemyChoice", 2f);    // warte, bis die Ability gesetzt wird und der Charger sich auffüllt
                    
                }
                if (isAlreadyCalculated)
                {
                    enemyChoiceCharger.GetComponent<Slider>().value++; //fülle den Charger

                    if (enemyChoiceCharger.GetComponent<Slider>().value == enemyChoiceCharger.GetComponent<Slider>().maxValue)
                    {
                        isEnemyTurnOver = true;
                        isAlreadyCalculated = false;
                        enemyChoiceCharger.GetComponent<Slider>().value = 0;
                        enemyChoiceCharger.SetActive(false);
                        currentState = BattleStates.CALCULATE_TURN;
                    }
                }
                break;

            case BattleStates.CALCULATE_TURN:

                
                if (!isAlreadyCalculated)
                {
                    isAlreadyCalculated = true; //damit nur einmal berechnet wird
                    int playerEndurance = player.Endurance;

                    // setze die Verteidigung auf die calculatedPlayerEndurance, falls der Player angegriffen wird;
                    if (chosenAbility.TypeName == Ability.AbilityTypes.ENEMYATTACK)
                    {
                        player.Endurance = calculatedPlayerEndurance;
                    }

                    // berechne den Zug mit der ausgewählten Ability und dem ersten Monster der Queue
                    calculateTurnScript = new CalculateTurnScript(chosenAbility, enemyQueue.Peek());

                    // setze den ursprünglichen Endurance Wert
                    player.Endurance = playerEndurance;

                    //prüfe, ob der Spieler geflohen ist
                    if (chosenAbility.TypeName == Ability.AbilityTypes.FLEE)
                    {


                        if (calculateTurnScript.hasPlayerEscaped())
                        {
                            isAlreadyCalculated = false;
                            currentState = BattleStates.CALCULATE_PLAYER_WIN;
                           
                            break;

                        }
                    }

                    //prüfe, ob der Spieler einen Defense Bonus erhalten hat
                    if (chosenAbility.TypeName == Ability.AbilityTypes.DEFEND)
                    {
                        defendAbilityCounter++;
                        if (defendAbilityCounter <= 3) // maximale Wirksamkeit der Defend-Ability liegt bei 3
                        {
                            // iterativer Aufruf, der immer vom Endurance-Wert des Players startet
                            int currentPlayerEndurance = player.Endurance;
                            int gainedDefenseBonus = 0;
                            for (int i = 0; i < defendAbilityCounter; i++) // wird so viel durchlaufen, wie die Defend-Ability ausgewählt wurde 
                            {

                                gainedDefenseBonus = calculateTurnScript.getDefenseBonus(currentPlayerEndurance);
                                currentPlayerEndurance += gainedDefenseBonus;

                            }
                            calculatedPlayerEndurance = currentPlayerEndurance; // speichere den neu berechneten Endurance-Wert des Players in einem Battle 
                            Debug.Log("DefenseBonus: " + gainedDefenseBonus + " CalculatedPlayerEndurance: " + calculatedPlayerEndurance);

                        }
                        else
                            Debug.Log("Defense can not increased anymore." + " CalculatedPlayerEndurance: " + calculatedPlayerEndurance);
                    }

                   
                    // prüfe, ob das Monster die Ability Dequeue ausgewählt hat
                    if(chosenAbility.TypeName == Ability.AbilityTypes.DEQUEUE)
                    {

                        Monster_Behaviour escapedEnemy = enemyQueue.Dequeue();
                        escapedEnemy.targetPosition = escapedEnemy.getMonsterCave().transform.position + Vector3.right*15*escapedEnemy.getCavePosition();
                     
                        // verschiebe den PlayerSprite nach rechts
                        GameMachine.gameMachine.PlayerPosition += Vector3.right * 20f;
                    }


                    //prüfe, ob in diesem Zug der Gegner besiegt wurde
                    if (calculateTurnScript.isEnemyDead())
                    {
                        Monster_Behaviour deadEnemy = enemyQueue.Dequeue();              // entferne das Monster aus der Queue
                        deadEnemies.Add(deadEnemy);
                        gainedExpCount += 30 + (int)(10 * deadEnemy.getMonsterData().getAttributeValueAtIndex(1)/ 100) ;   
                        gainedMoneyCount += 100;
                        deadEnemy.transform.Rotate(new Vector3(0.0f, 0.0f, -90.0f));     // drehe es um 90 grad
                        deadEnemy.transform.Translate(new Vector3(20.0f, 0.0f, 0.0f));   // verschiebe das Monster nach unten
                        deadEnemy.targetPosition = deadEnemy.transform.position;
                      // verschiebe den PlayerSprite nach rechts
                        GameMachine.gameMachine.PlayerPosition += Vector3.right * 20f;
                    }

                    // prüfe ob noch Monster in der Queue sind und ob der Player noch lebt: Dann neue Runde
                    if (enemyQueue.Count != 0 && player.Stamina > 0)
                    {

                        // entscheide welche Partei als nächstes dran ist
                        if (isPlayerTurnOver)
                        {
                            chosenAbility = null; // wieder freigeben
                            currentState = BattleStates.ENEMYCHOICE;
                            isPlayerTurnOver = false;  // wieder freigeben
                            isAlreadyCalculated = false;
                            break;
                        }
                        else if (isEnemyTurnOver)
                        {
                            currentState = BattleStates.PLAYERCHOICE;
                            isEnemyTurnOver = false;   // wieder freigeben
                            chosenAbility = null;
                            break;
                        }

                    }

                    // Player hat alle Monster besiegt und gewonnen
                    else if (enemyQueue.Count == 0)
                    {
                        chosenAbility = null;  // wieder freigeben
                        isAlreadyCalculated = false;
                        currentState = BattleStates.CALCULATE_PLAYER_WIN;
                        break;
                    }
                    // Player wurde besigt
                    else if (player.Stamina <= 0)
                    {
                        chosenAbility = null; // wieder freigeben
                        isAlreadyCalculated = false;
                        currentState = BattleStates.LOSE;
                        break;
                    }
                }
                break;

            case BattleStates.CALCULATE_PLAYER_WIN:
                if (!isAlreadyCalculated)
                {
                    calculatePlayerWinScript = new CaluclatePlayerWinScript(gainedExpCount, gainedMoneyCount);
                    isAlreadyCalculated = true;
                }
                endStateCounter += Time.deltaTime;
                if (endStateCounter >= 5f)
                {
                    calculatePlayerWinScript.deleteWinText();
                    currentState = BattleStates.END;
                    
                    endStateCounter = 0f;
                    chosenAbility = null;
                }
                break;

            case BattleStates.LOSE:
                if (!isAlreadyCalculated)
                {
                    playerLoseText.text = "Player is dead. You gained neither expierence nor money. \nYour EXP: " + player.PlayerExp + " -> " + (player.PlayerExp/2); 
                    isAlreadyCalculated = true;

                }
                endStateCounter += Time.deltaTime;
                if(endStateCounter >= 4f)
                {
                    playerLoseText.text = ""; // lösche den Text
                    player.loseBattle();      // setze die Stamina/Energy auf den maximalen Wert und halbiere die PlayerExp
                    currentState = BattleStates.END;
                    endStateCounter = 0f;
                    chosenAbility = null;
                }

                break;

            case BattleStates.END:

                currentState = BattleStates.INACTIVE;
                break;

            default: Debug.Log("default state");  break;

        }

       
	}

    void OnGUI()
    {

        GUI.Label(new Rect(10, 10, 150, 20), currentState.ToString());
        if (chosenAbility != null)
        {
            GUI.Label(new Rect(10, 30, 150, 20), chosenAbility.TypeName.ToString());
        }

    }

    public Queue<Monster_Behaviour> getEnemyQueue(){

        return enemyQueue;
    }

    public void clearEnemyQueue()
    {

        enemyQueue.Clear();
    }

}
