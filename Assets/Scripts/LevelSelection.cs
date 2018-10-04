using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Analytics;
using UnityEngine.Advertisements;

//using VoxelBusters.NativePlugins;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;


public class LevelSelection : MonoBehaviour
{
	public bool testing = true;
    public GameObject buttonBase;
    public UIGrid grid;
    UIPanel gridPanel;
    TweenAlpha panelTween;
    public int buttonsPerPage;
    public int firstInPage = 0;
    int page = 0;
    Resolution resolution;
    List<GameObject> currentButtons;
	public UIButton[] recordButtons;
	public UIPanel panel;
	public GameObject loading;
	public UISprite muteButton;
	public UISprite controlButton;
	public UISlider mathSkillSlider;
	public UISlider consecDaysSlider;
	public UILabel triesNumber;
	public TweenAlpha rewardsPanel;
    public TweenAlpha calendarPanel;
    public GameObject medals_GO;
	public UISprite [] medals;
	public UILabel consecDays;
	public UIPanel dailyPanel;
    public GameObject dailyButton;
    public GameObject startDaily;
	public TweenAlpha swipeIcon;
	public UIScrollView dragLevels;
	bool dragged;
    float swipeInitialTime;
    Vector3 swipeInitialPos;
    public GameObject connectionProblem;
    AppodealDemo appodealDemo;
    public bool autoFlip = false;

