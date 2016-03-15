using UnityEngine;
using System.Collections;

public class BattleStartScript{

    public BattleStartScript() {

        // Battle Start Screen

        // bewege die Camera und zoome näher heran
        Camera.main.GetComponent<CameraMovement>().inBattle = true;

    }

    public int decideWhoGoesFirst() { //return 0 für player, 1 für monster

        int r = -1;
        int random = Random.Range(0, 100);
        if (random % 2 == 0)
        {
            r = 0;
        }
        else {
            r = 1;
        }
            return r;

    }
}
