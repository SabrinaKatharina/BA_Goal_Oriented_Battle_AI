using UnityEngine;
using System.Collections;

public class Discontentment{

	public int [] goals; //Array der Einzelbedürfnisse

	/* goals[0] = tired;
	 * goals[1] = hunger;
	 * goals[2] = love;
	*/

	public const int GOAL_MIN = 0;
	public const int GOAL_MAX = 100;

	public Discontentment(){
		goals = new int[3];
		for (int i = 0; i < goals.Length; ++i) {
			goals[i] = GOAL_MIN;
		}
	}

	public Discontentment(int value){
		goals = new int[3];
		for (int i = 0; i < goals.Length; ++i) {
			goals[i] = value;
		}
	}

	public Discontentment(Discontentment disc){
		goals = new int[3];
		for (int i = 0; i < goals.Length; ++i) {
			goals[i] = disc.getValueAtIndex(i);
		}
	}

	public int getValueAtIndex(int index){

		return goals [index];
	}

	public void setValueAtIndex (int index, int value ){


			goals [index] = value;

	}

	public void addValueAtIndex(int index, int value){

		goals [index] += value;
		if(goals[index] >= GOAL_MAX){
			
			goals[index] = GOAL_MAX;
		}
		else if(goals[index] <= GOAL_MIN){
			goals[index] = GOAL_MIN;
		}
	}

	public void addDiscontentment(Discontentment deltaDisc){

		for (int i = 0; i < goals.Length; ++i) {
			goals [i] += deltaDisc.getValueAtIndex (i);

			// Clampen, falls ein Wert den Bereich überschreitet
			if(goals[i] >= GOAL_MAX){

				goals[i] = GOAL_MAX;
			}
			else if(goals[i] <= GOAL_MIN){
				goals[i] = GOAL_MIN;
			}
		}
	}

	public double getTotalDiscontentment(double durationOfAction){

		double r = 0; // return value

		double deltaGoals = 0; // Veränderungswert, mit welchem Wert sich pro Stunde der Wert des Goals inkrementiert
		// tired  = + 0/h
		// hunger = +10/h
		// love   = +20/h

		for (int i = 0; i < goals.Length; ++i) {

			r += (goals[i]+ (deltaGoals*durationOfAction)) * (goals[i]+(deltaGoals*durationOfAction)); // damit hohe Werte mehr gewichtet werden
			deltaGoals+=10.0;
		}
		return r;
	}


}
