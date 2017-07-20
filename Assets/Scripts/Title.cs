using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelBusters.NativePlugins;

public class Title : MonoBehaviour {
	bool credits = false;
	public UIPanel creditsPanel;

    bool _isAvailable = false;
    bool _isAuthenticated = false;
    string message = "";
    // Use this for initialization
    void Start () {
        _isAvailable = NPBinding.GameServices.IsAvailable();
        if (_isAvailable)
        {
            _isAuthenticated = NPBinding.GameServices.LocalUser.IsAuthenticated;
            if (!_isAuthenticated)
            {
                NPBinding.GameServices.LocalUser.Authenticate((bool _success, string _error) => {

                    if (_success)
                    {
                         
                    }
                    else
                    {

                    }
                });
            }
        }
    }

<<<<<<< HEAD
    public void play(){
		SceneManager.LoadScene ("LevelSelection");
=======
	public void play(){
		if (PlayerPrefs.GetInt ("PlayedGame", 0) == 0) {
			PlayerPrefs.SetInt ("PlayedGame", 1);
			PlayerPrefs.SetString ("scene", "Scene" + 1);
			SceneManager.LoadScene ("InGame");
		} else {
			SceneManager.LoadScene ("LevelSelection");
		}
>>>>>>> d3a67747d1e7e5d9ef3c6bc3e48cf2ad47dfe34f
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
