using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideo : MonoBehaviour {
	VideoPlayer vp;
	Animator anim;
	//public VideoClip [] clips; //0 = suma, 1 = resta, 2 = multi, 3 = div

	// Use this for initialization
	void Start () {
		vp = GetComponent<VideoPlayer> ();
		anim = GetComponentInChildren<Animator> ();
		vp.skipOnDrop = false;
		vp.Prepare ();
		//Debug.Log (vp.frameRate);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*public IEnumerator PlayClip(int i){
		Debug.Log (i);
		//vp.Stop ();
		vp.clip = clips [i];
		vp.skipOnDrop = false;
		vp.Prepare ();
		WaitForSeconds waitTime = new WaitForSeconds (2f);
		while (!vp.isPrepared) {
			yield return waitTime;
			break;
		}
		vp.Play ();
		anim.SetTrigger ("Start");
	}*/

	public void ToggleOff(){
		anim.SetTrigger ("FadeOut");
	}
}
