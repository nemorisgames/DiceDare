using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGame : MonoBehaviour {
	Dice dice;
	public GameObject finishedSign;
	public UnityEngine.UI.Text clock;
	public UnityEngine.UI.Text record;
	// Use this for initialization
	void Start () {
		dice = GameObject.FindGameObjectWithTag ("Dice").GetComponent<Dice> ();

	}

	public void calculateResult(int diceValueA, int diceValueB, int cellValue){
		print ("calculating");
		if (checkOperationResult(diceValueA, diceValueB) != cellValue) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		}
	}

	public void finishGame(){
		print("Finished");
		dice.enabled = false;
		finishedSign.SetActive (true);
	}

	public int checkOperationResult(int diceValueA, int diceValueB){
		int res = 0;
		switch (dice.currentOperation) {
		case Dice.Operation.Sum:
			res = ((diceValueA + diceValueB));
			break;
		case Dice.Operation.Rest:
			res = ((diceValueA - diceValueB));
			break;
		case Dice.Operation.Mult:
			res = ((diceValueA * diceValueB));
			break;
		case Dice.Operation.Div:
			res = ((diceValueA / diceValueB));
			break;
		}
		return res;
	}
	
	// Update is called once per frame
	void Update () {
		clock.text = (int)(Time.timeSinceLevelLoad / 60) + ":" + (int)(Time.timeSinceLevelLoad % 60);
	}
}
