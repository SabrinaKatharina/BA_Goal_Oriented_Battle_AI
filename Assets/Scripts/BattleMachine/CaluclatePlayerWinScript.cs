using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CaluclatePlayerWinScript{

    private PlayerInformation player;

    private Text playerWin;

    private bool hasLeveledUp;

    public CaluclatePlayerWinScript(int expValue, int moneyValue)
    {
        if (player == null)
        {

            player = GameObject.Find("Player").GetComponent<PlayerInformation>();
        }
        if (playerWin == null)
        {

            playerWin = GameObject.Find("PlayerWinText").GetComponent<Text>();
        }

        string text = "Calculate Player Win:\n";

        text += "Player Exp: " + player.PlayerExp + " + " + expValue;
        text += "\nPlayer Money: " + player.MoneyAmount + " + " + moneyValue;

        // füge die erworbenen Werte hinzu

        player.PlayerExp += expValue;
        player.MoneyAmount += moneyValue;

        // fülle die Stamina und die Energy für das nächste Battle auf

        player.Stamina = player.MaxStaminaValue;
        player.Energy = player.MaxEnergyValue;

        // prüfe , ob der Player aufgelevelt ist:
        if (player.PlayerExp >= player.NextLevelUpAmount)
            hasLeveledUp = true;

            while (player.PlayerExp >= player.NextLevelUpAmount)
        {
            // ziehe die levelUpAmount von der Exp ab
            player.PlayerExp -= player.NextLevelUpAmount;
            player.levelUp();

        }

        if (hasLeveledUp)
        {

            text += "\n";
            text += "\nLevel up!\n PlayerLevel: " + player.PlayerLevel;
            text += "\n Exp to next LevelUp: " + player.NextLevelUpAmount;

            text += "\n New Stats:";
            text += "\n  Stamina: " + player.Stamina;

            text += "\n  Energy: " + player.Energy;
            text += "\n  Strength: " + player.Strength;
            text += "\n  Endurance: " + player.Endurance;
            text += "\n  Intellect: " + player.Intellect;

        }
        // die angelegte Waffe bekommt auch Exp

        player.UsedWeapon.WeaponExp += (int)(expValue / 2);

        if (player.UsedWeapon.WeaponExp >= player.UsedWeapon.NextLevelUpCost)
        {

            text += "\nWeapon also levels up!";
            player.UsedWeapon.WeaponExp -= player.UsedWeapon.NextLevelUpCost;
            player.UsedWeapon.upgrade();
        }

        playerWin.text = text;
        Debug.Log(text);
        Camera.main.GetComponent<CameraMovement>().inBattle = false;
        GameMachine.gameMachine.PlayerPosition += new Vector3(-200f, 0f, 0f);


    }

    public void deleteWinText()
    {

        playerWin.text = "";
    }
}
