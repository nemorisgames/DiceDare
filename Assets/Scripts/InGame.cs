using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Analytics;

public class InGame : MonoBehaviour {
	Dice dice;
	Transform cells;
	GameObject [,] cellArray;
	public TextMesh [] cellsText;
	ArrayList texts = new ArrayList();
	ArrayList path = new ArrayList();
	public bool rotating = false;
	public GameObject finishedSign;
	public GameObject TimesUpSign;
	public UILabel clock;
	public UILabel record;
	float recordSeconds;
	[HideInInspector]
	public float pauseTime = 0;
	float pauseAux = 0;
	public int secondsAvailable = 65;
	public UITexture tutorial;
	public Texture2D[] imgTutorial;
	AudioSource audio;
	public AudioClip audioBadMove;
	public AudioClip audioGoodMove;
	public AudioClip audioFinish;

	public GameObject cellNormal;
	public GameObject cellBegin;
	public GameObject cellEnd;

	public GameObject cellSum;
	public GameObject cellSubstraction;
	public GameObject cellMultiplication;
	public GameObject cellDivision;
	public GameObject cellCW;
	public GameObject cellCCW;

	[HideInInspector]
	public bool pause = false;

	int timesDied = 0;
	public GameObject hintButton;
	int hintsAvailable = 3;

	// Use this for initialization
	void Start () {
		timesDied = PlayerPrefs.GetInt ("timesDied", 0);
		dice = GameObject.FindGameObjectWithTag ("Dice").GetComponent<Dice> ();
		componerEscena ();

		cells = GameObject.Find ("Cells").transform;
		cellsText = cells.GetComponentsInChildren<TextMesh> ();
		foreach (TextMesh t in cellsText) {
			texts.Add(t.GetComponent<Transform>());
		}
		recordSeconds = PlayerPrefs.GetFloat ("record"+PlayerPrefs.GetString ("scene", "Scene1"), -1f);
		if (recordSeconds > 0) {
			int minutes = (int)((recordSeconds) / 60);
			int seconds = (int)((recordSeconds) % 60);
			int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
			record.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}
		audio = GetComponent<AudioSource> ();
		print ("timesDied " + timesDied);
		if (timesDied >= 5)
			StartCoroutine(lightPath (2));
		//StartCoroutine (cellArray[1,2].GetComponent<Cell>().shine ());
		//StartCoroutine (lightPath (2));
	}

	public void hint(){
		StartCoroutine (lightPath (1));
		hintsAvailable--;
		if (hintsAvailable <= 0) {
			hintButton.SetActive (false);
		}
	}

