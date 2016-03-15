using UnityEngine;
using System.Collections;

public class Action{

	private string name; // Name der Aktion
	private double duration; //Dauer der Aktion
	private Discontentment deltaDiscontentment; //Veränderungswert


	public Action(string name, double duration, Discontentment deltaDisc ){

		this.name = name;
		this.duration = duration;
		this.deltaDiscontentment = deltaDisc;
	}

	public string getName(){
		return name;
	}

	public double getDuration(){
		return duration;
	}

	public Discontentment getDeltaDisc(){
		return deltaDiscontentment;
	}
}
