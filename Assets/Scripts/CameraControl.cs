using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
	Transform obj;
	Vector3 initialPosition;
	bool rotating = false;
	// Use this for initialization
	void Start () {
		obj = GameObject.FindGameObjectWithTag ("Dice").transform;
		initialPosition = transform.position + obj.position;
	}

	void rotateCamera(bool CW){
		StartCoroutine(rotateCameraCoroutine(CW));
	}

	IEnumerator rotateCameraCoroutine(bool CW){
		if (rotating)
			yield return false;
		rotating = true;
		int nSteps = 30;
		//for (int i = 1; i <= nSteps; i++) {
			yield return new WaitForSeconds (0.01f);
		Camera.main.transform.RotateAround (obj.position, Vector3.up, (CW ? 90f : -90f));// / nSteps);
		//}
		initialPosition = transform.position - obj.position * 2f + Vector3.up * 0.5f;
		rotating = false;
	}

	// Update is called once per frame
	void Update () {
		if(!rotating)
			transform.position = Vector3.Lerp (transform.position, initialPosition + obj.position, Time.deltaTime * 10f);
		transform.LookAt (obj);
	}
}
