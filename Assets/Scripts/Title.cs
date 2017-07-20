using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelBusters.NativePlugins;

public class Title : MonoBehaviour {

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

    public void play(){
		SceneManager.LoadScene ("LevelSelection");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
