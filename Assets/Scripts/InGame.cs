using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Analytics;
using UnityEngine.Advertisements;

//using VoxelBusters.NativePlugins;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class InGame : MonoBehaviour//, IRewardedVideoAdListener, IBannerAdListener, IInterstitialAdListener
{
	public bool daily = false;
	public bool tutorial = false;
	public TweenAlpha [] tutorialv3;
	//[HideInInspector]
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
    public GameObject cellSpikes;
    public GameObject cellChangeNumber;
    public GameObject cellDissapearNumber;

    public AudioSource bgm_go;
	public static AudioSource bgm;

    GameObject limitRight;
    GameObject limitLeft;
    GameObject limitUp;
    GameObject limitDown;

	//[HideInInspector]
	public bool pause = false;
	public bool unPauseOnMove = true;

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
	public UILabel triesLabel;

    string mensaje = "";
	int tutorialMode;
	string [] tutorialOps;
    // Use this for initialization
    AppodealDemo appodealDemo;

    public GameObject connectionProblem;
    public GameObject newRecordSign;
    public UILabel stageTime;
	string currentSceneText;
	string playerprefScene;
	
	[Header("Tutorial")]
	public UIPanel panelTransicion;
	private int newTutorialIndex = 0;
	public UIPanel panelUITutorial;
	public UILabel tutorialLeft, tutorialRight;
	private GameObject tutorialLeftGO, tutorialRightGO;
	public TweenScale tutorialLeftScale, tutorialRightScale;
	public TweenScale tutorialBottom;
	public UIWidget tutorialBottomWidget;
	public GameObject [] tutorialInstructions;
	private IEnumerator ITutorialPath, ITutorialPanel;
	private Color redText, greenText;
	private float totalTime = 0f;

    // Use this for initialization
    void Start () {
		if(!daily)
        	GlobalVariables.SetScenes();
        GameObject g = GameObject.Find("AppoDeal");
        if(g != null)
            appodealDemo = g.GetComponent<AppodealDemo>();
        //if (PlayerPrefs.GetInt("SeenTutorial",0) == 0 && !daily && !tutorial)
        //tutorialPanel.gameObject.SetActive(true);
        //playTutorial();

		ColorUtility.TryParseHtmlString("#FF321D",out redText);
		ColorUtility.TryParseHtmlString("#17CF3F",out greenText);

        if (bgm == null) {
			bgm = bgm_go;
			DontDestroyOnLoad (bgm);
			bgm.Play ();
		}/* else {
			DestroyImmediate (bgm_go);
		}*/
        /*if (PlayerPrefs.GetInt("Mute") == 1)
            bgm.mute = true;
        else
            bgm.mute = false;*/
        string texto = PlayerPrefs.GetString("scene", "Scene1");
        currentSceneText = texto;
        if (texto != "TUTORIAL")
        {
            string num = texto.Split(new char[1] { 'e' })[2];
            int level = (int.Parse(num));
            //int level = GlobalVariables.getSceneIndex(texto) + 1;
            currentScene = level;
            levelNum.text = "LEVEL " + level.ToString();
        }
        timesDied = PlayerPrefs.GetInt("timesDied", 0);
		totalTime = PlayerPrefs.GetFloat("totalTime",0);
        dice = GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>();

		if(tutorial){
			unPauseOnMove = false;
		}


		if(!daily)
        	componerEscena();

        cells = GameObject.Find("Cells").transform;
        cellsText = cells.GetComponentsInChildren<TextMesh>();
        foreach (TextMesh t in cellsText) {
            if(t.gameObject.name != "TextApprox")
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
		if(!tutorial){
			hintsAvailable = PlayerPrefs.GetInt("hints", 2);
        	hintIndicator.text = "" + hintsAvailable;
		}
		//showTutorial = nguiCam.cullingMask;

		//GetConsecutiveDays();
		dice.EnableTutorialSign(false);

		InitMedals();

		if(daily){
			DailyInit();
		}
		if(tutorial){
			dice.SetNumbers(1,2,3,3,3,3);
			tutorialLeftScale.transform.localScale = Vector3.up + Vector3.forward;
			tutorialRightScale.transform.localScale = Vector3.up + Vector3.forward;
			tutorialBottom.transform.localScale = Vector3.right + Vector3.forward;
			tutorialBottomWidget.alpha = 0;
		}

		Analytics_Start();

        if(appodealDemo != null)
            appodealDemo.showBanner(Appodeal.BANNER_BOTTOM);

    }

	private Cell tutorialEndBlock;

	public int TutorialIndex(){
		return newTutorialIndex;
	}

	public bool restartCoroutine;

	void DailyInit(){
		Debug.Log("here");
		ResetDiceNumbers();
		levelNum.text = "";
		currentBlock.currentNumbers = dice.faceNumbers();
		if(tutorial){
			tutorialEndBlock = currentBlock.InitNormalCell(0,0);
			currentBlock.InitNormalCell(1,5);
			tutorialLeft.text = 3+"\n\n"+dice.OperationString()+" "+1+"\n_____\n\n"+0;
			tutorialRight.text = 3+"\n\n"+dice.OperationString()+" "+2+"\n_____\n\n"+5;
			ITutorialPanel = ShowTutorialPanel(0,5,"+",1.5f);
			ITutorialPath = LightDice(Vector3.up,-Vector3.forward,1,2f);
			ShowCurrentTutorial();
		}
		else
			currentBlock.Init(currentBlock.currentNumbers, dice.currentOperation);
		dailyCorrect = 0;
		dailyWrong = 0;
		Pause();
	}

	private void ShowCurrentTutorial(){
		StartCoroutine(TutorialOrder());
	}

	private IEnumerator TutorialOrder(){
		if(newTutorialIndex != 3)
			tutorialBottomWidget.alpha = 0;
		yield return new WaitForSeconds(0.5f);
		switch(newTutorialIndex){
			case 0:
				ShowBottomTutorial(3);
				yield return new WaitForSeconds(0.5f);
				tutorialBottom.PlayForward();
			break;
			case 2:
				ShowBottomTutorial(0);
				yield return new WaitForSeconds(0.5f);
				tutorialBottom.PlayForward();
			break;
			case 1:
				ShowBottomTutorial(2);
				yield return new WaitForSeconds(0.5f);
				tutorialBottom.PlayForward();
			break;
			case 4:
				ShowBottomTutorial(1);
				yield return new WaitForSeconds(0.5f);
				tutorialBottom.PlayForward();
			break;
			default:
			break;
		}
		restartCoroutine = false;
		if(ITutorialPanel != null){
			//StopCoroutine(ITutorialPanel);
			StartCoroutine(ITutorialPanel);
		}
			
		if(ITutorialPath != null){
			//StopCoroutine(ITutorialPath);
			StartCoroutine(ITutorialPath);
		}	
	}

	private void ShowBottomTutorial(int index){
		foreach(GameObject g in tutorialInstructions){
			g.SetActive(false);
		}
		tutorialInstructions[index].SetActive(true);
	}

	public struct DiceLights{
		public Vector3 up;
		public Vector3 dir;
		public int cell;
		public float delay;
		public DailyBlock block;

		public void Config(Vector3 _up, Vector3 _dir, int _cell, DailyBlock _block, float _delay = 3f){
			up = _up;
			dir = _dir;
			cell = _cell;
			delay = _delay;
			block = _block;
		}
	}

	private DiceLights lightConfig;

	void componerEscena_Tutorial(){
		Pause();
		//currentBlock.DropRemainingBlocks();
		GameObject aux = (GameObject)Instantiate(baseBlock, new Vector3(dice.transform.position.x, 0f, dice.transform.position.z), currentBlock.transform.rotation);
		DailyBlock block = aux.GetComponent<DailyBlock>();
		currentBlock.currentNumbers = dice.faceNumbers();
		Dice.Operation operation = dice.currentOperation;
		
		newTutorialIndex++;
		switch(newTutorialIndex){
			case 1:
				block.InitNormalCell(1,6);
				block.InitOperationCell(0,Dice.Operation.Rest,6);
				ITutorialPanel = ShowTutorialPanel(6,6,"+",1f);
				ITutorialPath = LightDice(Vector3.forward,-Vector3.right,0,1.5f);
				ShowCurrentTutorial();	
			break;
			case 2:
				block.InitNormalCell(0,3);
				block.InitNormalCell(1,3);
				ITutorialPanel = ShowTutorialPanel(3,3,"-",1f);
				ITutorialPath = LightDice(Vector3.right,Vector3.up,1,1.5f);
				ShowCurrentTutorial();
			break;
			case 3:
				block.InitNormalCell(0,1);
				block.InitOperationCell(1,Dice.Operation.Sum,-3);
				ITutorialPanel = ShowTutorialPanel(1,-3,"-",1f);
				ITutorialPath = LightDice(-Vector3.up,Vector3.right,1,1.5f);
				ShowCurrentTutorial();
				unPauseOnMove = true;
			break;
			case 4:
				StartCoroutine(TutorialEndCell());
				//HideTutorialPanel();
				ITutorialPanel = null;
				ITutorialPath = null;
				ShowCurrentTutorial();
			break;
			default:
				if(unPauseOnMove)
					UnPause();
			break;
		}

		currentBlock = block;
	}

	private IEnumerator ShowTutorialPanel(int left, int right, string sign, float delay = 1.5f, bool writeFromDice = true){
		Pause();
		yield return new WaitForSeconds(delay);
		tutorialLeftScale.PlayForward();
		tutorialRightScale.PlayForward();
		if(true){
			tutorialLeft.text = dice.faceNumbers()[0]+"\n\n"+sign+" "+dice.faceNumbers()[1]+"\n_____\n\n"+left;
			tutorialRight.text = dice.faceNumbers()[0]+"\n\n"+sign+" "+dice.faceNumbers()[2]+"\n_____\n\n"+right;
		}
	}

	private IEnumerator LightDice(Vector3 up, Vector3 dir, int cell, float delay = 3f){
		DailyBlock block = currentBlock;
		tutorialLeft.color = redText;
		tutorialRight.color = redText;
		if(cell == 0){
			tutorialLeft.color = greenText;
		}
		else{
			tutorialRight.color = greenText;
		}
		lightConfig.Config(up,dir,cell,block,delay);

		yield return new WaitForSeconds(delay);
		UnPause();
		dice.ShineFace(up,1);
		yield return new WaitForSeconds(1f);
		dice.ShineFace(dir,1);
		yield return new WaitForSeconds(1f);
		if(cell == 0){
			StartCoroutine(block.LCell.GetComponent<Cell>().shine(1));
		}
		else{
			StartCoroutine(block.DCell.GetComponent<Cell>().shine(1));
		}
		//UnPause();
	}

	public void HideTutorialPanel(){
		//panelUITutorial.GetComponent<TweenAlpha>().PlayReverse();
		tutorialLeftScale.PlayReverse();
		tutorialRightScale.PlayReverse();
		if(newTutorialIndex < 4 && newTutorialIndex != 2)
			tutorialBottom.PlayReverse();
	}

	IEnumerator TutorialEndCell(){
		Vector3 pos = new Vector3(-2,-0.1f,0);
		GameObject aux = (GameObject)Instantiate(cellEnd, new Vector3(pos.x, pos.y - 8f, pos.z), cellEnd.transform.rotation);
		Cell cell = aux.GetComponent<Cell>();
		//c.Init(sum);
		//Debug.Log(numbers[0] + numbers[2]);
		float posAux = pos.y;
		float currentPos = cell.transform.position.y;
		while(currentPos < posAux){
			currentPos += 0.4f;
			cell.transform.position = new Vector3(cell.transform.position.x, currentPos, cell.transform.position.z);
			yield return new WaitForSeconds(Time.deltaTime);
		}
		cell.transform.position = new Vector3(cell.transform.position.x, posAux, cell.transform.position.z);
		UnPause();
	}

    void loadNextScene(string s)
    {
        VariablesGlobales.nextScene = s;
        GameObject.FindWithTag("loading").GetComponent<LoadSceneWait>().enabled = true;
    }

    public void NextTutorial(bool b){
	}

	IEnumerator tutorialPause(bool b){
		float pause = 0f;
		if(tutorialIndex < 2)
			pause = 2f;
		else
			pause = 0.5f;
		yield return new WaitForSeconds(pause);
		/*if(b)
			NextTutorial(false);
		else
			UnPause();*/
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

	void ResetDiceNumbers(){
		Debug.Log("RESET");
		dice.transform.Find ("TextUp").GetComponent<TextMesh> ().text = "" + Random.Range(2,7);
		dice.transform.Find ("TextLeft").GetComponent<TextMesh> ().text = "" + Random.Range(1,7);
		dice.transform.Find ("TextForward").GetComponent<TextMesh> ().text = "" + Random.Range(1,7);
		dice.transform.Find ("TextDown").GetComponent<TextMesh> ().text = "" + Random.Range(2,7);
		dice.transform.Find ("TextRight").GetComponent<TextMesh> ().text = "" + Random.Range(2,7);
		dice.transform.Find ("TextBackward").GetComponent<TextMesh> ().text = "" + Random.Range(1,7);
	}
    
    public void hintPressed()
    {
        connectionProblem.SetActive(false);
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
                if (appodealDemo != null)
                    appodealDemo.showRewardedVideo(gameObject);
                
                //hintScreen.SendMessage ("PlayForward");
				Pause ();
                
			} else {
				StartCoroutine (lightPath (2));
				hintsAvailable--;
				hintIndicator.text = "" + hintsAvailable;
				PlayerPrefs.SetInt ("hints", hintsAvailable);
			}
			/*#if !UNITY_EDITOR
			Analytics.CustomEvent ("hints");
			#endif*/
		}
	}
    
    private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
				if(daily){
					PlayerPrefs.SetInt("triesLeft",PlayerPrefs.GetInt("triesLeft") + 2);
					triesLabel.text = PlayerPrefs.GetInt("triesLeft",0).ToString();
                    playAgainDaily();
				}
				else{
					hintsAvailable += 2;
					PlayerPrefs.SetInt ("hints", hintsAvailable);
					hintIndicator.text = "" + hintsAvailable;
                    closeHintScreen();
                }

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
                connectionProblem.SetActive(true);
                closeHintScreen();
                break;
		}
	}

	public void closeHintScreen(){
		hintScreen.SendMessage ("PlayReverse");
		UnPause ();
	}

	public void EvaluarDificultad(string [] cuadros, string [] path){
		int count = 0;
		for(int i = 0; i < cuadros.Length; i++){
			//Debug.Log(cuadros[i]);(
			string aux = cuadros[i].Trim();
			int auxI = 0;
			if(aux != ""){
				auxI = int.Parse(aux);
				if(auxI != 0 && auxI != 9)
				{
					//Debug.Log(cuadros[i].Trim());
					count++;
				}
			}
		}
		Debug.Log("Cantidad cuadros: "+count);
		Debug.Log("Largo ruta: "+(path.Length + 1));
	}

	public void componerEscena(){
		string completo = "";
        if (SceneManager.GetActiveScene().name == "InGameTest")
        {
            completo = PlayerPrefs.GetString("SceneTest");
        }
        else
        {
            int level = GlobalVariables.getSceneIndex(PlayerPrefs.GetString("scene", "Scene1"));
            //print(Mathf.Clamp(int.Parse(level.Substring(5, level.Length - 5)) - 1, 1, 100000) + " " + GlobalVariables.Scene[0]);
            completo = GlobalVariables.Scene[Mathf.Clamp(level, 0, 100000)];
            if (tutorial)
            {
                switch (tutorialMode)
                {
                    case 1:
                        completo = GlobalVariables.Scene91;
                        break;
                    case 2:
                        completo = GlobalVariables.Scene92;
                        break;
                    case 3:
                        completo = GlobalVariables.Scene93;
                        break;
                    case 4:
                        completo = GlobalVariables.Scene94;
                        break;
                }
            }
        }
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

        if (SceneManager.GetActiveScene().name == "InGameTest")
        {
            completoNumbers = PlayerPrefs.GetString("SceneNumbersTest");
        }
        else
        {
            int level = GlobalVariables.getSceneIndex(PlayerPrefs.GetString("scene", "Scene1"));
            completoNumbers = GlobalVariables.SceneNumbers[Mathf.Clamp(level, 0, 100000)];
            if (tutorial)
            {
                switch (tutorialMode)
                {
                    case 1:
                        completoNumbers = GlobalVariables.Scene91Numbers;
                        break;
                    case 2:
                        completoNumbers = GlobalVariables.Scene92Numbers;
                        break;
                    case 3:
                        completoNumbers = GlobalVariables.Scene93Numbers;
                        break;
                    case 4:
                        completoNumbers = GlobalVariables.Scene94Numbers;
                        break;
                }
            }
        }
		string completoPath = "";

        if (SceneManager.GetActiveScene().name == "InGameTest")
        {
            completoPath = PlayerPrefs.GetString("ScenePathTest");
        }
        else
        {
            int level = GlobalVariables.getSceneIndex(PlayerPrefs.GetString("scene", "Scene1"));
            completoPath = GlobalVariables.ScenePath[Mathf.Clamp(level, 0, 100000)];
            if (tutorial)
                completoPath = GlobalVariables.Scene91Path;
        }
		string [] auxPath = completoPath.Split (new char[1]{ '|' });
		string[] auxCoord = new string[2];
		for (int i = 0; i < auxPath.Length; i++) {
			auxCoord = auxPath [i].Split (new char[1]{ ',' });
			Vector2 coord = new Vector2 (float.Parse (auxCoord [0]), float.Parse (auxCoord [1]));
			path.Add (coord);
		};

		//Evaluar
		EvaluarDificultad(arreglo,auxPath);

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
                case 10:
                    g = (GameObject)Instantiate(cellSpikes, new Vector3(j, -0.1f, -i) - posIni, Quaternion.identity);
                    break;
                case 11:
                    g = (GameObject)Instantiate(cellChangeNumber, new Vector3(j, -0.1f, -i) - posIni, Quaternion.identity);
                break;
                    case 12:
                    g = (GameObject)Instantiate(cellDissapearNumber, new Vector3(j, -0.1f, -i) - posIni, Quaternion.identity);
                    break;
            }
            if (g != null) {
                    if (arregloNumbers[indice].Contains("*"))
                    {
                        g.GetComponent<Cell>().SetNumberApprox(int.Parse(arregloNumbers[indice].Remove(0, 2)));
                    }
                    else {
                        g.GetComponent<Cell>().number = int.Parse(arregloNumbers[indice]);
                    }
					g.transform.parent = rootCells;
					cellArray [i,j] = g;
				}
				indice++;
			}
		}

        GameObject limitParent = GameObject.Find("Limits");
        if (limitParent != null)
        {
            limitRight = GameObject.Find("LimitRight");
            limitRight.transform.position = new Vector3(0.5f, 0f, 0.5f);
            limitRight.transform.localScale = new Vector3(1f, 1f, int.Parse(info[0]));
            limitLeft = GameObject.Find("LimitLeft");
            limitLeft.transform.position = new Vector3(-int.Parse(info[1]) + 0.5f, 0f, 0.5f);
            limitLeft.transform.localScale = new Vector3(1f, 1f, int.Parse(info[0]));
            limitUp = GameObject.Find("LimitUp");
            limitUp.transform.position = new Vector3(0.5f, 0f, 0.5f);
            limitUp.transform.localScale = new Vector3(int.Parse(info[1]), 1f, 1f);
            limitDown = GameObject.Find("LimitDown");
            limitDown.transform.position = new Vector3(0.5f, 0f, - int.Parse(info[0]) + 0.5f);
            limitDown.transform.localScale = new Vector3(int.Parse(info[1]), 1f, 1f);

            limitParent.transform.position = new Vector3((int.Parse(info[1]) - int.Parse(info[2])) - 1f, 0f, int.Parse(info[3]));
        }
    }

    public void backToCreator()
    {
        SceneManager.LoadScene("LevelCreator");
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
            if(!daily)
			    pauseTime += Time.timeSinceLevelLoad - pauseAux;
			pauseAux = 0;
		}
	}

	private IEnumerator TutorialFall(){
		dice.ExecAnim("BadMove");
		yield return new WaitForSeconds(1.2f);
		while(panelTransicion.alpha < 1){
			panelTransicion.alpha += 0.1f;
			yield return new WaitForSeconds(0.01f);
		}
		SceneManager.LoadScene("NewTutorial");
		/*dice.ExecAnim("Idle");
		//dice.transform.eulerAngles = Vector3.zero;
		yield return new WaitForSeconds(0.5f);
		dice.ResetNumberPositions();
		dice.RestoreLastRotation();
		while(panelTransicion.alpha > 0){
			panelTransicion.alpha -= 0.1f;
			yield return new WaitForSeconds(0.01f);
		}
		UnPause();
		ShowCurrentTutorial();*/
	}

	private IEnumerator TutorialFail(){
		dice.ExecAnim("BadMove");
		yield return new WaitForSeconds(0.8f);
		while(panelTransicion.alpha < 1){
			panelTransicion.alpha += 0.1f;
			yield return new WaitForSeconds(0.01f);
		}
		dice.ExecAnim("Idle");
		dice.RollBack();
		//dice.transform.eulerAngles = Vector3.zero;
		yield return new WaitForSeconds(1f);
		dice.ResetNumberPositions();
		while(panelTransicion.alpha > 0){
			panelTransicion.alpha -= 0.1f;
			yield return new WaitForSeconds(0.01f);
		}
		UnPause();
		if(newTutorialIndex < 4){
			tutorialLeftScale.PlayForward();
			tutorialRightScale.PlayForward();
			StartCoroutine(LightDice(lightConfig.up,lightConfig.dir,lightConfig.cell,1f));
		}
		
	}

	public bool calculateResult(int diceValueA, int diceValueB, int cellValue, Cell cell = null){
		print ("calculating");
		if (checkOperationResult (diceValueA, diceValueB) != cellValue) {
			if(tutorial){
				StartCoroutine(TutorialFail());
			}
			else if(daily){
				DailyAnswer(false);
				componerEscena_Daily();
			}
			else
				badMove ();
		} else {
			cell.changeState (Cell.StateCell.Passed);
			audio.pitch = Random.Range (0.95f, 1.05f);
			audio.PlayOneShot(audioGoodMove);
			if(path.Count > 0)
				path.RemoveAt (0);
			Instantiate (dice.goodMove, new Vector3(dice.transform.position.x,dice.transform.position.y + diceSize, dice.transform.position.z), Quaternion.LookRotation(Vector3.up));
			if(tutorial){
				HideTutorialPanel();
				if(unPauseOnMove)
					UnPause();
				componerEscena_Tutorial();
			}
			else if(daily){
				DailyAnswer(true);
				componerEscena_Daily();
			}
		}
		return (checkOperationResult (diceValueA, diceValueB) == cellValue);
	}

	public void badMove(){
		/*#if !UNITY_EDITOR
			Analytics.CustomEvent ("badMove", new Dictionary<string, object> {
			{ "scene", PlayerPrefs.GetString("scene", "Scene1") },
			{ "steps", dice.steps },
			{ "place", dice.gameObject.transform.position},
			{ "time", secondsAvailable - Time.timeSinceLevelLoad }
			});
		#endif*/
		if(tutorial){
			Pause();
			StartCoroutine(TutorialFall());
			return;
		}
        if (Random.Range(0, 100) < 30)
        {
            if (appodealDemo != null)
                appodealDemo.showInterstitial();
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
        dice.ExecAnim("BadMove");
    }

	IEnumerator reloadScene(){
		yield return new WaitForSeconds (1f);
		timesDied++;
		PlayerPrefs.SetInt ("timesDied", timesDied);
		PlayerPrefs.SetFloat("totalTime",totalTime + Time.timeSinceLevelLoad);
        loadNextScene(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	//0: adyacente, 1: tres adyacentes, 2: todos
	public IEnumerator lightPath(int mode){
		if (!pause && !daily) {
			Pause ();
			mode = Mathf.Clamp (mode, 0, 2);
			switch (mode) {
			case 0:
				//dice.EnableTutorialSign(true);
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
                    /*
				foreach (Vector2 v in path) {
					StartCoroutine (cellArray [(int)v.x, (int)v.y].GetComponent<Cell> ().shine (1));
					yield return new WaitForSeconds (1f / 2);
				}*/
                    print("PROBLEMA CON PATH CUANDO UNO SE MUEVE Y NO HA TERMINADO DE MOSTRAR");
                for(int i = path.Count - 1; i == 0; i--)
                {
                    StartCoroutine(cellArray[(int)(((Vector2)path[i]).x), (int)(((Vector2)path[i]).y)].GetComponent<Cell>().shine(1));
                    yield return new WaitForSeconds(1f / 2);
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
		PlayerPrefs.SetFloat("totalTime",0);

        finished = true;
		print("Finished");
		//HideTutorial();
		Pause ();
		foreach (Transform t in adjacentCells)
			t.GetComponent<AdjacentCellFinder> ().EnableCell (false);
		//for (int i = 0; i < tutorialClips.Length; i++)
		if(tutorialv2.gameObject.activeSelf)
			dice.EnableTutorialSign(false);

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
        if(PlayerPrefs.GetString("scene") == "Scene0")
        {
            PlayerPrefs.SetString("scene", GlobalVariables.getIndexScene("1"));
            PlayerPrefs.SetInt("unlocked" + GlobalVariables.getIndexScene("1"), 1);
        }
        if (daily)
        {
            PlayerPrefs.SetInt("lvlSelectDaily", 0);
        }

        if (tutorial)
        {
			tutorialBottom.PlayReverse();
            /*int levelS = GlobalVariables.getSceneIndex(PlayerPrefs.GetString("scene", "Scene1")) + 1; //int.Parse(PlayerPrefs.GetString("scene", "Scene1").Split(new char[1] { 'e' })[2]) + 1;//// (int.Parse (num) + 1);

            if (levelS < GlobalVariables.nLevels)
            {
                //PlayerPrefs.SetInt("unlockedScene" + levelS, 1);
                PlayerPrefs.SetInt("unlocked" + GlobalVariables.getIndexScene("" + levelS), 1);
                PlayerPrefs.SetString("scene", (GlobalVariables.getIndexScene("" + levelS)));
            }*/
			StartCoroutine(finishDaily());
			Debug.Log("Finished Tutorial");
            return;
        }
        if (stageTime != null)
            stageTime.text = clockShow.text;
        string level = PlayerPrefs.GetString("scene", "Scene1");
        string completo = GlobalVariables.Scene[GlobalVariables.getSceneIndex(level)];
        string[] aux = completo.Split(new char[1] { '$' })[0].Split(new char[1] { '|' });
		string stage = aux[5].Trim();
        //if (!daily && Time.timeSinceLevelLoad - pauseTime < PlayerPrefs.GetFloat("record" + PlayerPrefs.GetString("scene", "Scene1"), float.MaxValue))
        if (!daily && Time.timeSinceLevelLoad - pauseTime < PlayerPrefs.GetFloat("record" + stage, float.MaxValue))
        {
            newRecordSign.SetActive(true);
			Debug.Log("record"+stage);
            PlayerPrefs.SetFloat("record" + stage, Time.timeSinceLevelLoad - pauseTime);
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
		
		/*#if !UNITY_EDITOR
		Analytics.CustomEvent ("Finish"+PlayerPrefs.GetString("scene", "Scene1"), new Dictionary<string, object> {
			{ "steps", dice.steps },
			{ "time", secondsAvailable - Time.timeSinceLevelLoad }
		});
		#endif*/



		if(daily){
			UpdateConsecutiveDays();
			StartCoroutine(finishDaily());
		}
		else{
			Debug.Log("skill");
            //string texto = GlobalVariables.getSceneName(PlayerPrefs.GetString("scene", "Scene1"));
            //string num = texto.Split (new char[1]{ 'e' }) [2];
            // test1 int levelS = GlobalVariables.getSceneIndex(PlayerPrefs.GetString("scene", "Scene1")) + 1 + 1; //int.Parse(PlayerPrefs.GetString("scene", "Scene1").Split(new char[1] { 'e' })[2]) + 1;//// (int.Parse (num) + 1);
            int levelS = GlobalVariables.getSceneIndex(PlayerPrefs.GetString("scene", "Scene1")) + 1 + 1;

            if (levelS < GlobalVariables.nLevels)
            {
                //PlayerPrefs.SetInt("unlockedScene" + levelS, 1);
                PlayerPrefs.SetInt("unlocked" + GlobalVariables.getIndexScene("" + levelS), 1);
                PlayerPrefs.SetString("scene", (GlobalVariables.getIndexScene("" + levelS)));
            }
            //        if (levelS < GlobalVariables.nLevels)
            //PlayerPrefs.SetInt ("unlockedScene" + levelS, 1);

            Debug.Log(LevelSelection.LevelSkillTotal());
			StartCoroutine(moveSlider(dailySlider, LevelSelection.LevelSkillTotal() + PlayerPrefs.GetFloat("totalDaily",0)/2f));
		}

		Analytics_Finish();
		
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
			res = (Mathf.RoundToInt((diceValueA * 1f) / (diceValueB * 1f)));
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
                    if (appodealDemo != null)
                        appodealDemo.showBanner(Appodeal.BANNER_BOTTOM);
                }
			}
		}
	}

	IEnumerator finishDaily(){
		if(!tutorial)
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
			Analytics.CustomEvent ("timesUp"+PlayerPrefs.GetString("scene", "Scene1"));
		#endif
	}

	public void playAgain(){
        //Appodeal.hide(Appodeal.BANNER_BOTTOM);
        PlayerPrefs.SetString("scene", currentSceneText);
        if (appodealDemo != null)
            appodealDemo.hideBanner();
        loadNextScene(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene (SceneManager.GetActiveScene().name);
    }

    public void skipTutorial(){
        //PlayerPrefs.SetInt("SeenTutorial",1);
        //playAgain();
        print("skip " + PlayerPrefs.GetInt("scene"));
        if (appodealDemo != null)
            appodealDemo.hideBanner();
        loadNextScene("LevelSelection");
        //SceneManager.LoadScene("LevelSelection");
    }

    public void playTutorial(){
        //Appodeal.hide(Appodeal.BANNER_BOTTOM);
        if (appodealDemo != null)
            appodealDemo.hideBanner();
        loadNextScene("InGame_tutorial");
        //SceneManager.LoadScene ("InGame_tutorial");
    }

    public void exit()
    {
        //Appodeal.hide(Appodeal.BANNER_BOTTOM);
        if (appodealDemo != null)
            appodealDemo.hideBanner();
        /*string texto = PlayerPrefs.GetString ("scene", "Scene1");
		string num = texto.Split (new char[1]{ 'e' }) [2];
		int level = (int.Parse (num) + 1);
		if(level < GlobalVariables.nLevels)
			PlayerPrefs.SetInt ("unlockedScene" + level, 1);*/
        /*if(tutorial && PlayerPrefs.GetInt("SeenTutorial",0) == 0)
			PlayerPrefs.SetInt("SeenTutorial",1);*/
        string texto = GlobalVariables.getSceneName(PlayerPrefs.GetString("scene", "Scene1"));
        string num = "1";
        if (texto != "InGame_tutorial" && texto != "TUTORIAL")
            num = texto.Split(new char[1] { 'e' })[2];
			/*
		#if !UNITY_EDITOR
				Analytics.CustomEvent ("backToselection" + num);
		#endif
		*/
        int level = Mathf.Clamp((int.Parse(num)), 1, 1000);
        //Debug.Log()
        //if(!tutorial)
        //level += 1;
        //else
        //if(PlayerPrefs.GetInt("SeenTutorial",0) == 0)
        //	PlayerPrefs.SetInt("SeenTutorial",1);
        //PlayerPrefs.SetString("scene", "Scene" + level);
		if(!finished){
			Analytics_Exit();
		}
        loadNextScene("LevelSelection");
        //SceneManager.LoadScene ("LevelSelection");
    }

    public void exitGame()
    {
        //Appodeal.hide(Appodeal.BANNER_BOTTOM);
        if (appodealDemo != null)
            appodealDemo.hideBanner();
        Application.Quit();
    }

	public void ExitToMenu(){
		SceneManager.LoadScene("LevelSelection");
	}


	public void next()
    {
        //Appodeal.hide(Appodeal.BANNER_BOTTOM);
        if (appodealDemo != null)
            appodealDemo.hideBanner();
        string texto = PlayerPrefs.GetString("scene", "Scene1");
        int num = 1;
        if (texto != "InGame_tutorial" && texto != "TUTORIAL")
            num = GlobalVariables.getSceneIndex(texto);// texto.Split (new char[1]{ 'e' }) [2];
        /*#if !UNITY_EDITOR
		Analytics.CustomEvent ("enteringLevel" + num);
#endif*/
        int level = num + 1 + 1;
        if (tutorial && level == 2) level = 1;
		//Debug.Log()
		//if(!tutorial)
		
		//else
			//if(PlayerPrefs.GetInt("SeenTutorial",0) == 0)
			//	PlayerPrefs.SetInt("SeenTutorial",1);

		if (level > GlobalVariables.nLevels)
			exit ();
		else {
			PlayerPrefs.SetInt ("timesDied", 0);
			PlayerPrefs.SetFloat("totalTime",0);
			//PlayerPrefs.SetInt ("unlockedScene" + level, 1);
			/*if(!tutorial && (level - 1) % 5 == 1){
				//LevelSelection.CheckTutorial(level);
			}
			else{
            */
                //PlayerPrefs.SetInt("unlocked" + GlobalVariables.getIndexScene("" + level), 1);
                //PlayerPrefs.SetString("scene", (GlobalVariables.getIndexScene("" + level)));
                //PlayerPrefs.SetString ("scene", "Scene" + level);
                loadNextScene("InGame");
                //SceneManager.LoadScene ("InGame");
            //}
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Destroy (bgm.gameObject);
            //Appodeal.hide(Appodeal.BANNER_BOTTOM);
            if (appodealDemo != null)
                appodealDemo.hideBanner();
            loadNextScene("LevelSelection");
            //SceneManager.LoadScene ("LevelSelection");
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
				//Debug.Log("counting");
				
				//int minutes = (int)((60 - Time.timeSinceLevelLoad + pauseTime + extraSeconds) / 60);
				int seconds = (int)((60 - Time.timeSinceLevelLoad + pauseTime + extraSeconds));
				//Debug.Log(seconds);
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
		else if (!adjacentCells.gameObject.activeSelf && !finished && !tutorial)
			adjacentCells.gameObject.SetActive (true);
				

		adjacentCells.position = dice.transform.position;

		if(tutorial){
			if(Input.GetKeyDown(KeyCode.C)){
				badMove();
			}
		}
	}

	public static void GetConsecutiveDays(){
		//load last played date
		int consecutiveDays = PlayerPrefs.GetInt("consecutiveDays",-1);
		System.DateTime lastPlayedDate = System.DateTime.Parse(PlayerPrefs.GetString("lastPlayedDate",System.DateTime.Now.Date.ToString()));
		int daysSinceLastPlay = (int)(System.DateTime.Now - lastPlayedDate).TotalDays;
		if(daysSinceLastPlay == 0){
            if (consecutiveDays == -1)
            {
                Debug.Log("First stage played");
                PlayerPrefs.SetInt("consecutiveDays", 1);
            }
            else
            {
                Debug.Log("Already played today");
            }
		}
		if(daysSinceLastPlay == 1){
            print("sumando dias 2");
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
        /*
		System.DateTime lastPlayedDate = System.DateTime.Parse(PlayerPrefs.GetString("lastPlayedDate",System.DateTime.Now.Date.ToString()));
		int daysSinceLastPlay = (int)(System.DateTime.Now - lastPlayedDate).TotalDays;
		if(daysSinceLastPlay == 1){
			int consecutiveDays = PlayerPrefs.GetInt("consecutiveDays");
			consecutiveDays = Mathf.Clamp(consecutiveDays + 1,0,7);
			PlayerPrefs.SetString("lastPlayedDate",System.DateTime.Now.Date.ToString());
			PlayerPrefs.SetInt("consecutiveDays",consecutiveDays);
            print("sumando dias " + consecutiveDays);
        }
        */
	}

	void componerEscena_Daily(){
		currentBlock.DropRemainingBlocks();
		GameObject aux = (GameObject)Instantiate(baseBlock, new Vector3(dice.transform.position.x, 0f, dice.transform.position.z), currentBlock.transform.rotation);
		DailyBlock block = aux.GetComponent<DailyBlock>();
		currentBlock.currentNumbers = dice.faceNumbers();
		Dice.Operation operation = dice.currentOperation;
		if(currentBlock.currentNumbers[0] > 100 || currentBlock.currentNumbers[0] * currentBlock.currentNumbers[1] >= 100 || currentBlock.currentNumbers[0] * currentBlock.currentNumbers[2]  >= 100 || currentBlock.currentNumbers[0] == 0 || Mathf.Abs(currentBlock.currentNumbers[0]) > 10 && (Mathf.Abs(currentBlock.currentNumbers[1]) > 10 || Mathf.Abs(currentBlock.currentNumbers[2]) > 10)){
			Debug.Log("here");
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
		else
        {
            //extraSeconds = Mathf.Clamp(extraSeconds -= 5f, -10f, 25f);
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

        if (consec == -1)
            PlayerPrefs.SetInt("consecutiveDays", 0);
        else
        {
            //print("sumando dias 3");
            //PlayerPrefs.SetInt("consecutiveDays", (int)consec + 1);
        }

		Debug.Log(result + ", "+ totalPercentage);
		if(result > totalPercentage)
			totalPercentage = (totalPercentage + result * 1.1f)/2f;
		else if(result < totalPercentage)
			totalPercentage = (totalPercentage - (1 - result) * 0.05f);

        //completa el dia correspondiente en el calendario
        print(totalPercentage + " " + (int)(Mathf.Clamp01(totalPercentage) * 100f));
       

        PlayerPrefs.SetFloat("totalDaily",Mathf.Clamp01(totalPercentage));
		PlayerPrefs.SetString("lastPlayedDate",System.DateTime.Now.Date.ToString());
		//dailySlider.value = LevelSelection.LevelSkillTotal() + totalPercentage/2f;
		PlayerPrefs.SetInt("triesLeft",Mathf.Clamp(PlayerPrefs.GetInt("triesLeft") - 1,0,int.MaxValue));
		if(triesLabel != null) triesLabel.text = PlayerPrefs.GetInt("triesLeft",0).ToString();
        print(LevelSelection.LevelSkillTotal() + totalPercentage / 2f);
        print(PlayerPrefs.GetInt("todayRecord") + " < " + (int)(result * 100f));
        newRecordSign.SetActive(PlayerPrefs.GetInt("todayRecord") < (int)(result * 100f));
        PlayerPrefs.SetInt("dailyScore", (int)(result *  100f));
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

	void OnApplicationQuit(){
		Analytics_Exit();
	}

	private void Analytics_Exit(){
		#if !UNITY_EDITOR
		if(tutorial){
			Analytics.CustomEvent("QuitTutorial");
		}
		else if(daily){
			Analytics.CustomEvent("QuitDaily");
		}
		else if(!daily && !tutorial){
			Analytics.CustomEvent("Quit"+PlayerPrefs.GetString("scene","Scene1"),new Dictionary<string,object>{
				{"sceneTime",Time.timeSinceLevelLoad}
				{"totalSceneTime",totalTime},
			});
		}
		#endif
	}

	void Analytics_Start(){
		#if !UNITY_EDITOR
		if(tutorial){
			Analytics.CustomEvent("StartedTutorial");
		}
		else if(daily){
			Analytics.CustomEvent("StartedDaily");
		}
		else if(!daily && !tutorial){
			Analytics.CustomEvent("Started"+PlayerPrefs.GetString("scene","Scene1"));
		}
		#endif
	}

	void Analytics_Finish(){
		#if !UNITY_EDITOR
		if(tutorial){
			Analytics.CustomEvent("FinishedTutorial");
		}
		else if(daily){
			Analytics.CustomEvent("FinishedDaily");
		}
		else if(!daily && !tutorial){
			Analytics.CustomEvent("Finished"+PlayerPrefs.GetString("scene","Scene1"),new Dictionary<string,object>
			{
				{"sceneTime",Time.timeSinceLevelLoad}
				{"totalSceneTime",totalTime},
			});
		}
		#endif
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

    public void playAgainDaily(){
		int tries = PlayerPrefs.GetInt("triesLeft",0);
		if(tries > 0){
			Debug.Log("again");
            loadNextScene("LevelSelection");
            //SceneManager.LoadScene("LevelSelection");
            PlayerPrefs.SetInt("lvlSelectDaily", 1);
        }
		else
        {
            loadNextScene("LevelSelection");
            //SceneManager.LoadScene("LevelSelection");
            PlayerPrefs.SetInt("lvlSelectDaily", 2);
            /*
#if !UNITY_EDITOR
            if (appodealDemo != null)
                appodealDemo.showRewardedVideo(gameObject);
#else
            HandleShowResult(ShowResult.Finished);
#endif
            Debug.Log("showing video");*/
        }
    }
}
