using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowPlayerData : MonoBehaviour
{

    private Text ui;

    // Use this for initialization
    void Start()
    {
        
        ui = GameObject.FindGameObjectWithTag("playerUI").GetComponent<Text>();
    }


    // Update is called once per frame
    void Update()
    {
        ui.transform.position = Camera.main.WorldToScreenPoint(GameMachine.gameMachine.PlayerSpritePosition + Vector3.left*20 + Vector3.down*25);
        showData();
    }


    void showData()
    {
        PlayerInformation player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInformation>();
        string text = "PlayerName: ";
        text += player.PlayerName;

        text += "\nLevel: ";
        text += player.PlayerLevel;

        text += "\nStamina: ";
        text += player.Stamina;

        text += "\nEnergy: ";
        text += player.Energy;

        text += "\n";

        text += "\nStrength: ";
        text += player.Strength;

        text += "\nEndurance: ";
        text += player.Endurance;

        text += "\nIntellect: ";
        text += player.Intellect;

       

        if (player.UsedWeapon != null) { 
          text += "\nCurrent Weapon: ";
          text += player.UsedWeapon.WeaponName;

          text += "\nWeapon's Level: ";
          text += player.UsedWeapon.WeaponLevel;

          text += "\nWeapon's Element: ";
          text += player.UsedWeapon.ElementType;
        }
        ui.text = text;
    }

    public void deleteText()
    {

        ui.text = "";
    }

  

}
