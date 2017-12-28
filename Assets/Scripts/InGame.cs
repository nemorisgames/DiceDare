using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Analytics;
using UnityEngine.Advertisements;

//using VoxelBusters.NativePlugins;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class InGame : MonoBehaviour, IInterstitialAdListener, IBannerAdListener, IRewardedVideoAdListener//, IRewardedVideoAdListener, IBannerAdListener, IInterstitialAdListener
{
	public bool daily = false;
	public bool tutorial = false;
	public TweenAlpha [] tutorialv3;
	[HideInInspector]
	public int tutorialIndex = -1;
	public UIPanel tutorialPanel;
	public Dice dice;
	Transform cells;
	GameObject [,] cellArray;
	public TextMesh [] cellsText;
	ArrayList texts = new ArrayList();
	ArrayList path = new ArrayList();
	public bool rotating = false;
	public GameObject finishedSign;
	public UILabel clockShow;
	public UILabel record;
	public UILabel levelNum;
	float recordSeconds;
	[HideInInspector]
	public float pauseTime = 0;
	float pauseAux = 0;
	public int secondsAvailable = 65;
	//public UITexture tutorial;
	//public Texture2D[] imgTutorial;
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
	public GameObject cellDeath;

	public AudioSource bgm_go;
	public static AudioSource bgm;

	//public TutorialVideo tutorialVideo;

	//[HideInInspector]
	public bool pause = false;

	int timesDied = 0;
	public GameObject hintScreen;
	public UILabel hintIndicator;
	int hintsAvailable = 3;

	//public TutorialVideo [] tutorialClips;

	public bool testing = false;
	float diceSize;
	public Transform adjacentCells;
	public Transform tutorialv2;
	public Material[] cellMaterials;
	public Material[] cellTextMaterials;

	//int tutorialIndex;
    public TweenAlpha hintMessage;
    int hintPressedNumber = 0;

	public DailyBlock currentBlock;
	DailyBlock lastBlock;
	public GameObject baseBlock;
    
	int dailyCorrect;
	int dailyWrong;
	[HideInInspector]
	public int currentScene;
	public GameObject medals_GO;
	public UISprite [] medals;

    string mensaje = "";

    // Use this for initialization
    void Start () {
		if(PlayerPrefs.GetInt("SeenTutorial",0) == 0 && !daily && !tutorial)
			//tutorialPanel.gameObject.SetActive(true);
			playTutorial();

		if (bgm == null) {
			bgm = bgm_go;
			DontDestroyOnLoad (bgm);
			bgm.Play ();
		}/* else {
			DestroyImmediate (bgm_go);
		}*/
        if (PlayerPrefs.GetInt("Mute") == 1)
            bgm.mute = true;
        else
            bgm.mute = false;
        string texto = PlayerPrefs.GetString("scene", "Scene1");
        if (texto == "TUTORIAL")
        {
            string num = texto.Split(new char[1] { 'e' })[2];
            int level = (int.Parse(num));
            currentScene = level;
            levelNum.text = "LEVEL " + level.ToString();
        }
        timesDied = PlayerPrefs.GetInt("timesDied", 0);
        dice = GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>();
		if(!daily)
        	componerEscena();

        cells = GameObject.Find("Cells").transform;
        cellsText = cells.GetComponentsInChildren<TextMesh>();
        foreach (TextMesh t in cellsText) {
            texts.Add(t.GetComponent<Transform>());
        }
		if(!daily){
       		recordSeconds = PlayerPrefs.GetFloat("record" + PlayerPrefs.GetString("scene", "Scene1"), -1f);
			if (recordSeconds > 0) {
				int minutes = (int)((recordSeconds) / 60);
				int seconds = (int)((recordSeconds) % 60);
				int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
				record.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;
			}
		}
        audio = GetComponent<AudioSource>();
        print("timesDied " + timesDied);
        if (timesDied >= 5 && !daily)
            StartCoroutine(lightPath(2));
        //StartCoroutine (cellArray[1,2].GetComponent<Cell>().shine ());
        //StartCoroutine (lightPath (2));
        diceSize = dice.GetComponent<MeshRenderer>().bounds.size.y / 2;
        hintsAvailable = PlayerPrefs.GetInt("hints", 2);
        hintIndicator.text = "" + hintsAvailable;
		//showTutorial = nguiCam.cullingMask;

		//GetConsecutiveDays();
		dice.EnableTutorialSign(false);

		InitMedals();

		if(daily){
			DailyInit();
		}
		
        
		Appodeal.setBannerCallbacks(this);
		Appodeal.setInterstitialCallbacks(this);
		Appodeal.setRewardedVideoCallbacks(this);
        
        showBanner();
    }

	public void NextTutorial(bool b){
		if(tutorialIndex == 5)
			return;
		if(tutorialIndex >= 0){
			if(tutorialIndex != 4)
				tutorialv3[tutorialIndex].PlayReverse();
			tutorialv3[tutorialIndex].gameObject.SetActive(false);
		}
		tutorialIndex++;
		if(tutorialv3.Length <= tutorialIndex)
			return;
		tutorialv3[tutorialIndex].gameObject.SetActive(true);
		if(tutorialIndex != 5)
			tutorialv3[tutorialIndex].PlayForward();
		else
			tutorialv3[tutorialIndex].value = 1;
		Pause();
		if(tutorialIndex == 2)
			b = true;
		StartCoroutine(tutorialPause(b));
	}

	IEnumerator tutorialPause(bool b){
		float pause = 0f;
		if(tutorialIndex % 2 == 0)
			pause = 3f;
		else
			pause = 0.5f;
		yield return new WaitForSeconds(pause);
		if(b)
			NextTutorial(false);
		else
			UnPause();
	}

	void InitMedals(){
		if(medals_GO != null){
			medals = new UISprite[4];
			for(int i=0;i<4;i++){
				medals[i] = medals_GO.transform.Find("M"+i.ToString()).GetComponent<UISprite>();
				medals[i].enabled = false;
			}
		}
	}
	void SetMedals(){
		int [] unlockedMedals = new int[4];
		for(int i=0;i<4;i++){
			unlockedMedals[i] = PlayerPrefs.GetInt("Medal"+i.ToString(),0);
			if(unlockedMedals[i] == 1)
				medals[i].enabled = true;
		}
	}

	void DailyInit(){
		ResetDiceNumbers();
		levelNum.text = "";
		currentBlock.currentNumbers = dice.faceNumbers();
		currentBlock.Init(currentBlock.currentNumbers, dice.currentOperation);
		dailyCorrect = 0;
		dailyWrong = 0;
		Pause();
	}

	void ResetDiceNumbers(){
		dice.transform.Find ("TextUp").GetComponent<TextMesh> ().text = "" + Random.Range(2,7);
		dice.transform.Find ("TextLeft").GetComponent<TextMesh> ().text = "" + Random.Range(1,7);
		dice.transform.Find ("TextForward").GetComponent<TextMesh> ().text = "" + Random.Range(1,7);
		dice.transform.Find ("TextDown").GetComponent<TextMesh> ().text = "" + Random.Range(2,7);
		dice.transform.Find ("TextRight").GetComponent<TextMesh> ().text = "" + Random.Range(2,7);
		dice.transform.Find ("TextBackward").GetComponent<TextMesh> ().text = "" + Random.Range(1,7);
	}

    public void hintPressed()
    {
        if (hintPressedNumber <= 0)
        {
            hintMessage.PlayForward();
            hintPressedNumber++;
        }
        else
        {
            hint();
        }
    }

    private void OnGUI()
    {
        //GUI.Label(new Rect(10, 10, 400, 200), mensaje);
    }

    public void hint(){
		if (!pause && path.Count > 0)
        {
            if (hintsAvailable <= 0) {
                showVideo();
                
                //hintScreen.SendMessage ("PlayForward");
				Pause ();
                
			} else {
				StartCoroutine (lightPath (2));
				hintsAvailable--;
				hintIndicator.text = "" + hintsAvailable;
				PlayerPrefs.SetInt ("hints", hintsAvailable);
			}
			#if !UNITY_EDITOR
			Analytics.CustomEvent ("hints");
			#endif
		}
	}

    public void showBanner()
    {
        Appodeal.show(Appodeal.BANNER_BOTTOM);
    }

	public void showInterstitial()
	{
		Appodeal.show(Appodeal.INTERSTITIAL);
    }

	public void showVideo(){
        Appodeal.show(Appodeal.REWARDED_VIDEO);
#if !UNITY_EDITOR
		Analytics.CustomEvent ("showVideo");
#endif
    }
    #region Rewarded Video callback handlers
    public void onRewardedVideoLoaded() { mensaje += ("Video loaded"); }
    public void onRewardedVideoFailedToLoad() { mensaje += ("Video failed"); }
    public void onRewardedVideoShown() { mensaje += ("Video shown"); }
    public void onRewardedVideoClosed() { mensaje += ("Video closed"); HandleShowResult(ShowResult.Finished); }
    public void onRewardedVideoFinished(int amount, string name) { mensaje += ("Reward: " + amount + " " + name); }
    #endregion

    #region Banner callback handlers
    public void onBannerLoaded() { mensaje += ("banner loaded"); }
    public void onBannerFailedToLoad() { mensaje += ("banner failed"); }
    public void onBannerShown() { mensaje += ("banner opened"); }
    public void onBannerClicked() { mensaje += ("banner clicked"); }
    #endregion

    #region Interstitial callback handlers
    public void onInterstitialLoaded() { mensaje += ("Interstitial loaded"); }
    public void onInterstitialFailedToLoad() { mensaje += ("Interstitial failed"); }
    public void onInterstitialShown() { mensaje += ("Interstitial opened"); }
    public void onInterstitialClosed() { mensaje += ("Interstitial closed"); }
    public void onInterstitialClicked() { mensaje += ("Interstitial clicked"); }
    #endregion
   
    private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			    hintsAvailable += 2;
			    PlayerPrefs.SetInt ("hints", hintsAvailable);
			    hintIndicator.text = "" + hintsAvailable;
			    closeHintScreen ();
                mensaje += "Ad succesful";
                break;
		case ShowResult.Skipped:
			    Debug.Log("The ad was skipped before reaching the end.");
                mensaje += "The ad was skipped before reaching the end.";
                closeHintScreen();
            break;
		case ShowResult.Failed:
			    Debug.LogError("The ad failed to be shown.");
                mensaje += "The ad failed to be shown.";
                closeHintScreen();
                break;
		}
	}

	public void closeHintScreen(){
		hintScreen.SendMessage ("PlayReverse");
		UnPause ();
	}

	public void componerEscena(){
		string completo = "";
		switch (PlayerPrefs.GetString ("scene", "Scene1")) {
		case "Scene1":completo = GlobalVariables.Scene1;break;
		case "Scene2":completo = GlobalVariables.Scene2;break;
		case "Scene3":completo = GlobalVariables.Scene3;break;
		case "Scene4":completo = GlobalVariables.Scene4;break;
		case "Scene5":completo = GlobalVariables.Scene5;break;
		case "Scene6":completo = GlobalVariables.Scene6;break;
		case "Scene7":completo = GlobalVariables.Scene7;break;
		case "Scene8":completo = GlobalVariables.Scene8;break;
		case "Scene9":completo = GlobalVariables.Scene9;break;
		case "Scene10":completo = GlobalVariables.Scene10;break;
		case "Scene11":completo = GlobalVariables.Scene11;break;
		case "Scene12":completo = GlobalVariables.Scene12;break;
		case "Scene13":completo = GlobalVariables.Scene13;break;
		case "Scene14":completo = GlobalVariables.Scene14;break;
		case "Scene15":completo = GlobalVariables.Scene15;break;
		case "Scene16":completo = GlobalVariables.Scene16;break;
		case "Scene17":completo = GlobalVariables.Scene17;break;
		case "Scene18":completo = GlobalVariables.Scene18;break;
		case "Scene19":completo = GlobalVariables.Scene19;break;
		case "Scene20":completo = GlobalVariables.Scene20;break;
		case "Scene21":completo = GlobalVariables.Scene21;break;
		}
		if(tutorial)
			completo = GlobalVariables.Scene1;
		string[] aux = completo.Split(new char[1]{'$'});
		string[] info = aux[0].Split(new char[1]{'|'});
		string[] arreglo = aux[1].Split(new char[1]{'|'});
		Vector3 posIni = new Vector3 (int.Parse (info [2]), 0f, -int.Parse (info [3]));
		if (int.Parse (info [4]) > 0) {
			//tutorialIndex = int.Parse (info [4]);
			//tutorialVideo.PlayClip (int.Parse (info [4]) - 1);
			//tutorial.mainTexture = imgTutorial [int.Parse (info [4]) - 1];
			//tutorial.transform.Find ("Sprite").GetComponent<UISprite> ().alpha = 1f;
			//tutorial.transform.SendMessage ("PlayForward");
			//tutorial.transform.Find ("Sprite").SendMessage ("PlayForward");
			//tutorialClips[(int.Parse (info [4]) - 1)].gameObject.SetActive(true);
			//tutorialVideo.gameObject.SetActive(true);
			//StartCoroutine(tutorialVideo.PlayClip(int.Parse (info [4]) - 1));
			//Pause ();
		}
		
		string completoNumbers = "";
		switch (PlayerPrefs.GetString ("scene", "Scene1")) {
		case "Scene1":completoNumbers = GlobalVariables.Scene1Numbers;break;
		case "Scene2":completoNumbers = GlobalVariables.Scene2Numbers;break;
		case "Scene3":completoNumbers = GlobalVariables.Scene3Numbers;break;
		case "Scene4":completoNumbers = GlobalVariables.Scene4Numbers;break;
		case "Scene5":completoNumbers = GlobalVariables.Scene5Numbers;break;
		case "Scene6":completoNumbers = GlobalVariables.Scene6Numbers;break;
		case "Scene7":completoNumbers = GlobalVariables.Scene7Numbers;break;
		case "Scene8":completoNumbers = GlobalVariables.Scene8Numbers;break;
		case "Scene9":completoNumbers = GlobalVariables.Scene9Numbers;break;
		case "Scene10":completoNumbers = GlobalVariables.Scene10Numbers;break;
		case "Scene11":completoNumbers = GlobalVariables.Scene11Numbers;break;
		case "Scene12":completoNumbers = GlobalVariables.Scene12Numbers;break;
		case "Scene13":completoNumbers = GlobalVariables.Scene13Numbers;break;
		case "Scene14":completoNumbers = GlobalVariables.Scene14Numbers;break;
		case "Scene15":completoNumbers = GlobalVariables.Scene15Numbers;break;
		case "Scene16":completoNumbers = GlobalVariables.Scene16Numbers;break;
		case "Scene17":completoNumbers = GlobalVariables.Scene17Numbers;break;
		case "Scene18":completoNumbers = GlobalVariables.Scene18Numbers;break;
		case "Scene19":completoNumbers = GlobalVariables.Scene19Numbers;break;
		case "Scene20":completoNumbers = GlobalVariables.Scene20Numbers;break;
		case "Scene21":completoNumbers = GlobalVariables.Scene21Numbers;break;
		}
		if(tutorial)
			completoNumbers = GlobalVariables.Scene1Numbers;
		string completoPath = "";
		switch (PlayerPrefs.GetString ("scene", "Scene1")) {
		case "Scene1":completoPath = GlobalVariables.Scene1Path;break;
		case "Scene2":completoPath = GlobalVariables.Scene2Path;break;
		case "Scene3":completoPath = GlobalVariables.Scene3Path;break;
		case "Scene4":completoPath = GlobalVariables.Scene4Path;break;
		case "Scene5":completoPath = GlobalVariables.Scene5Path;break;
		case "Scene6":completoPath = GlobalVariables.Scene6Path;break;
		case "Scene7":completoPath = GlobalVariables.Scene7Path;break;
		case "Scene8":completoPath = GlobalVariables.Scene8Path;break;
		case "Scene9":completoPath = GlobalVariables.Scene9Path;break;
		case "Scene10":completoPath = GlobalVariables.Scene10Path;break;
		case "Scene11":completoPath = GlobalVariables.Scene11Path;break;
		case "Scene12":completoPath = GlobalVariables.Scene12Path;break;
		case "Scene13":completoPath = GlobalVariables.Scene13Path;break;
		case "Scene14":completoPath = GlobalVariables.Scene14Path;break;
		case "Scene15":completoPath = GlobalVariables.Scene15Path;break;
		case "Scene16":completoPath = GlobalVariables.Scene16Path;break;
		case "Scene17":completoPath = GlobalVariables.Scene17Path;break;
		case "Scene18":completoPath = GlobalVariables.Scene18Path;break;
		case "Scene19":completoPath = GlobalVariables.Scene19Path;break;
		case "Scene20":completoPath = GlobalVariables.Scene20Path;break;
		case "Scene21":completoPath = GlobalVariables.Scene21Path;break;
		}
		if(tutorial)
			completoPath = GlobalVariables.Scene1Path;
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
		dice.transform.Find ("TextUp").GetComponent<TextMesh> ().text = "" + int.Parse (infoNumbers[0]);
		dice.transform.Find ("TextLeft").GetComponent<TextMesh> ().text = "" + int.Parse (infoNumbers[1]);
		dice.transform.Find ("TextForward").GetComponent<TextMesh> ().text = "" + int.Parse (infoNumbers[2]);
		int indice = 0;
		Transform rootCells = GameObject.Find ("Cells").transform;
		cellArray = new GameObject[int.Parse(info[0]),int.Parse(info[1])];
		for(int i = 0; i < int.Parse(info[0]); i++){
			for (int j = 0; j < int.Parse (info [1]); j++) {
				GameObject g = null;
				switch (int.Parse (arreglo [indice])) {
				case -2:
					g = (GameObject)Instantiate (cellEnd, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case -1:
					g = (GameObject)Instantiate (cellBegin, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case 1:
				case 2:
					g = (GameObject)Instantiate (cellNormal, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case 3:
					g = (GameObject)Instantiate (cellSum, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case 4:
					g = (GameObject)Instantiate (cellSubstraction, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case 5:
					g = (GameObject)Instantiate (cellMultiplication, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case 6:
					g = (GameObject)Instantiate (cellDivision, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case 7:
					g = (GameObject)Instantiate (cellCW, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case 8:
					g = (GameObject)Instantiate (cellCCW, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
					break;
				case 9:
					g = (GameObject)Instantiate (cellDeath, new Vector3 (j, -0.1f, -i) - posIni, Quaternion.identity);
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
		if (!pause) {
			pauseAux = Time.timeSinceLevelLoad;
			pause = true;
		}
	}

	public void UnPause(){
		if (pause) {
			pause = false;
			pauseTime += Time.timeSinceLevelLoad - pauseAux;
			pauseAux = 0;
		}
	}

	public void calculateResult(int diceValueA, int diceValueB, int cellValue){
		print ("calculating");
		if (checkOperationResult (diceValueA, diceValueB) != cellValue) {
			if(daily){
				DailyAnswer(false);
				componerEscena_Daily();
			}
			else
				badMove ();
		} else {
			audio.pitch = Random.Range (0.95f, 1.05f);
			audio.PlayOneShot(audioGoodMove);
			if(path.Count > 0)
				path.RemoveAt (0);
			Instantiate (dice.goodMove, new Vector3(dice.transform.position.x,dice.transform.position.y + diceSize, dice.transform.position.z), Quaternion.LookRotation(Vector3.up));
			if(daily){
				DailyAnswer(true);
				componerEscena_Daily();
			}
		}
	}

	public void badMove(){
#if !UNITY_EDITOR
		Analytics.CustomEvent ("badMove", new Dictionary<string, object> {
		{ "scene", PlayerPrefs.GetString("scene", "Scene1") },
		{ "steps", dice.steps },
		{ "place", dice.gameObject.transform.position},
		{ "time", secondsAvailable - Time.timeSinceLevelLoad }
		});
#endif
        if (Random.Range(0, 100) < 30)
        {
            showInterstitial();
        }
        //else
            continueBadMove();
	}

    void continueBadMove()
    {
        print("badMove");
        dice.enabled = false;
        StartCoroutine(reloadScene());
        audio.pitch = 1f;
        audio.PlayOneShot(audioBadMove);
        dice.GetComponent<Animator>().SetTrigger("BadMove");
    }

	IEnumerator reloadScene(){
		yield return new WaitForSeconds (1f);
		timesDied++;
		PlayerPrefs.SetInt ("timesDied", timesDied);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	//0: adyacente, 1: tres adyacentes, 2: todos
	public IEnumerator lightPath(int mode){
		if (!pause && !daily) {
			Pause ();
			mode = Mathf.Clamp (mode, 0, 2);
			switch (mode) {
			case 0:
				dice.EnableTutorialSign(true);
				if (path.Count > 0)
					StartCoroutine (cellArray [(int)((Vector2)path [0]).x, (int)((Vector2)path [0]).y].GetComponent<Cell> ().shine (2));
				yield return new WaitForSeconds (1f);
				dice.EnableTutorialSign(false);
				break;
			case 1:
				int aux = Mathf.Min (3, path.Count);
				for (int i = 0; i < aux; i++) {
					StartCoroutine (cellArray [(int)((Vector2)path [i]).x, (int)((Vector2)path [i]).y].GetComponent<Cell> ().shine (1));
					yield return new WaitForSeconds (1f / 2);
				}
				break;
			case 2:
				foreach (Vector2 v in path) {
					StartCoroutine (cellArray [(int)v.x, (int)v.y].GetComponent<Cell> ().shine (1));
					yield return new WaitForSeconds (1f / 2);
				}
				foreach (Transform t in adjacentCells)
					t.GetComponent<AdjacentCellFinder> ().active = true;
                StartCoroutine (cellArray [(int)((Vector2)path [0]).x, (int)((Vector2)path [0]).y].GetComponent<Cell> ().shine (2));
				yield return new WaitForSeconds (1f);
				break;
			}
			UnPause ();
			/*yield return new WaitForSeconds (0.03f);
			foreach (Transform t in adjacentCells)
				t.GetComponent<AdjacentCellFinder> ().EnableCell (true);*/
		}
	}

    public Dice.Direction getDirection(Vector3 currentPos, Vector2 endPos)
    {
        Vector2 pos = new Vector2(currentPos.x * 1, currentPos.z * 1);
        endPos = new Vector2(endPos.y, endPos.x);
        print(currentPos + ", (" + (pos.x) + ", " + (pos.y) + "), " + endPos);
        if (pos.x == (int)endPos.x)
        {
            if (pos.y < (int)endPos.y)
                return Dice.Direction.Down;
            else
                return Dice.Direction.Up;
        }
        else
        {
            if(pos.y == (int)endPos.y)
            {
                if (pos.x < (int)endPos.x)
                    return Dice.Direction.Left;
                else
                    return Dice.Direction.Right;
            }
        }
        return Dice.Direction.Down;
    }


    public Camera nguiCam;
	/*public LayerMask hideTutorial;
	LayerMask showTutorial;

	public void HideTutorial(){
		if (nguiCam.cullingMask != hideTutorial)
			nguiCam.cullingMask = hideTutorial;
		else
			nguiCam.cullingMask = showTutorial;
	}*/

	public bool finished = false;

	public void finishGame(){
        PlayerPrefs.SetInt("timesDied", 0);

        finished = true;
		print("Finished");
		//HideTutorial();
		Pause ();
		foreach (Transform t in adjacentCells)
			t.GetComponent<AdjacentCellFinder> ().EnableCell (false);
		//for (int i = 0; i < tutorialClips.Length; i++)

		if(!daily) StartCoroutine (dropCells ());
		dice.enabled = false;
		//finishedSign.SetActive (true);
		//finishedSign.SendMessage ("PlayForward");
		dice.enabled = false;
		dice.transform.rotation = Quaternion.identity;
		dice.GetComponent<Animator> ().SetTrigger ("Finished");
		//tutorialVideo.ToggleOff ();
		audio.pitch = 1f;
		audio.PlayOneShot(audioFinish);

		if(tutorial)
			return;

        if (!daily && Time.timeSinceLevelLoad - pauseTime < PlayerPrefs.GetFloat("record" + PlayerPrefs.GetString("scene", "Scene1"), float.MaxValue))
        {
            PlayerPrefs.SetFloat("record" + PlayerPrefs.GetString("scene", "Scene1"), Time.timeSinceLevelLoad - pauseTime);
            /*if (NPBinding.GameServices.LocalUser.IsAuthenticated)
            {
                NPBinding.GameServices.ReportScoreWithGlobalID(PlayerPrefs.GetString("scene", "Scene1"), (int)((Time.timeSinceLevelLoad - pauseTime) * 100), (bool _success, string _error) => {

                    if (_success)
                    {
                        Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} finished successfully.", PlayerPrefs.GetString("scene", "Scene1")));
                        Debug.Log(string.Format("New score= {0}.", Time.timeSinceLevelLoad - pauseTime));
                    }
                    else
                    {
                        Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} failed.", PlayerPrefs.GetString("scene", "Scene1")));
                        Debug.Log(string.Format("Error= {0}.", _error.ToString()));
                    }
                });
            }*/
        }
		#if !UNITY_EDITOR
		Analytics.CustomEvent ("finish", new Dictionary<string, object> {
		{ "scene", PlayerPrefs.GetString("scene", "Scene1") },
			{ "steps", dice.steps },
			{ "time", secondsAvailable - Time.timeSinceLevelLoad }
		});
		#endif

		if(daily){
			UpdateConsecutiveDays();
			StartCoroutine(finishDaily());
		}
		else{
			Debug.Log("skill");
			string texto = PlayerPrefs.GetString ("scene", "Scene1");
			string num = texto.Split (new char[1]{ 'e' }) [2];
			int level = (int.Parse (num) + 1);
			if(level < GlobalVariables.nLevels)
				PlayerPrefs.SetInt ("unlockedScene" + level, 1);
			Debug.Log(LevelSelection.LevelSkillTotal());
			StartCoroutine(moveSlider(dailySlider, LevelSelection.LevelSkillTotal() + PlayerPrefs.GetFloat("totalDaily",0)/2f));
		}
		
		CalculateMedals();
			
	}

    public void checkLeaderboard()
    {
        /*NPBinding.GameServices.ShowLeaderboardUIWithGlobalID(PlayerPrefs.GetString("scene", "Scene1"), eLeaderboardTimeScope.ALL_TIME, leaderboardCallback());
    }
    GameServices.GameServiceViewClosed leaderboardCallback()
    {
        return null;*/
    }

    public int checkOperationResult(int diceValueB, int diceValueA){
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
			//if (res == 0)
			//	res = -1;
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

	IEnumerator dropCells(){
		Cell[] cellsAux = GameObject.Find ("Cells").transform.GetComponentsInChildren<Cell> ();
		for (int i = 0; i < cellsAux.Length; i++) {
			if (cellsAux [i].stateCell != Cell.StateCell.Passed) {
				float rnd = Random.Range (0.01f, 0.08f);
				yield return new WaitForSeconds (rnd);
				Rigidbody rb = cellsAux[i].GetComponent<Rigidbody> ();
				rb.isKinematic = false;
				rb.useGravity = true;
				StartCoroutine (disableCell (cellsAux[i]));
				if (i == cellsAux.Length - 1) {
					yield return new WaitForSeconds (1.4f);
					finishedSign.SetActive (true);
					finishedSign.SendMessage ("PlayForward");
                    showBanner();
                }
			}
		}
	}

	IEnumerator finishDaily(){
		CalculateDailyResult();
		yield return new WaitForSeconds (1.4f);
		finishedSign.SetActive (true);
		finishedSign.SendMessage ("PlayForward");
	}

	IEnumerator disableCell(Cell c){
		yield return new WaitForSeconds (2f);
		c.gameObject.SetActive (false);
	}

	void timesUp(){
		clockShow.text = "00";
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
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void skipTutorial(){
		PlayerPrefs.SetInt("SeenTutorial",1);
		playAgain();
	}

	public void playTutorial(){
		Appodeal.hide(Appodeal.BANNER_BOTTOM);
        SceneManager.LoadScene ("InGame_tutorial");
	}

	public void exit()
    {
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
        /*string texto = PlayerPrefs.GetString ("scene", "Scene1");
		string num = texto.Split (new char[1]{ 'e' }) [2];
		int level = (int.Parse (num) + 1);
		if(level < GlobalVariables.nLevels)
			PlayerPrefs.SetInt ("unlockedScene" + level, 1);*/
		if(tutorial && PlayerPrefs.GetInt("SeenTutorial",0) == 0)
			PlayerPrefs.SetInt("SeenTutorial",1);
		Destroy (bgm.gameObject);
		SceneManager.LoadScene ("LevelSelection");
	}

    public void exitGame()
    {
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
        Application.Quit();
    }


	public void next()
    {
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
        string texto = PlayerPrefs.GetString ("scene", "Scene1");
        string num = "1";
        if(texto != "InGame_tutorial")
            num = texto.Split (new char[1]{ 'e' }) [2];
		#if !UNITY_EDITOR
		Analytics.CustomEvent ("enteringLevel" + num);
		#endif
		int level = (int.Parse (num));

		if(!tutorial)
			level += 1;
		else
			if(PlayerPrefs.GetInt("SeenTutorial",0) == 0)
				PlayerPrefs.SetInt("SeenTutorial",1);

		if (level > GlobalVariables.nLevels)
			exit ();
		else {
			PlayerPrefs.SetInt ("timesDied", 0);
			//PlayerPrefs.SetInt ("unlockedScene" + level, 1);
			PlayerPrefs.SetString ("scene", "Scene" + level);
			SceneManager.LoadScene ("InGame");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Destroy (bgm.gameObject);
            Appodeal.hide(Appodeal.BANNER_BOTTOM);
            SceneManager.LoadScene ("LevelSelection");
		}
		if (finishedSign.activeSelf) {
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
		if(!tutorial){
			if (!pause && !daily) {
				int minutes = (int)((Time.timeSinceLevelLoad - pauseTime) / 60);
				int seconds = (int)((Time.timeSinceLevelLoad - pauseTime) % 60);
				//int dec = (int)(((Time.timeSinceLevelLoad - pauseTime) % 60 * 10f) - ((int)((Time.timeSinceLevelLoad - pauseTime) % 60) * 10));
				if(!finished) clockShow.text = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
			}
			if(!pause && daily){
				Debug.Log("counting");
				int minutes = (int)((60 - Time.timeSinceLevelLoad + pauseTime + extraSeconds) / 60);
				int seconds = (int)((60 - Time.timeSinceLevelLoad + pauseTime + extraSeconds) % 60);
				//int dec = (int)(((1 - Time.timeSinceLevelLoad) % 60 * 10f) - ((int)((1 - Time.timeSinceLevelLoad) % 60) * 10));
				if(!finished && seconds >= 0) clockShow.text = (seconds < 10 ? "0" : "") + seconds;

				if(!finished && 60 + extraSeconds - (Time.timeSinceLevelLoad - pauseTime) <= 0)
					finishGame();
			}
		}
		

		//test
		if (testing) {
			if (Input.GetKeyDown (KeyCode.Q)) {
				StartCoroutine (lightPath (1));
			}

			if (Input.GetKeyDown (KeyCode.E)) {
				StartCoroutine (lightPath (0));
			}

			if (Input.GetKeyDown (KeyCode.R)) {
				StartCoroutine (lightPath (2));
			}

			if (Input.GetKeyDown (KeyCode.P)) {
				PlayerPrefs.DeleteAll ();
			}
		}
		if (rotating || pause || dice.onMovement) {
			if (rotating)
				foreach (Transform t in adjacentCells)
					t.GetComponent<AdjacentCellFinder> ().EnableCell (false);
			adjacentCells.gameObject.SetActive (false);
		}
		else if (!adjacentCells.gameObject.activeSelf && !finished)
			adjacentCells.gameObject.SetActive (true);
				

		adjacentCells.position = dice.transform.position;
	}

	public static void GetConsecutiveDays(){
		//load last played date
		int consecutiveDays = PlayerPrefs.GetInt("consecutiveDays",-1);
		System.DateTime lastPlayedDate = System.DateTime.Parse(PlayerPrefs.GetString("lastPlayedDate",System.DateTime.Now.Date.ToString()));
		int daysSinceLastPlay = (int)(System.DateTime.Now - lastPlayedDate).TotalDays;
		if(daysSinceLastPlay == 0){
			if(consecutiveDays == -1){
				Debug.Log("First stage played");
				PlayerPrefs.SetInt("consecutiveDays",1);
			}
			else
				Debug.Log("Already played today");
		}
		if(daysSinceLastPlay == 1){
			PlayerPrefs.SetInt("consecutiveDays",consecutiveDays + 1);
		}
		else if(daysSinceLastPlay > 1){
			PlayerPrefs.SetInt("consecutiveDays",0);
			Debug.Log("Haven't played in over a day");
		}
		
		if(System.DateTime.Now != lastPlayedDate)
			PlayerPrefs.SetString("lastPlayedDate",System.DateTime.Now.Date.ToString());
	}

	void UpdateConsecutiveDays(){
		System.DateTime lastPlayedDate = System.DateTime.Parse(PlayerPrefs.GetString("lastPlayedDate",System.DateTime.Now.Date.ToString()));
		int daysSinceLastPlay = (int)(System.DateTime.Now - lastPlayedDate).TotalDays;
		if(daysSinceLastPlay != 0){
			int consecutiveDays = PlayerPrefs.GetInt("consecutiveDays");
			consecutiveDays = Mathf.Clamp(consecutiveDays + 1,0,7);
			PlayerPrefs.SetString("lastPlayedDate",System.DateTime.Now.Date.ToString());
			PlayerPrefs.SetInt("consecutiveDays",consecutiveDays);
		}
	}

	void componerEscena_Daily(){
		currentBlock.DropRemainingBlocks();
		GameObject aux = (GameObject)Instantiate(baseBlock, new Vector3(dice.transform.position.x, 0f, dice.transform.position.z), currentBlock.transform.rotation);
		DailyBlock block = aux.GetComponent<DailyBlock>();
		currentBlock.currentNumbers = dice.faceNumbers();
		Dice.Operation operation = dice.currentOperation;
		if(currentBlock.currentNumbers[0] > 100 || currentBlock.currentNumbers[0] * currentBlock.currentNumbers[1] >= 100 || currentBlock.currentNumbers[0] * currentBlock.currentNumbers[2]  >= 100 || currentBlock.currentNumbers[0] == 0 || Mathf.Abs(currentBlock.currentNumbers[0]) > 10 && (Mathf.Abs(currentBlock.currentNumbers[1]) > 10 || Mathf.Abs(currentBlock.currentNumbers[2]) > 10)){
			ResetDiceNumbers();
			currentBlock.currentNumbers = dice.faceNumbers();
		}

		int switchOp = Random.Range(1,4);
		
		if(((dailyCorrect + dailyWrong) % switchOp == 0) || dice.currentOperation == Dice.Operation.Div){
			operation = (Dice.Operation)(Random.Range(0,4));
			while(operation == dice.currentOperation){
				operation = (Dice.Operation)(Random.Range(0,4));
			}
			dice.changeOperation(operation);
		}

		block.Init(currentBlock.currentNumbers, operation);
		if(lastBlock != null)
			lastBlock.DropPassedBlocks();
		lastBlock = currentBlock;
		currentBlock = block;
		levelNum.text = "Right: "+dailyCorrect+" ; Wrong: "+dailyWrong;
		//Pause();
		
	}

	int dailyConsec = 0;
	float extraSeconds = 0;
	float dailySPA = 2.4f;

	public void DailyAnswer(bool b){
		if(b){
			dailyCorrect++;
			dailyConsec++;
			if(dailyConsec == 3){
				extraSeconds = Mathf.Clamp(extraSeconds += 5f,0f,25f);
				dailyConsec = 0;
			}
		}
		else{
			dailyWrong++;
			dailyConsec = 0;
		}
	}

	float dailyOptimo = 25;
	float dailyPercentage;
	public UILabel dailyCorrectLabel;
	public UILabel dailyPercentageLabel;
	public UISlider dailySlider;

	void CalculateDailyResult(){
		float auxOptimo = dailyOptimo + Mathf.Floor(extraSeconds/2.4f);
		dailyOptimo = auxOptimo;
		dailyCorrectLabel.text = dailyCorrect+"/"+(dailyCorrect+dailyWrong);
		dailyPercentage = dailyCorrect/(float)(dailyCorrect+dailyWrong);
		float result = ((dailyPercentage * dailyCorrect)/dailyOptimo);
		result = Mathf.Clamp01(result);
		//dailyPercentageLabel.text = Mathf.Round(result*10000f)/100f+"%";

		StartCoroutine(raiseNumber(dailyPercentageLabel, result));
		float totalPercentage = PlayerPrefs.GetFloat("totalDaily",0);
		
		float consec = Mathf.Clamp((float)PlayerPrefs.GetInt("consecutiveDays"),0f,7f);
		totalPercentage = Mathf.Clamp01(totalPercentage * (1 + consec/150f));

		if(consec == -1)
			PlayerPrefs.SetInt("consecutiveDays",1);
		else
			PlayerPrefs.SetInt("consecutiveDays",(int)consec+1);

		Debug.Log(result + ", "+ totalPercentage);
		if(result > totalPercentage)
			totalPercentage = (totalPercentage + result * 1.1f)/2f;
		else if(result < totalPercentage)
			totalPercentage = (totalPercentage - (1 - result) * 0.05f);
		
		PlayerPrefs.SetFloat("totalDaily",Mathf.Clamp01(totalPercentage));
		PlayerPrefs.SetString("lastPlayedDate",System.DateTime.Now.Date.ToString());
		//dailySlider.value = LevelSelection.LevelSkillTotal() + totalPercentage/2f;
		PlayerPrefs.SetInt("triesLeft",Mathf.Clamp(PlayerPrefs.GetInt("triesLeft") - 1,0,int.MaxValue));
		
		StartCoroutine(moveSlider(dailySlider, LevelSelection.LevelSkillTotal() + totalPercentage/2f));
	}

	void CalculateMedals(){
		float totalSkill = LevelSelection.LevelSkillTotal() + PlayerPrefs.GetFloat("totalDaily")/2f;
		SetMedals();

		if(totalSkill >= 0.1f && PlayerPrefs.GetInt("Medal0",0) == 0){
			TweenMedal(0);
		}
			
		if(totalSkill >= 0.25f && PlayerPrefs.GetInt("Medal1",0) == 0){
			TweenMedal(1);
		}
			
		if(totalSkill >= 0.5f && PlayerPrefs.GetInt("Medal2",0) == 0){
			TweenMedal(2);
		}
			
		if(totalSkill == 1f && PlayerPrefs.GetInt("Medal3",0) == 0){
			TweenMedal(3);
		}
	}

	IEnumerator medalDelay(TweenAlpha medal){
		yield return new WaitForSeconds(1.5f);
		medal.PlayForward();
	}

	void TweenMedal(int index){
		TweenAlpha medal = medals[index].GetComponent<TweenAlpha>();
		medals[index].enabled = true;
		medal.value = 0;
		audio.PlayOneShot(audioGoodMove);
		StartCoroutine(medalDelay(medal));
		PlayerPrefs.SetInt("Medal"+index.ToString(),1);
	}

	public static IEnumerator moveSlider(UISlider slider, float target){
		for(float f = 0; f < target; f+=0.01f){
			slider.value = f;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		slider.value = target;
	}

	public static IEnumerator raiseNumber(UILabel label, float target){
		for(float f = 0; f < target; f+=0.01f){
			label.text = Mathf.Round(f*10000f)/100f+"%";
			yield return new WaitForSeconds(Time.deltaTime);
			if(f > target){
				label.text = Mathf.Round(f*10000f)/100f+"%";
				yield break;
			}
		}
	}

    
    
}
