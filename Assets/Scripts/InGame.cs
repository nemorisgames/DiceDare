using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGame : MonoBehaviour {
	Dice dice;
	Transform cells;
	bool rotating = false;
	public GameObject finishedSign;
	public UnityEngine.UI.Text clock;
	public UnityEngine.UI.Text record;
	public int secondsAvailable = 65;
	// Use this for initialization
	void Start () {
		dice = GameObject.FindGameObjectWithTag ("Dice").GetComponent<Dice> ();
		cells = GameObject.Find ("Cells").transform;
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

	IEnumerator rotateCells(bool CW){
		rotating = true;
		int nSteps = 30;
		for (int i = 1; i <= nSteps; i++) {
			yield return new WaitForSeconds (0.01f);
			cells.RotateAround (dice.transform.position, Vector3.up, (CW ? 90f : -90f) / nSteps);
		}
		rotating = false;
	}
	
	// Update is called once per frame
	void Update () {
		int minutes = (int)((secondsAvailable - Time.timeSinceLevelLoad) / 60);
		int seconds = (int)((secondsAvailable - Time.timeSinceLevelLoad) % 60);
		int dec = (int)(((secondsAvailable - Time.timeSinceLevelLoad) % 60 * 10f) - ((int)((secondsAvailable - Time.timeSinceLevelLoad) % 60) * 10));
		clock.text = (minutes < 10?"0":"") + minutes + ":" + (seconds < 10?"0":"") + seconds + "." + dec;

		if (Input.GetKeyDown (KeyCode.Q)) {
			StartCoroutine (rotateCells (false));
			//Camera.main.transform.SendMessage("rotateCamera", false);
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			StartCoroutine (rotateCells (true));
			//Camera.main.transform.SendMessage("rotateCamera", true);
		}
	}
}
