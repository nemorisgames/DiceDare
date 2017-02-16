using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LevelSelection : MonoBehaviour {
	public UIButton record1Button;
	public UIButton record2Button;
	public UIButton record3Button;
	public UIButton record4Button;
	public UIButton record5Button;
	public UILabel record1;
	public UILabel record2;
	public UILabel record3;
	public UILabel record4;
	public UILabel record5;
	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll ();
		float recordSeconds = PlayerPrefs.GetFloat ("recordScene1", -1f);
		record2Button.isEnabled = false;
		record3Button.isEnabled = false;
		record4Button.isEnabled = false;
		record5Button.isEnabled = false;
		if (recordSeconds > 0) {
			record2Button.isEnabled = true;
			int minutes = (int)((recordSeconds) / 60);
			int seconds = (int)((recordSeconds) % 60);
			int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
			record1.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}

		recordSeconds = PlayerPrefs.GetFloat ("recordScene2", -1f);
		if (recordSeconds > 0) {
			record3Button.isEnabled = true;
			int minutes = (int)((recordSeconds) / 60);
			int seconds = (int)((recordSeconds) % 60);
			int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
			record2.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}

		recordSeconds = PlayerPrefs.GetFloat ("recordScene3", -1f);
		if (recordSeconds > 0) {
			record4Button.isEnabled = true;
			int minutes = (int)((recordSeconds) / 60);
			int seconds = (int)((recordSeconds) % 60);
			int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
			record3.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}

		recordSeconds = PlayerPrefs.GetFloat ("recordScene4", -1f);
		if (recordSeconds > 0) {
			record5Button.isEnabled = true;
			int minutes = (int)((recordSeconds) / 60);
			int seconds = (int)((recordSeconds) % 60);
			int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
			record4.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}

		recordSeconds = PlayerPrefs.GetFloat ("recordScene5", -1f);
		if (recordSeconds > 0) {
			int minutes = (int)((recordSeconds) / 60);
			int seconds = (int)((recordSeconds) % 60);
			int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
			record5.text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
		}
	}

	public void launchLevel(string texto){
		string num = texto.Split (new char[1]{ ' ' }) [1];
		#if !UNITY_EDITOR
		Analytics.CustomEvent ("enteringLevel" + num);
		#endif
		PlayerPrefs.SetString ("scene", "Scene" + num);
		SceneManager.LoadScene ("InGame");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