    void Awake(){
        gridPanel = grid.GetComponentInParent<UIPanel>();
        panelTween = gridPanel.GetComponent<TweenAlpha>();
    }
    void Start ()
    {
        resolution = Screen.currentResolution;
        Debug.Log(resolution);
        firstInPage = 0;
        currentButtons = new List<GameObject>();
        //PlayerPrefs.DeleteAll();
        /*
        PlayerPrefs.SetString("scene", "Scene1");
        PlayerPrefs.SetInt("unlockedScene5", 1);
        PlayerPrefs.SetInt("unlockedScene3", 1);
        PlayerPrefs.SetInt("unlockedScene1", 1);
        PlayerPrefs.SetInt("unlockedScene4", 1);
        PlayerPrefs.SetInt("unlockedScene6", 1);
        PlayerPrefs.SetFloat("recordScene5", 10f);
        PlayerPrefs.SetFloat("recordScene3", 11f);
        PlayerPrefs.SetFloat("recordScene1", 12f);
        PlayerPrefs.SetFloat("recordScene4", 14f);
        */
        GlobalVariables.SetScenes();
        if (false) // !GlobalVariables.finishOrderingProcess)
        {
            GlobalVariables.SetScenes();
            //GlobalVariables.orderScenes();
            for (int i = 0; i < GlobalVariables.nLevels; i++)
                print(GlobalVariables.getIndexScene("" + (i + 1)));
        }
        
        if(GameObject.Find("AppoDeal") != null)
            appodealDemo = GameObject.Find("AppoDeal").GetComponent<AppodealDemo>();

        InitMedals();

        PlayerPrefs.SetInt ("continueBGM", 0);
		if (PlayerPrefs.GetInt ("unlocked" + GlobalVariables.getIndexScene("1")) != 1) {
			PlayerPrefs.SetInt ("unlocked" + GlobalVariables.getIndexScene("1"), 1);
		}
		//PlayerPrefs.DeleteAll ();

		if (PlayerPrefs.GetInt ("Mute") == 1)
			Mute ();

		if (PlayerPrefs.GetInt ("Control") == 1) {
			controlButton.spriteName = "tap";
			controlButton.GetComponent<UIButton> ().normalSprite = "tap";
		}

        //calcula la posicion del scroll
        if (PlayerPrefs.GetString ("scene") != "") {
			string texto = PlayerPrefs.GetString ("scene", "Scene1");
            int num = 1;
            if (texto != "InGame_tutorial")
            {
                num = GlobalVariables.getSceneIndex(texto) + 1;//GlobalVariables.getSceneName(texto).Split(new char[1] { 'e' })[2];
                print(texto);
                //num = texto.Split(new char[1] { 'e' })[2];
            }
			int level = num;
            print("levellll " + level);
            int textoLevel = int.Parse(texto.Substring(5,texto.Length-5));
            int auxIndex = 0;
            CalculateGrid();
            while(auxIndex + (grid.maxPerLine * 2) < textoLevel){
                auxIndex += (grid.maxPerLine * 2);
            }
            Debug.Log(auxIndex);
            firstInPage = auxIndex;
            page = firstInPage / (grid.maxPerLine * 2);
            //while(auxIndex < texto.)
            panel.transform.localPosition = new Vector3 (-400 * level, panel.transform.position.y, panel.transform.position.z);
			panel.clipOffset = new Vector2 (400 * level, panel.clipOffset.y);
		}

		PlayerPrefs.SetInt ("timesDied", 0);
        //activa o desactiva los botones
        /*for (int i = 0; i < GlobalVariables.nLevels; i++)
        {
            int t = (GlobalVariables.getSceneIndex("Scene" + (i + 1)));
            print("boton" + t + " " + PlayerPrefs.GetInt("unlockedScene" + (i + 1), 0));
            
            if (PlayerPrefs.GetInt("unlockedScene" + (i + 1), 0) != 1)
            {
                recordButtons[t].isEnabled = false;
                print("locked " + t);
			}
            else
            {
                print("unlocked " + t);
            }
		}*/
		
        //escribe los tiempos en cada boton
		/*for (int i = 0; i < GlobalVariables.nLevels; i++) {
            int t = (GlobalVariables.getSceneIndex("Scene" + (i + 1))); 
			//float recordSeconds = PlayerPrefs.GetFloat ("recordScene" + (GlobalVariables.getSceneIndex("Scene" + (i + 1))), -1f);
            float recordSeconds = PlayerPrefs.GetFloat("recordScene" + (i + 1), -1f);
            recordButtons[t].transform.Find("record").gameObject.SetActive(recordSeconds > 0);
			if (recordSeconds > 0) {
				int minutes = (int)((recordSeconds) / 60);
				int seconds = (int)((recordSeconds) % 60);
				int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
				recordButtons[t].transform.Find("record").GetComponent<UILabel>().text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
			}
		}*/


        //new grid
        ResetGridButtons(firstInPage);

        int consecutiveDays = PlayerPrefs.GetInt("consecutiveDays", 0);
        System.DateTime date = System.DateTime.Now.Date;
        //date = System.DateTime.Parse("12/28/2017 12:00:00 AM");
        System.DateTime lastPlayedDate = System.DateTime.Parse(PlayerPrefs.GetString("lastPlayedDate", date.ToString()));
        int daysSinceLastPlay = (int)(date - lastPlayedDate).TotalDays;
        print(lastPlayedDate + " " + date + " " + daysSinceLastPlay + " " + consecutiveDays);
        if (daysSinceLastPlay > 0)
        {
            PlayerPrefs.SetInt("triesLeft", 1);
        }
        if (daysSinceLastPlay == 0)
        {
            if (consecutiveDays == -1)
            {
                Debug.Log("First stage played");
            }
            else
            {
                Debug.Log(date);
                Debug.Log("Already played today");
            }
        }
        else
        {
            if (daysSinceLastPlay == 1)
            {
                PlayerPrefs.SetFloat("totalDaily", 0);
                print("aumentando consecutive days");
                //if (consecutiveDays < 0) consecutiveDays = 0;
                consecutiveDays++;
                PlayerPrefs.SetInt("consecutiveDays", consecutiveDays);
            }
            else
            {
                if (daysSinceLastPlay > 1)
                {
                    PlayerPrefs.SetFloat("totalDaily", 0);
                    PlayerPrefs.SetInt("consecutiveDays", 0);
                    Debug.Log("Haven't played in over a day");
                    consecDaysSlider.value = 0;
                    consecutiveDays = 0;
                }
            }
        }
        PlayerPrefs.SetString("lastPlayedDate", date.ToString());

        float penalization = 0;
        consecDays.text = consecutiveDays.ToString();
        if (consecutiveDays == 0)
        {
            consecDaysSlider.value = 0;
            penalization = -0.01f;
        }
        else if (consecutiveDays == -1)
        {
            consecDaysSlider.value = 0;
            consecDays.text = 0.ToString();
        }
        else
        {
            float aux = consecutiveDays;
            consecDaysSlider.value = Mathf.Clamp(aux, 0f, 7f) * (1f / 7f);
        }
        if (System.DateTime.Now.Date.ToString() == PlayerPrefs.GetString("lastLoadedDaily", ""))
        {
            penalization = 0;
        }

        PlayerPrefs.SetFloat("totalDaily", PlayerPrefs.GetFloat("totalDaily") + penalization);
        if(appodealDemo != null)
            appodealDemo.Init();
        showBanner();
        if (PlayerPrefs.GetInt("lvlSelectDaily",0) == 1){
            dailyPanel.GetComponent<TweenAlpha>().PlayForward();
            GetConsecutiveDays();
		}
        if (PlayerPrefs.GetInt("lvlSelectDaily", 0) == 2)
        {
            connectionProblem.SetActive(false);
            rewardsPanel.GetComponent<TweenAlpha>().PlayForward();
            GetConsecutiveDays();
        }
        
        GameObject bgm = GameObject.Find("BGM");
        if (bgm != null) Destroy(bgm);
        StartCoroutine(showSwipe());
        
    }

