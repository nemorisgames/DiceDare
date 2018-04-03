using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {
	public string nextScene = "Title";
	public float timeLimit = 7f;
	// Use this for initialization
	void Start () {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("lvlSelectDaily", 0);
        //permite que el calendario no escriba puntaje cuando no se escribio en la sesion anterior
        PlayerPrefs.DeleteKey("dailyScore");
        //print("mute " + PlayerPrefs.GetInt("Mute", 1));
        if (PlayerPrefs.GetInt("Mute", 0) == 1)
            AudioListener.volume = 0f;
    }
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad > timeLimit) {
			SceneManager.LoadScene (nextScene);
		}
	}
}
