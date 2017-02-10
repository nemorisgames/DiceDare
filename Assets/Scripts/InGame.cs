using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Analytics;

public class InGame : MonoBehaviour {
	Dice dice;
	Transform cells;
	public bool rotating = false;
	public GameObject finishedSign;
	public GameObject TimesUpSign;
	public UILabel clock;
	public UILabel record;
	float recordSeconds;
	public int secondsAvailable = 65;
	AudioSource audio;
	public AudioClip audioBadMove;
	public AudioClip audioGoodMove;
	public AudioClip audioFinish;
	// Use this for initialization
	void Start () {
		dice = GameObject.FindGameObjectWithTag ("Dice").GetComponent<Dice> ();
		cells = GameObject.Find ("Cells").transform;
		recordSeconds = PlayerPrefs.GetFloat ("record"+SceneManager.GetActiveScene ().name, -1f);
		if (recordSeconds > 0) {
			int minutes = (int)((secondsAvailable - recordSeconds) / 60);
			int seconds = (int)((secondsAvailable - recordSeconds) % 60);
			int dec = (int)(((secondsAvailable - recordSeconds) % 60 * 10f) - ((int)((secondsAvailable - recordSeconds) % 60) * 10));
			record.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}
		audio = GetComponent<AudioSource> ();
	}

	public void calculateResult(int diceValueA, int diceValueB, int cellValue){
		print ("calculating");
		if (checkOperationResult (diceValueA, diceValueB) != cellValue) {
			#if !UNITY_EDITOR
			Analytics.CustomEvent ("badMove", new Dictionary<string, object> {
				{ "scene", SceneManager.GetActiveScene ().name },
				{ "steps", dice.steps },
				{ "place", dice.gameObject.transform.position},
				{ "time", secondsAvailable - Time.timeSinceLevelLoad }
			});
			#endif
			StartCoroutine (reloadScene ());
			audio.pitch = 1f;
			audio.PlayOneShot(audioBadMove);
		} else {
			audio.pitch = Random.Range (0.95f, 1.05f);
			audio.PlayOneShot(audioGoodMove);
		}
	}

	IEnumerator reloadScene(){
		yield return new WaitForSeconds (2f);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void finishGame(){
		print("Finished");
		dice.enabled = false;
		finishedSign.SetActive (true);

		audio.pitch = 1f;
		audio.PlayOneShot(audioFinish);
		#if !UNITY_EDITOR
			if(secondsAvailable - Time.timeSinceLevelLoad > PlayerPrefs.GetFloat ("record", 0f))
			PlayerPrefs.SetFloat ("record"+SceneManager.GetActiveScene ().name, secondsAvailable - Time.timeSinceLevelLoad);
		Analytics.CustomEvent ("finish", new Dictionary<string, object> {
			{ "scene", SceneManager.GetActiveScene ().name },
			{ "steps", dice.steps },
			{ "time", secondsAvailable - Time.timeSinceLevelLoad }
		});
			#endif
	}

	public int checkOperationResult(int diceValueA, int diceValueB){
		int res = 0;
		print(dice.currentOperation);
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

	public IEnumerator rotateCells(bool CW){
		rotating = true;
		yield return new WaitForSeconds (0.5f);
		int nSteps = 30;
		for (int i = 1; i <= nSteps; i++) {
			yield return new WaitForSeconds (0.01f);
			cells.RotateAround (dice.transform.position, Vector3.up, (CW ? 90f : -90f) / nSteps);
		}
		rotating = false;
	}

	void timesUp(){
		clock.text = "00:00.0";
		TimesUpSign.SetActive (true);
		#if !UNITY_EDITOR
		Analytics.CustomEvent ("timesUp", new Dictionary<string, object> {
			{ "scene", SceneManager.GetActiveScene ().name },
			{ "steps", dice.steps },
			{ "time", secondsAvailable - Time.timeSinceLevelLoad }
		});
		#endif
	}

	public void playAgain(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void exit(){
		SceneManager.LoadScene ("LevelSelection");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene ("LevelSelection");
		if (finishedSign.activeSelf || TimesUpSign.activeSelf) {
			return;
		}
		if (secondsAvailable - Time.timeSinceLevelLoad <= 0) {
			timesUp ();
		}
		else {
			int minutes = (int)((secondsAvailable - Time.timeSinceLevelLoad) / 60);
			int seconds = (int)((secondsAvailable - Time.timeSinceLevelLoad) % 60);
			int dec = (int)(((secondsAvailable - Time.timeSinceLevelLoad) % 60 * 10f) - ((int)((secondsAvailable - Time.timeSinceLevelLoad) % 60) * 10));
			clock.text = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;
		}
	}
}