    void CalculateGrid(){
        //calcular filas + ajustar escala
        if(resolution.width > resolution.height){
            grid.maxPerLine = 5;
            grid.transform.localScale = new Vector3(1,1,1);
        }
        else{
            grid.maxPerLine = 3;
            grid.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
        }
        buttonsPerPage = grid.maxPerLine * 2;
    }

    void ResetGridButtons(int startIndex){
        //limpiar
        if(currentButtons.Count > 0)
            foreach(GameObject go in currentButtons)
                DestroyImmediate(go);
        currentButtons.Clear();
        CalculateGrid();
        //instanciar botones
        Random.InitState(25*page + 25 + 100*page);
        for(int i = startIndex; i < Mathf.Min(startIndex + buttonsPerPage,GlobalVariables.nLevels-1); i++){
            GameObject go = (GameObject)Instantiate(buttonBase,grid.transform.position,grid.transform.rotation,grid.transform);
            currentButtons.Add(go);
            //sprite color
            UISprite sprite = go.GetComponent<UISprite>();
            int random = Random.Range(0,60);
            Color aux = new Color();

            if(random < 10)
                sprite.spriteName = "Cubo1";
            else if(random < 20)
                sprite.spriteName = "Cubo2";
            else if(random < 30){
                sprite.spriteName = "Cubo1";
                // E35A73FF
                ColorUtility.TryParseHtmlString("#FD6A85FF",out aux);
                sprite.color = aux;
            }
            else if(random < 40){
                sprite.spriteName = "Cubo2";
                //528F94FF 9E9E9FFF
                ColorUtility.TryParseHtmlString("#B8B8BFFF",out aux);
                sprite.color = aux;
            }
            else if(random < 50){
                sprite.spriteName = "Cubo1";
                ColorUtility.TryParseHtmlString("#70D6DFFF",out aux);
                sprite.color = aux;
            }
            else if(random < 60){
                sprite.spriteName = "Cubo2";
                ColorUtility.TryParseHtmlString("#69D884FF",out aux);
                sprite.color = aux;
            }

            //int t = (GlobalVariables.getSceneIndex("Scene" + (i + 1)));
            UILabel nameLabel = go.transform.Find("Label").GetComponent<UILabel>();
            //button name
            nameLabel.text = "LVL" + (i+1).ToString();
            go.name = "Button"+(i+1).ToString();
            //button record
            //float recordSeconds = PlayerPrefs.GetFloat("recordScene" + (i + 1), -1f);
            float recordSeconds = PlayerPrefs.GetFloat ("recordScene" + (i+1), -1f);
            //Debug.Log((i+1) +" "+recordSeconds);
            UILabel recordLabel = go.transform.Find("record").GetComponent<UILabel>();
            recordLabel.gameObject.SetActive(recordSeconds > 0);
            if(recordSeconds > 0){
                int minutes = (int)((recordSeconds) / 60);
				int seconds = (int)((recordSeconds) % 60);
				int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
				recordLabel.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;
            }
            //button click
            EventDelegate.Set(go.GetComponent<UIButton>().onClick,() => launchLevel(nameLabel.text));
        }
        grid.Reposition();
        panelTween.PlayForward();
        panelTween.ResetToBeginning();

        /*for(int i = 1; i < 10; i++){
            Debug.Log("Scene "+i+": "+PlayerPrefs.GetFloat("recordScene"+GlobalVariables.getSceneIndex("Scene"+i)));
        }*/
    }


