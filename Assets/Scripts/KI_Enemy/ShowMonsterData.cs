using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowMonsterData : MonoBehaviour {

	private Monster_Behaviour monster;
	private Text ui;
    private Text log;

	// Use this for initialization
	void Start () {
		monster = gameObject.GetComponent<Monster_Behaviour>();
		ui = GameObject.FindGameObjectWithTag("uiText").GetComponent<Text>();
        log = GameObject.FindGameObjectWithTag("uiText2").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseOver()
	{
		// Monster-Rasse
		string uiText = monster.gameObject.name;
		uiText += "\nElement: ";
        uiText += monster.getMonsterData().getElementType().ToString();

        if (!monster.inCombat)
        {
            uiText += "\nTired: ";
            uiText += monster.getCurrDiscValueAtIndex(0).ToString();

            uiText += "\nHunger: ";
            uiText += monster.getCurrDiscValueAtIndex(1).ToString();

            uiText += "\nLove: ";
            uiText += monster.getCurrDiscValueAtIndex(2).ToString();

            uiText += "\nClock: ";
            uiText += monster.getClock().ToString();

            if (monster.getMonsterData().hasPartner())
            {
                uiText += "\nMonster has a partner.";
            }

            if (monster.getMonsterData().hasChild())
            {

                uiText += "\nMonster has a child.";
            }
        }
		uiText += "\n\nStamina: ";
		uiText += monster.getMonsterData().getAttributeValueAtIndex(0).ToString();
		
		uiText += "\nStrength: ";
		uiText += monster.getMonsterData().getAttributeValueAtIndex(1).ToString();
		
		uiText += "\nEndurance: ";
		uiText += monster.getMonsterData().getAttributeValueAtIndex(2).ToString();
        if (!monster.inCombat)
        {
            uiText += "\nHumor: ";
            uiText += monster.getMonsterData().getAttributeValueAtIndex(3).ToString();

            uiText += "\nNaivity: ";
            uiText += monster.getMonsterData().getAttributeValueAtIndex(4).ToString();

            uiText += "\nAnxiety: ";
            uiText += monster.getMonsterData().getAttributeValueAtIndex(5).ToString();

            uiText += "\nIndependency: ";
            uiText += monster.getMonsterData().getAttributeValueAtIndex(6).ToString();

            uiText += "\nVanity: ";
            uiText += monster.getMonsterData().getAttributeValueAtIndex(7).ToString();
        }
		ui.text = uiText;
        string logText = "Monster Log: ";
        logText += monster.MonsterLog;
        log.text = logText;
	}

	void OnMouseExit()
	{
		ui.text = "";
	}

    public void doNotShowData()
    {
        ui.text = "";
        log.text = "";
    }
	 
}
