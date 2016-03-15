using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreateNewPlayer : MonoBehaviour {

    private PlayerInformation newPlayer;
    private string newPlayerName = "Your Name";
    private bool isFemale;                         // Toggle für Female selection
    private bool isMale = true;                    // Toggle für Male selection
    private bool isGenderSet;                      // bool ob gender ausgewählt wurde
    private bool chooseSword = true;               // Toggle für Sword Weapon selection
    private bool chooseBow;                        // Toggle für Bow Weapon selection
    private bool chooseAxe;                        // Toggle für Axe Weapon selection
    private Text playerUI;                         // UI für die Player Information
    public Sprite[] playerSprites;                 // Sprites für die Spielercharaktere
    private SpriteRenderer currSprite;


    // Use this for initialization
    void Start () {

       // newPlayer = new Player();
        isGenderSet = false;
        playerUI = GameObject.FindGameObjectWithTag("playerUI").GetComponent<Text>();
        newPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInformation>();
        currSprite = GameObject.FindGameObjectWithTag("playerSprite").GetComponent<SpriteRenderer>();
        currSprite.GetComponent<SpriteRenderer>().sprite = playerSprites[0];

    }

    // Update is called once per frame
    void Update() {

        // zeige das richtige Charakter-Sprite an

        if (isMale)
            currSprite.GetComponent<SpriteRenderer>().sprite = playerSprites[0];
        else if(isFemale)
            currSprite.GetComponent<SpriteRenderer>().sprite = playerSprites[1];

        /* zeige die Attributwerte des Players*/
        string uiText = newPlayerName;
        uiText += "\n";

        if (isFemale || isMale)
        {
            string gender = "";

            if (isFemale) gender = "Female";
            else if (isMale) gender = "Male";
           
            uiText += "Gender: ";
            uiText += gender;


        }

        uiText += "\nLevel: ";
        uiText += newPlayer.PlayerLevel.ToString();

        uiText += "\nStamina: ";
        uiText += newPlayer.Stamina.ToString();

        uiText += "\nEndurance: ";
        uiText += newPlayer.Endurance.ToString();

        uiText += "\nIntellect: ";
        uiText += newPlayer.Intellect.ToString();

        uiText += "\nStrength: ";
        uiText += newPlayer.Strength.ToString();
        /* falls eine Waffe angeklickt ist, wird dessen AttackPower dazu gerechnet*/
        if (chooseBow)
        {
            uiText += " + ";
            uiText += 5;
        }
        else if (chooseSword)
        {

            uiText += " + ";
            uiText += 10;
        }
        else if (chooseAxe) {
            uiText += " + ";
            uiText += 15;
        }

        if (chooseBow || chooseSword || chooseAxe)
        {

            uiText += "\nWeapon's Attack Speed: ";
            if (chooseBow && !chooseSword && !chooseAxe)
            {
                uiText += 2.5;
                uiText += "\nWeapon: Basic Fire Bow";
            }
            else if (chooseSword && !chooseBow && !chooseAxe)
            {
                uiText += 5.0;
                uiText += "\nWeapon: Basic Ice Sword";
            }
            else if (chooseAxe && !chooseBow && !chooseSword) { 
                uiText += 7.5;
                uiText += "\nWeapon: Basic Water Axe";

            }
        }

       playerUI.text = uiText;
    }

    void OnGUI() {

        newPlayerName = GUI.TextArea(new Rect(20, 100, 150, 20), newPlayerName, 16); // begrenze die Anzahl der Zeichen auf 15 

        /* Toggles  */
        isFemale = GUI.Toggle(new Rect(20, 140, 150, 20), isFemale, "Female");
        if (isFemale) isMale = false; // nur ein Geschlecht darf ausgewählt sein
        isMale = GUI.Toggle(new Rect(180, 140, 150, 20),isMale, "Male");
        if (isMale) isFemale = false;  // nur ein Geschlecht darf ausgewählt sein

        chooseSword = GUI.Toggle(new Rect(20, 180, 150, 20), chooseSword, "Ice Sword Weapon");
        if (chooseSword)
        {
            chooseBow = false;
            chooseAxe = false;
        }
        chooseBow = GUI.Toggle(new Rect(180, 180, 150, 20),chooseBow, "Fire Bow Weapon");
        if (chooseBow)
        {
            chooseSword = false;
            chooseAxe = false;
        }
        chooseAxe = GUI.Toggle(new Rect(330, 180, 150, 20), chooseAxe, "Water Axe Weapon");
        if (chooseAxe)
        {
            chooseBow = false;
            chooseSword = false;
        }


        /* Buttons */

        // Create Player Button
        if (GUI.Button(new Rect(500, 500, 150, 20), "Create Player")) {


            /* überprüfe, welche Toggle selektiert sind*/
            if (chooseBow)
            {
                newPlayer.UsedWeapon = new BowWeapon();
                newPlayer.ownedWeapons.Add(newPlayer.UsedWeapon);     
            }
            else if (chooseSword)
            {

                newPlayer.UsedWeapon = new SwordWeapon();
                newPlayer.ownedWeapons.Add(newPlayer.UsedWeapon);

            }
            else if (chooseAxe)
            {

                newPlayer.UsedWeapon = new AxeWeapon();
                newPlayer.ownedWeapons.Add(newPlayer.UsedWeapon);

            }

            if (isMale)
            {
                newPlayer.IsMale = true;
                isGenderSet = true;
            }
            else if (isFemale)
            {

                newPlayer.IsMale = false;
                isGenderSet = true;
            }
           

        }

        // sobald eine gültige Auswahl getroffen ist, lade nächste Scene
        if(isGenderSet && newPlayer.UsedWeapon != null)
        {
            newPlayer.PlayerName = newPlayerName;
            
            if (GUI.Button(new Rect(700, 500, 150, 20), "Load Next Scene")) {

                // erhöhe die Strength mit der WeaponPower

                newPlayer.Strength += newPlayer.UsedWeapon.AttackPower;

                // to do: save game information

                UnityEngine.SceneManagement.SceneManager.LoadScene("scene");
                
            }
        }
    
    }


}