    /*public void test1()
    {

        print("test1");
        //if (GameObject.Find("AppoDeal") != null)
        showBanner();
        //GameObject.Find("AppoDeal").GetComponent<AppodealDemo>().showBanner(0);
    }

    public void test2()
    {
        print("test2");
        GameObject.Find("AppoDeal").GetComponent<AppodealDemo>().showRewardedVideo(null);
    }*/

    IEnumerator showSwipe(){
		yield return new WaitForSeconds(5f);
		if(!dragged)
			EnableSwipe();
	}

	public void showVideo(){

        if (appodealDemo != null)
            appodealDemo.showRewardedVideo(gameObject);
#if !UNITY_EDITOR
				Analytics.CustomEvent ("showVideo");
#else
        HandleShowResult(ShowResult.Finished);
#endif
    }

	string mensaje;
    

	private void HandleShowResult(ShowResult result)
	{
		int tries = 0;
		switch (result)
		{
		case ShowResult.Finished:
                tries += 2;
                //Es importante que no se ejecute instantaneo o hará que el juego se reinicie usar waitforendofframe
                StartCoroutine(rewardFromVideo(tries));
                Debug.Log("Rewarded video finished");
                mensaje += "Ad succesful";
                break;
		case ShowResult.Skipped:
			    Debug.Log("The ad was skipped before reaching the end.");
                mensaje += "The ad was skipped before reaching the end.";
                closeRewardScreen();
            break;
		case ShowResult.Failed:
			    Debug.LogError("The ad failed to be shown.");
                mensaje += "The ad failed to be shown.";
                connectionProblem.SetActive(true);
                closeRewardScreen();
                break;
		}
	}
    public void showBanner()
    {
        if (appodealDemo != null)
            appodealDemo.showBanner(Appodeal.BANNER_TOP);
    }

    public void hideBanner()
    {
        if (appodealDemo != null)
            appodealDemo.hideBanner();
    }

    public void closeRewardScreen(){
		rewardsPanel.PlayReverse();
        calendarPanel.PlayForward();
        GetConsecutiveDays();
    }

    void loadNextScene(string s)
    {
        VariablesGlobales.nextScene = s;
        GameObject.FindWithTag("loading").GetComponent<LoadSceneWait>().enabled = true;
    }

