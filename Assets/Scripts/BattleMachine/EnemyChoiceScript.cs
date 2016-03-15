using UnityEngine;
using System.Collections;

public class EnemyChoiceScript {

    private bool isLastInQueue;  // true, sobald das Monster das letzte übrig gebliebene Monster in der EnemyQueue ist
    private Ability chosenAbility; // speichere, welche Ability ausgewählt wurde
    private Monster_Behaviour monster;  // das kämpfende Monster

    public Ability ChosenAbility { get { return chosenAbility; } }

    public EnemyChoiceScript(Monster_Behaviour currMonster, int currEnemyQueueCount) {

        monster = currMonster;
        // speichere, ob das Monster das letze Monster in der Enemy-Queue ist; falls ja, ist die Dequeue Ability nicht verfügbar!
        if(currEnemyQueueCount == 1)
        {
            isLastInQueue = true;
            Debug.Log("Enemy is last in queue and cannot esacpe.");
        }
        else
        {
            isLastInQueue = false;
        }

        monster.GetComponentInParent<Goal_Manager>().battleThinkCycle(isLastInQueue);
        chosenAbility = monster.GetComponentInParent<Goal_Manager>().ChosenBattleAbility;  // wähle die beste Ability in diesem Zug aus
        Debug.Log("EnemyChoiceScript over: " + chosenAbility.TypeName + " , " + chosenAbility.Value);

    }

}
