using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class Appodeal_nemoris : MonoBehaviour
{

#if UNITY_EDITOR && !UNITY_ANDROID && !UNITY_IPHONE
		string appKey = "";
#elif UNITY_ANDROID
    string appKey = "7db225fc3f0d8f1a441776cfd1413e123475b21648b84ae3";
#elif UNITY_IPHONE
	string appKey = "2c47c5255d7dd1eb44c0ac7ebe135f126ae2598802f1b7cc";
#else
    string appKey = "";
#endif
    // Use this for initialization
    void Start () {
        Init();

        Appodeal.setTesting(true);
        Appodeal.show(Appodeal.INTERSTITIAL);
    }

    public void Init()
    {
        Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.REWARDED_VIDEO);
    }
    

    void OnGUI()
    {
        //if (GUI.Button(new Rect(Screen.width / 10, Screen.height / 10, 200, 100), "INITIALIZE"))
        //    Init();
    }
        // Update is called once per frame
    void Update () {
		
	}
}
