using UnityEngine;
using System.Collections;

public class PlayerChoiceScript : MonoBehaviour{

    private bool isChoiceDone = false;  // false wenn der Player eine Ability auswählen kann, true wenn er ein Button geklickt hat 

    void Start() {

       
    }

    void Update() {

    }

    // getter und setter

    public bool IsChoiceDone{
    
        get{ return isChoiceDone; }
        set { isChoiceDone = value; }
    }


    // Button-Methoden
    public void chooseAttackButton() {

        Debug.Log("Attack Button Hit");
        this.GetComponentInParent<TurnBasedCombatStateMachine>().chosenAbility = new Ability(Ability.AbilityTypes.ATTACK);
        isChoiceDone = true;
    }

    public void chooseHealButton() {
        Debug.Log("Heal Button Hit");
        this.GetComponentInParent<TurnBasedCombatStateMachine>().chosenAbility = new Ability(Ability.AbilityTypes.HEAL);
        isChoiceDone = true;

    }

    public void chooseDefendButton() {
        Debug.Log("Defend Button Hit");
        this.GetComponentInParent<TurnBasedCombatStateMachine>().chosenAbility = new Ability(Ability.AbilityTypes.DEFEND);
        isChoiceDone = true;

    }

    public void chooseFleeButton() {
        Debug.Log("Flee Button Hit");
        this.GetComponentInParent<TurnBasedCombatStateMachine>().chosenAbility = new Ability(Ability.AbilityTypes.FLEE);
        isChoiceDone = true;

    }
}
