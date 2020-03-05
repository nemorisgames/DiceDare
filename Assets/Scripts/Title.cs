using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using VoxelBusters.NativePlugins;

public class Title : MonoBehaviour {
	bool credits = false;
	public UIPanel creditsPanel;
    public GameObject loading;

    bool _isAvailable = false;
    bool _isAuthenticated = false;
    string message = "";
    public GameObject dailyPanel;
    public AudioClip bgm;

    void Start ()
    {
        string language = PlayerPrefs.GetString("language","");
        if(language == ""){
            if(Application.systemLanguage == SystemLanguage.Spanish){
                Localization.language = "Spanish";
                PlayerPrefs.SetString("language","Spanish");
            }
            else{
                Localization.language = "English";
                PlayerPrefs.SetString("language","English");
            }
        }
        else{
            Localization.language = language;
        }
        /*
        if (!GlobalVariables.finishOrderingProcess)
        {
            GlobalVariables.SetScenes();
            //GlobalVariables.orderScenes();
            for (int i = 0; i < GlobalVariables.nLevels; i++)
                print(GlobalVariables.getIndexScene("" + (i + 1)));
        }*/
        GlobalVariables.SetScenes();
        PlayerPrefs.SetInt("lvlSelectDaily",0);
        /*_isAvailable = NPBinding.GameServices.IsAvailable();
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
        }*/
        BGMManager.Instance.Play(bgm,0.47f,1.4f);
        int consecutiveDays = PlayerPrefs.GetInt("consecutiveDays", -1);
        System.DateTime date = System.DateTime.Now.Date;
        System.DateTime lastPlayedDate = System.DateTime.Parse(PlayerPrefs.GetString("lastPlayedDate", date.ToString()));
        int daysSinceLastPlay = (int)(date - lastPlayedDate).TotalDays;
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
                //dailyPanel.SetActive(false);
            }
        }
    }

    void loadNextScene(string s)
    {
        print("escene " + s);
        VariablesGlobales.nextScene = s;
        GameObject.FindWithTag("loading").GetComponent<LoadSceneWait>().enabled = true;
    }

    public void play(){

        if (GameObject.Find("AppoDeal") != null)
            GameObject.Find("AppoDeal").GetComponent<AppodealDemo>().Init();

        if (PlayerPrefs.GetInt ("PlayedGame", 0) == 0) {
            loading.SetActive(true);
            PlayerPrefs.SetInt ("PlayedGame", 1);
            if(!PlayerPrefs.HasKey("scene"))
                PlayerPrefs.SetString ("scene", GlobalVariables.getIndexScene("1"));
            loadNextScene("NewTutorial");
            //SceneManager.LoadScene ("InGame_tutorial");
			PlayerPrefs.SetInt("lvlSelectDaily",0);
		} else
        {
            loadNextScene("LevelSelection");
            //SceneManager.LoadScene ("LevelSelection");
		}
	}

	public void daily(){

        if (GameObject.Find("AppoDeal") != null)
            GameObject.Find("AppoDeal").GetComponent<AppodealDemo>().Init();

        //SceneManager.LoadScene ("LevelSelection");
        loadNextScene("LevelSelection");
        PlayerPrefs.SetInt("lvlSelectDaily",1);
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
