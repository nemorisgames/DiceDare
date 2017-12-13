using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LevelSelection : MonoBehaviour {
	public bool testing = true;
	public UIButton[] recordButtons;
	public UIPanel panel;
	public GameObject loading;
	public UISprite muteButton;
	public UISprite controlButton;
	public UISlider mathSkillSlider;
	public UISlider consecDaysSlider;
	public GameObject medals_GO;
	public UISprite [] medals;
	public UILabel consecDays;
	public UIPanel dailyPanel;
	public GameObject startDaily;
	public GameObject alreadyPlayed;
	// Use this for initialization
	void Start () {
		InitMedals();
		
		PlayerPrefs.SetInt ("continueBGM", 0);
		if (PlayerPrefs.GetInt ("unlockedScene1") != 1) {
			PlayerPrefs.SetInt ("unlockedScene1", 1);
		}
		//PlayerPrefs.DeleteAll ();

		if (PlayerPrefs.GetInt ("Mute") == 1)
			Mute ();

		if (PlayerPrefs.GetInt ("Control") == 1) {
			controlButton.spriteName = "tap";
			controlButton.GetComponent<UIButton> ().normalSprite = "tap";
		}

		if (PlayerPrefs.GetString ("scene") != "") {
			string texto = PlayerPrefs.GetString ("scene", "Scene1");
			string num = texto.Split (new char[1]{ 'e' }) [2];
			int level = (int.Parse (num) - 1);
			panel.transform.localPosition = new Vector3 (-400 * level, panel.transform.position.y, panel.transform.position.z);
			panel.clipOffset = new Vector2 (400 * level, panel.clipOffset.y);
		}

		PlayerPrefs.SetInt ("timesDied", 0);
		for (int i = 0; i < recordButtons.Length; i++) {
			if (PlayerPrefs.GetInt ("unlockedScene" + (i+1),0) != 1) {
				recordButtons[i].isEnabled = false;
			}
		}
		//	

		for (int i = 0; i < recordButtons.Length; i++) {
			float recordSeconds = PlayerPrefs.GetFloat ("recordScene" + (i + 1), -1f);
			recordButtons[i].transform.Find("record").gameObject.SetActive(recordSeconds > 0);
			if (recordSeconds > 0) {
				int minutes = (int)((recordSeconds) / 60);
				int seconds = (int)((recordSeconds) % 60);
				int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
				recordButtons[i].transform.Find("record").GetComponent<UILabel>().text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
			}
		}

		if(PlayerPrefs.GetInt("lvlSelectDaily",0) == 1){
			dailyPanel.GetComponent<TweenAlpha>().PlayForward();
			GetConsecutiveDays();
		}
			
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

	public void launchLevel(string texto){
		string num = texto.Split (new char[1]{ 'L' }) [2];
		#if !UNITY_EDITOR
		Analytics.CustomEvent ("enteringLevel" + num);
		#endif
		PlayerPrefs.SetString ("scene", "Scene" + num);
		if(loading != null)
			loading.SetActive (true);
		SceneManager.LoadScene ("InGame");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			PlayerPrefs.DeleteAll ();
			Debug.Log ("delet this");
			SceneManager.LoadScene ("LevelSelection");
		}

		if (Input.GetKeyDown (KeyCode.U)) {
			for (int i = 0; i < recordButtons.Length; i++) {
				PlayerPrefs.SetInt ("unlockedScene" + (i+1), 1);
			}
			SceneManager.LoadScene ("LevelSelection");
		}
	}

	void Mute(){
		PlayerPrefs.SetInt ("Mute", 1);
		muteButton.spriteName = "mute2";
		muteButton.GetComponent<UIButton> ().normalSprite = "mute2";
		Camera.main.GetComponent<AudioSource> ().mute = true;
	}

	void UnMute(){
		PlayerPrefs.SetInt ("Mute", 0);
		muteButton.spriteName = "mute";
		muteButton.GetComponent<UIButton> ().normalSprite = "mute";
		Camera.main.GetComponent<AudioSource> ().mute = false;
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

	public void LaunchDaily(){
		SceneManager.LoadScene("InGame_daily");
	}

	public void GetConsecutiveDays(){
		ToggleCanPlay(true);
		//load last played date
		int consecutiveDays = PlayerPrefs.GetInt("consecutiveDays",-1);
		System.DateTime date = System.DateTime.Now.Date;
		//date = System.DateTime.Parse("11/30/2017 12:00:00 AM");
		System.DateTime lastPlayedDate = System.DateTime.Parse(PlayerPrefs.GetString("lastPlayedDate",date.ToString()));
		int daysSinceLastPlay = (int)(date - lastPlayedDate).TotalDays;
		if(daysSinceLastPlay == 0){
			if(consecutiveDays == -1){
				Debug.Log("First stage played");
			}
			else{
				Debug.Log(date);
				Debug.Log("Already played today");
				if(!testing) ToggleCanPlay(false);
			}
				
		}
		else if(daysSinceLastPlay > 1){
			PlayerPrefs.SetInt("consecutiveDays",0);
			Debug.Log("Haven't played in over a day");
			consecDaysSlider.value = 0;
			consecutiveDays = 0;
		}
		float penalization = 0;
		consecDays.text = consecutiveDays.ToString();
		if(consecutiveDays == 0){
			consecDaysSlider.value = 0;
			penalization = -0.01f;
		}
		else if(consecutiveDays == -1){
			consecDaysSlider.value = 0;
			consecDays.text = 0.ToString();
		}
		else{
			float aux = consecutiveDays;
			consecDaysSlider.value = Mathf.Clamp(aux,0f,7f) * (1f/7f);
		}
		if(System.DateTime.Now.Date.ToString() == PlayerPrefs.GetString("lastLoadedDaily","")){
			penalization = 0;
		}
		SetMedals();
		PlayerPrefs.SetString("lastLoadedDaily",System.DateTime.Now.Date.ToString());
		PlayerPrefs.SetFloat("totalDaily",PlayerPrefs.GetFloat("totalDaily")+penalization);
		Debug.Log(LevelSkillTotal() + " | "+PlayerPrefs.GetFloat("totalDaily",0)/2f);
		mathSkillSlider.value = LevelSkillTotal() + PlayerPrefs.GetFloat("totalDaily",0)/2f;
		PlayerPrefs.SetInt("lvlSelectDaily",0);
	}

	void ToggleCanPlay(bool b){
		startDaily.SetActive(b);
		alreadyPlayed.SetActive(!b);
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
}
