using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class Appodeal_nemoris : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_ANDROID
        string appKey = "7db225fc3f0d8f1a441776cfd1413e123475b21648b84ae3";
#endif
#if UNITY_IPHONE
        string appKey = "2c47c5255d7dd1eb44c0ac7ebe135f126ae2598802f1b7cc";

#endif
		Appodeal.setTesting(false);
		Appodeal.disableLocationPermissionCheck();
		Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.REWARDED_VIDEO);

		//Appodeal.show(Appodeal.INTERSTITIAL);
    }

	void Awake(){
		//Appodeal.requestAndroidMPermissions(this);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