    IEnumerator rewardFromVideo(int t)
    {
        yield return new WaitForEndOfFrame();
        triesNumber.text = "" + t;
        PlayerPrefs.SetInt("triesLeft", t);
        closeRewardScreen();
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

    public void launchLevel(string texto)
    {
        print(texto);
        if (texto == "TUTORIAL")
        {
#if !UNITY_EDITOR
		    Analytics.CustomEvent ("enteringTutorial");
#endif
            PlayerPrefs.SetString("scene", "TUTORIAL");
			PlayerPrefs.SetInt("tutorialMode",1);
            if (loading != null)
                loading.SetActive(true);
            loadNextScene("InGame_tutorial");
            //SceneManager.LoadScene("InGame_tutorial");
        }
        else { 
            string num = texto.Split(new char[1] { 'L' })[2];
            #if !UNITY_EDITOR
		            Analytics.CustomEvent ("enteringLevel" + num);
            #endif
            Debug.Log("Enter stage "+num+" -> "+GlobalVariables.getSceneIndex(num));
            PlayerPrefs.SetString("scene", "Scene"+num);
            if (loading != null)
                loading.SetActive(true);
			//if(int.Parse(num) - 1 % 5 == 1)
				//CheckTutorial(int.Parse(num));
            loadNextScene("InGame");
            //SceneManager.LoadScene("InGame");
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			PlayerPrefs.DeleteAll ();
			Debug.Log ("delet this");
            loadNextScene("LevelSelection");
            //SceneManager.LoadScene ("LevelSelection");
        }

        if (Input.GetKeyDown (KeyCode.U)) {
			for (int i = 0; i < recordButtons.Length; i++) {
				PlayerPrefs.SetInt ("unlockedScene" + (i+1), 1);
			}
            loadNextScene("LevelSelection");
            //SceneManager.LoadScene ("LevelSelection");
        }
        //Debug.Log(dragged);

        if (dragLevels.isDragging)
			dragged = true;
            
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            NextPage();
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            PrevPage();
        }

        if(autoFlip && resolution.width != Screen.currentResolution.width){
            resolution = Screen.currentResolution;
            ResetGridButtons(firstInPage);
        }

        if (Input.GetMouseButtonDown (0)) {
            swipeInitialPos = Input.mousePosition;
            swipeInitialTime = Time.time;
            dragged = true;
        }

        if(Input.GetMouseButtonUp(0)){
            if (Input.GetMouseButtonUp (0)) {
                dragged = false;
				if (swipeInitialTime + 1f > Time.time && Vector3.Distance (swipeInitialPos, Input.mousePosition) >= 40f) {
					Vector3 dir = (Input.mousePosition - swipeInitialPos).normalized;
					print ("swipe!" + Vector3.Angle (new Vector3 (1f, 0f, 0f), dir));
					float angle = Vector3.Angle (new Vector3 (1f, 0f, 0f), dir);
					if (angle >= 0f && angle < 90f) {
                        PrevPage();
					} 
                    else {
                        NextPage();
					}
				}
			}
        }

        if((dragLevels.isDragging || dragged) && swipeIcon.gameObject.activeSelf){
			swipeIcon.PlayReverse();
		}
            
	}

    void NextPage(){
        if(firstInPage + buttonsPerPage >= GlobalVariables.nLevels)
            return;
        page = Mathf.Clamp(page + 1,0,300);     
        Debug.Log("page "+page);
        firstInPage = page * buttonsPerPage;
        ResetGridButtons(firstInPage);
    }

    void PrevPage(){
        page = Mathf.Clamp(page - 1,0,300); 
        Debug.Log("page "+page);
        firstInPage = page * buttonsPerPage;
        ResetGridButtons(firstInPage);
    }

	void Mute(){
		PlayerPrefs.SetInt ("Mute", 1);
		muteButton.spriteName = "mute2";
		muteButton.GetComponent<UIButton> ().normalSprite = "mute2";
        //Camera.main.GetComponent<AudioSource> ().mute = true;
        AudioListener.volume = 0f;
	}

    public void AutoRotateToggle(){
        autoFlip = !autoFlip;
    }

	void UnMute(){
		PlayerPrefs.SetInt ("Mute", 0);
		muteButton.spriteName = "mute";
		muteButton.GetComponent<UIButton> ().normalSprite = "mute";
		//Camera.main.GetComponent<AudioSource> ().mute = false;
        AudioListener.volume = 1f;
    }

	public void MuteButton(){
		if (PlayerPrefs.GetInt ("Mute") == 0)
			Mute ();
		else
			UnMute ();
	}

	public void ControlButton(){
		if (PlayerPrefs.GetInt ("Control") == 0) {
			controlButton.spriteName = "tap";
			controlButton.GetComponent<UIButton> ().normalSprite = "tap";
			PlayerPrefs.SetInt ("Control", 1);
		} else {
			controlButton.spriteName = "swipe";
			controlButton.GetComponent<UIButton> ().normalSprite = "swipe";
			PlayerPrefs.SetInt ("Control", 0);
		}
	}

	public void TutorialButton(){
		if(PlayerPrefs.GetInt("tutorialsDisabled") == 0){
			//set sprite
			PlayerPrefs.SetInt("tutorialsDisabled",1);
		}
		else{
			//set sprite
			PlayerPrefs.SetInt("tutorialsDisabled",0);
		}
	}

	public void LaunchDaily(){
        loadNextScene("InGame_daily");
        //SceneManager.LoadScene("InGame_daily");
    }

