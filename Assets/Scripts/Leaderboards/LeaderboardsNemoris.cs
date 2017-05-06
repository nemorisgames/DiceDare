using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VoxelBusters.NativePlugins;

public class LeaderboardsNemoris : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        bool _isAvailable = NPBinding.GameServices.IsAvailable();
        print("game services " + _isAvailable);
        if (_isAvailable)
        {
            bool _isAuthenticated = NPBinding.GameServices.LocalUser.IsAuthenticated;
            print("autnticated " + _isAuthenticated);
            if (!_isAuthenticated)
            {
                NPBinding.GameServices.LocalUser.Authenticate((bool _success, string _error) =>
                {

                    if (_success)
                    {
                        Debug.Log("Sign-In Successfully");
                        Debug.Log("Local User Details : " + NPBinding.GameServices.LocalUser.ToString());

                    }
                    else
                    {
                        Debug.Log("Sign-In Failed with error " + _error);
                    }
                });
            }
        }

    }

    public void enviarScore()
    {
        NPBinding.GameServices.ReportScoreWithGlobalID("highscorescene1", 100, (bool _success, string _error) =>
        {

            if (_success)
            {
                Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} finished successfully.", "highscorescene1"));
                Debug.Log(string.Format("New score= {0}.", 100));
            }
            else
            {
                Debug.Log(string.Format("Request to report score to leaderboard with GID= {0} failed.", "highscorescene1"));
                Debug.Log(string.Format("Error= {0}.", _error.ToString()));
            }
        });
        
    }

    public void verLeaderboard() {
        NPBinding.GameServices.ShowLeaderboardUIWithGlobalID("highscorescene1", eLeaderboardTimeScope.TODAY, (string _error) => {
            Debug.Log("Closed leaderboard UI. " + _error);
        });
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Y)){
            enviarScore();
        }
        if (Input.GetKeyDown(KeyCode.U)){
            verLeaderboard();
        }
    }
}
