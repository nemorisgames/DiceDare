﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideo : MonoBehaviour {
	VideoPlayer vp;
	Animator anim;
	public VideoClip [] clips; //0 = suma, 1 = resta, 2 = multi, 3 = div

	// Use this for initialization
	void Start () {
		vp = GetComponent<VideoPlayer> ();
		anim = GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayClip(int i){
		Debug.Log (i);
		//vp.Stop ();
		vp.clip = clips [i];
		vp.Play ();
		anim.SetTrigger ("Start");
	}

	public void ToggleOff(){
		anim.SetTrigger ("FadeOut");
	}
}