    public void GetConsecutiveDays(){
        ToggleCanPlay(true);
        ///codigo nuevo
        ///
        int triesLeft = PlayerPrefs.GetInt("triesLeft", 1);
        if (triesLeft == 0)
            if (!testing) ToggleCanPlay(false);
        triesNumber.text = triesLeft.ToString();
        ////hasta aqui

        //load last played date
        /*int consecutiveDays = PlayerPrefs.GetInt("consecutiveDays",-1);
		System.DateTime date = System.DateTime.Now.Date;
		//date = System.DateTime.Parse("12/28/2017 12:00:00 AM");
		System.DateTime lastPlayedDate = System.DateTime.Parse(PlayerPrefs.GetString("lastPlayedDate",date.ToString()));
		int daysSinceLastPlay = (int)(date - lastPlayedDate).TotalDays;
        print(lastPlayedDate + " " + date + " " + daysSinceLastPlay + " " + consecutiveDays);
		if(daysSinceLastPlay > 0){
			PlayerPrefs.SetInt("triesLeft",1);
		}
		int triesLeft = PlayerPrefs.GetInt("triesLeft",1);
		if(triesLeft == 0)
			if(!testing) ToggleCanPlay(false);
		triesNumber.text = triesLeft.ToString();
        if (daysSinceLastPlay == 0)
        {
            if (consecutiveDays == -1)
            {
                Debug.Log("First stage played");
            }
            else
            {
                Debug.Log(date);
                Debug.Log("Already played today");
            }
        }
        else
        {
            if (daysSinceLastPlay == 1)
            {
                print("aumentando consecutive days");
                if (consecutiveDays < 0) consecutiveDays = 0;
                consecutiveDays++;
                PlayerPrefs.SetInt("consecutiveDays", consecutiveDays);
            }
            else
            {
                if (daysSinceLastPlay > 1)
                {
                    PlayerPrefs.SetInt("consecutiveDays", 0);
                    Debug.Log("Haven't played in over a day");
                    consecDaysSlider.value = 0;
                    consecutiveDays = 0;
                }
            }
        }
        PlayerPrefs.SetString("lastPlayedDate", date.ToString());*/

        SetMedals();
		PlayerPrefs.SetString("lastLoadedDaily",System.DateTime.Now.Date.ToString());
		Debug.Log(LevelSkillTotal() + " | "+PlayerPrefs.GetFloat("totalDaily",0)/2f);
		mathSkillSlider.value = LevelSkillTotal() + PlayerPrefs.GetFloat("totalDaily",0)/2f;
		PlayerPrefs.SetInt("lvlSelectDaily",0);
	}

	void ToggleCanPlay(bool b){
        if (!b)
        {
            connectionProblem.SetActive(false);
            rewardsPanel.PlayForward();
            calendarPanel.PlayReverse();
        }
        else
        {
            rewardsPanel.PlayReverse();
            calendarPanel.PlayForward();
        }
        startDaily.SetActive(b);
	}

	public static float LevelSkillTotal(){
		float total = 0;
		for(int i=1;i<=20;i++){
			float percentage = 0;
			if(i <= 5)
				percentage = 0.01f;
			else if(i > 5 && i <= 10)
				percentage = 0.02f;
			else if(i > 10 && i <= 15)
				percentage = 0.03f;
			else if(i > 15 && i <= 20)
				percentage = 0.04f;
			//total += percentage*(float)(PlayerPrefs.GetFloat("recordScene"+i,0));
			if(PlayerPrefs.GetFloat("recordScene"+i,0) != 0)
				total += percentage;
			//Debug.Log("Scene "+i+": "+PlayerPrefs.GetFloat("recordScene"+i,0));
		}
		Debug.Log(total);
		return Mathf.Round(total*100f)/100f;
	}

	void EnableSwipe(){
		if(PlayerPrefs.GetInt("unlockedScene3") == 1){
			swipeIcon.gameObject.SetActive(true);
		}
	}

	public static void CheckTutorial(int level){
        //if (PlayerPrefs.GetInt("tutorialMode", 0) == Mathf.RoundToInt(level / 5) + 1 || level == 1) return;
        if(level == 1) return;
		PlayerPrefs.SetInt("tutorialMode",Mathf.RoundToInt(level/5) + 1);
        SceneManager.LoadScene("InGame_tutorial");
    }

}