	public void componerEscena(){
		string completo = "";
		switch (PlayerPrefs.GetString ("scene", "Scene1")) {
		case "Scene1":completo = GlobalVariables.Scene1;break;
		case "Scene2":completo = GlobalVariables.Scene2;break;
		case "Scene3":completo = GlobalVariables.Scene3;break;
		case "Scene4":completo = GlobalVariables.Scene4;break;
		case "Scene5":completo = GlobalVariables.Scene5;break;
		}
		string[] aux = completo.Split(new char[1]{'$'});
		string[] info = aux[0].Split(new char[1]{'|'});
		string[] arreglo = aux[1].Split(new char[1]{'|'});
		Vector3 posIni = new Vector3 (int.Parse (info [2]), 0f, -int.Parse (info [3]));
		if (int.Parse (info [4]) > 0) { 
			tutorial.mainTexture = imgTutorial [int.Parse (info [4]) - 1];
			tutorial.transform.FindChild ("Sprite").GetComponent<UISprite> ().alpha = 1f;
			tutorial.transform.SendMessage ("PlayForward");
			tutorial.transform.FindChild("Sprite").SendMessage ("PlayForward");
			Pause ();
		}
		string completoNumbers = "";
		switch (PlayerPrefs.GetString ("scene", "Scene1")) {
		case "Scene1":completoNumbers = GlobalVariables.Scene1Numbers;break;
		case "Scene2":completoNumbers = GlobalVariables.Scene2Numbers;break;
		case "Scene3":completoNumbers = GlobalVariables.Scene3Numbers;break;
		case "Scene4":completoNumbers = GlobalVariables.Scene4Numbers;break;
		case "Scene5":completoNumbers = GlobalVariables.Scene5Numbers;break;
		}
		string completoPath = "";
		switch (PlayerPrefs.GetString ("scene", "Scene1")) {
		case "Scene1":completoPath = GlobalVariables.Scene1Path;break;
		case "Scene2":completoPath = GlobalVariables.Scene2Path;break;
		case "Scene3":completoPath = GlobalVariables.Scene3Path;break;
		//case "Scene4":completoPath = GlobalVariables.Scene4Path;break;
		//case "Scene5":completoPath = GlobalVariables.Scene5Path;break;*/
		default:completoPath = GlobalVariables.Scene1Path;break;
		}
		string [] auxPath = completoPath.Split (new char[1]{ '|' });
		string[] auxCoord = new string[2];
		for (int i = 0; i < auxPath.Length; i++) {
			auxCoord = auxPath [i].Split (new char[1]{ ',' });
			Vector2 coord = new Vector2 (float.Parse (auxCoord [0]), float.Parse (auxCoord [1]));
			path.Add (coord);
		};

		string[] auxNumbers = completoNumbers.Split(new char[1]{'$'});
		string[] infoNumbers = auxNumbers[0].Split(new char[1]{'|'});
		string[] arregloNumbers = auxNumbers[1].Split(new char[1]{'|'});
		dice.transform.FindChild ("TextUp").GetComponent<TextMesh> ().text = "" + int.Parse (infoNumbers[0]);
		dice.transform.FindChild ("TextLeft").GetComponent<TextMesh> ().text = "" + int.Parse (infoNumbers[1]);
		dice.transform.FindChild ("TextForward").GetComponent<TextMesh> ().text = "" + int.Parse (infoNumbers[2]);
		int indice = 0;
		Transform rootCells = GameObject.Find ("Cells").transform;
		cellArray = new GameObject[int.Parse(info[0]),int.Parse(info[1])];
		for(int i = 0; i < int.Parse(info[0]); i++){
			for (int j = 0; j < int.Parse (info [1]); j++) {
				GameObject g = null;
				switch (int.Parse (arreglo [indice])) {
				case -2:
					g = (GameObject)Instantiate (cellEnd, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				case -1:
					g = (GameObject)Instantiate (cellBegin, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				case 1:
				case 2:
					g = (GameObject)Instantiate (cellNormal, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				case 3:
					g = (GameObject)Instantiate (cellSum, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				case 4:
					g = (GameObject)Instantiate (cellSubstraction, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				case 5:
					g = (GameObject)Instantiate (cellMultiplication, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				case 6:
					g = (GameObject)Instantiate (cellDivision, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				case 7:
					g = (GameObject)Instantiate (cellCW, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				case 8:
					g = (GameObject)Instantiate (cellCCW, new Vector3 (j, 0.05f, -i) - posIni, Quaternion.identity);
					break;
				}
				if (g != null) {
					g.GetComponent<Cell> ().number = int.Parse (arregloNumbers [indice]);
					g.transform.parent = rootCells;
					cellArray [i,j] = g;
				}
				indice++;
			}
		}
	}

	public void Pause(){
		pauseAux = Time.timeSinceLevelLoad;
		pause = true;
	}

	public void UnPause(){
		pause = false;
		pauseTime += Time.timeSinceLevelLoad - pauseAux;
		pauseAux = 0;
	}

	public void calculateResult(int diceValueA, int diceValueB, int cellValue){
		print ("calculating");
		if (checkOperationResult (diceValueA, diceValueB) != cellValue) {
			#if !UNITY_EDITOR
			Analytics.CustomEvent ("badMove", new Dictionary<string, object> {
			{ "scene", PlayerPrefs.GetString("scene", "Scene1") },
				{ "steps", dice.steps },
				{ "place", dice.gameObject.transform.position},
				{ "time", secondsAvailable - Time.timeSinceLevelLoad }
			});
			#endif
			dice.enabled = false;
			StartCoroutine (reloadScene ());
			audio.pitch = 1f;
			audio.PlayOneShot(audioBadMove);
			dice.GetComponent<Animator> ().SetTrigger ("BadMove");
		} else {
			audio.pitch = Random.Range (0.95f, 1.05f);
			audio.PlayOneShot(audioGoodMove);
			path.RemoveAt (0);
		}
	}

	IEnumerator reloadScene(){
		yield return new WaitForSeconds (1f);
		timesDied++;
		PlayerPrefs.SetInt ("timesDied", timesDied);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	//0: adyacente, 1: tres adyacentes, 2: todos
	public IEnumerator lightPath(int mode){
		if (!pause) {
			Pause ();
			mode = Mathf.Clamp (mode, 0, 2);
			switch (mode) {
			case 0:
				if (path.Count > 0)
					StartCoroutine (cellArray [(int)((Vector2)path [0]).x, (int)((Vector2)path [0]).y].GetComponent<Cell> ().shine (2));
				yield return new WaitForSeconds (1f);
				break;
			case 1:
				int aux = Mathf.Min (3, path.Count);
				for (int i = 0; i < aux; i++) {
					StartCoroutine (cellArray [(int)((Vector2)path [i]).x, (int)((Vector2)path [i]).y].GetComponent<Cell> ().shine (1));
					yield return new WaitForSeconds (1f/2);
				}
				break;
			case 2:
				foreach (Vector2 v in path) {
					StartCoroutine (cellArray [(int)v.x, (int)v.y].GetComponent<Cell> ().shine (1));
					yield return new WaitForSeconds (1f/2);
				}
				break;
			}
			UnPause ();
		}
	}

	public void finishGame(){
		print("Finished");
		dice.enabled = false;
		finishedSign.SetActive (true);
		dice.enabled = false;
		dice.transform.rotation = Quaternion.identity;
		dice.GetComponent<Animator> ().SetTrigger ("Finished");
		audio.pitch = 1f;
		audio.PlayOneShot(audioFinish);
		if(Time.timeSinceLevelLoad - pauseTime < PlayerPrefs.GetFloat ("record"+PlayerPrefs.GetString ("scene", "Scene1"), float.MaxValue))
			PlayerPrefs.SetFloat ("record"+PlayerPrefs.GetString ("scene", "Scene1"), Time.timeSinceLevelLoad - pauseTime);
		
		#if !UNITY_EDITOR
		Analytics.CustomEvent ("finish", new Dictionary<string, object> {
		{ "scene", PlayerPrefs.GetString("scene", "Scene1") },
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
			foreach (Transform t in texts) {
				t.RotateAround(t.position, Vector3.up, (CW ? -90f : 90f)/ nSteps);
			}
		}
		rotating = false;
	}

	void timesUp(){
		clock.text = "00:00.0";
		TimesUpSign.SetActive (true);
		dice.enabled = false;
		StartCoroutine (reloadScene ());
		audio.pitch = 1f;
		audio.PlayOneShot(audioBadMove);
		dice.GetComponent<Animator> ().SetTrigger ("BadMove");
		#if !UNITY_EDITOR
		Analytics.CustomEvent ("timesUp", new Dictionary<string, object> {
		{ "scene", PlayerPrefs.GetString("scene", "Scene1") },
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
		/*if (secondsAvailable - Time.timeSinceLevelLoad <= 0) {
			timesUp ();
		}
		else {
			int minutes = (int)((secondsAvailable - Time.timeSinceLevelLoad) / 60);
			int seconds = (int)((secondsAvailable - Time.timeSinceLevelLoad) % 60);
			int dec = (int)(((secondsAvailable - Time.timeSinceLevelLoad) % 60 * 10f) - ((int)((secondsAvailable - Time.timeSinceLevelLoad) % 60) * 10));
			clock.text = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;
		}*/
		if (!pause) {
			int minutes = (int)((Time.timeSinceLevelLoad - pauseTime) / 60);
			int seconds = (int)((Time.timeSinceLevelLoad - pauseTime) % 60);
			int dec = (int)(((Time.timeSinceLevelLoad - pauseTime) % 60 * 10f) - ((int)((Time.timeSinceLevelLoad - pauseTime) % 60) * 10));
			clock.text = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;
		}

		//test
		if(Input.GetKeyDown(KeyCode.Q)){
			StartCoroutine (lightPath (1));
		}

		if(Input.GetKeyDown(KeyCode.E)){
			StartCoroutine (lightPath (0));
		}

		if(Input.GetKeyDown(KeyCode.R)){
			StartCoroutine (lightPath (2));
		}

		if(Input.GetKeyDown(KeyCode.P)){
			PlayerPrefs.DeleteAll ();
		}
	}
}
