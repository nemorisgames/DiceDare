using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LevelSelection : MonoBehaviour {
	
	public UIButton[] recordButtons;
	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll ();
		PlayerPrefs.SetInt ("timesDied", 0);

		//for(int i = 1; i < recordButtons.Length; i++)
		//	recordButtons[i].isEnabled = false;

		for (int i = 0; i < recordButtons.Length; i++) {
			float recordSeconds = PlayerPrefs.GetFloat ("recordScene" + (i + 1), -1f);
			recordButtons[i].transform.FindChild("record").gameObject.SetActive(recordSeconds > 0);
			if (recordSeconds > 0) {
				int minutes = (int)((recordSeconds) / 60);
				int seconds = (int)((recordSeconds) % 60);
				int dec = (int)(((recordSeconds) % 60 * 10f) - ((int)((recordSeconds) % 60) * 10));
				recordButtons[i].transform.FindChild("record").GetComponent<UILabel>().text = "" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "." + dec;	
			}
		}
	}

	public void launchLevel(string texto){
		string num = texto.Split (new char[1]{ 'L' }) [2];
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
