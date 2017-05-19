using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
	bool credits = false;
	public UIPanel creditsPanel;

	// Use this for initialization
	void Start () {
		
	}


	public void play(){
		if (PlayerPrefs.GetInt ("PlayedGame", 0) == 0) {
			PlayerPrefs.SetInt ("PlayedGame", 1);
			PlayerPrefs.SetString ("scene", "Scene" + 1);
			SceneManager.LoadScene ("InGame");
		} else {
			SceneManager.LoadScene ("LevelSelection");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			PlayerPrefs.SetInt ("PlayedGame", 0);
			Debug.Log ("not played");
		}
	}

	public void Credits(){
		if (!credits) {
			creditsPanel.GetComponent<TweenAlpha> ().PlayForward ();
			credits = true;
		} else {
			creditsPanel.GetComponent<TweenAlpha> ().PlayReverse ();
			credits = false;
		}
	}
}
