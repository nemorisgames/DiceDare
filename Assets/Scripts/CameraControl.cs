using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
	Transform obj;
	Vector3 initialPosition;
	// Use this for initialization
	void Start () {
		obj = GameObject.FindGameObjectWithTag ("Dice").transform;
		initialPosition = transform.position + obj.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp (transform.position, initialPosition + obj.position, Time.deltaTime * 10f);
		transform.LookAt (obj);
	}
}
