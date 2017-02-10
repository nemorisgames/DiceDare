using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LevelSelection : MonoBehaviour {
	public UIButton record1Button;
	public UIButton record2Button;
	public UIButton record3Button;
	public UILabel record1;
	public UILabel record2;
	public UILabel record3;
	// Use this for initialization
	void Start () {
		float recordSeconds = PlayerPrefs.GetFloat ("recordInGame1", -1f);
		record2Button.isEnabled = false;
		record3Button.isEnabled = false;
		if (recordSeconds > 0) {
			record2Button.isEnabled = true;
			int minutes = (int)((60 - recordSeconds) / 60);
			int seconds = (int)((60 - recordSeconds) % 60);
			int dec = (int)(((60 - recordSeconds) % 60 * 10f) - ((int)((60 - recordSeconds) % 60) * 10));
			record1.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}

		recordSeconds = PlayerPrefs.GetFloat ("recordInGame2", -1f);
		if (recordSeconds > 0) {
			record3Button.isEnabled = true;
			int minutes = (int)((90 - recordSeconds) / 60);
			int seconds = (int)((90 - recordSeconds) % 60);
			int dec = (int)(((90 - recordSeconds) % 60 * 10f) - ((int)((90 - recordSeconds) % 60) * 10));
			record2.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}

		recordSeconds = PlayerPrefs.GetFloat ("recordInGame2", -1f);
		if (recordSeconds > 0) {
			int minutes = (int)((120 - recordSeconds) / 60);
			int seconds = (int)((120 - recordSeconds) % 60);
			int dec = (int)(((120 - recordSeconds) % 60 * 10f) - ((int)((120 - recordSeconds) % 60) * 10));
			record3.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}
	}

	public void level1(){
		Analytics.CustomEvent ("enteringLevel1");
		SceneManager.LoadScene ("InGame1");
	}
	public void level2(){
		Analytics.CustomEvent ("enteringLevel2");
		SceneManager.LoadScene ("InGame2");
	}
	public void level3(){
		Analytics.CustomEvent ("enteringLevel3");
		SceneManager.LoadScene ("InGame3");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
