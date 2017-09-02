using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Heyzap; // make sure to include this line at the top of any C# file you use our SDK from

public class HeyZap_Nemoris : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Demographic information about your app's current user can optionally be used to increase CPMs. If you have it, provide it at any time (but also as early as possible) like so:
        // HZDemographics.SetUserGender(HZDemographics.Gender.FEMALE); // things like location, birthday, education, and more can be set as well

        // Your Publisher ID is: 268f0f16490c582afaac18d73891d2df
        HeyzapAds.Start("268f0f16490c582afaac18d73891d2df", HeyzapAds.FLAG_NO_OPTIONS);
        DontDestroyOnLoad(gameObject);
        HeyzapAds.ShowMediationTestSuite();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
