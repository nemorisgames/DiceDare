﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
	TextMesh text;
	public enum StateCell {Normal, Passed, EndCell};
	public StateCell stateCell = StateCell.Normal;
	public int number = 3;
	// Use this for initialization
	void Start () {
		text = transform.FindChild ("Text").GetComponent<TextMesh> ();
		text.text = "" + number;
		changeState (stateCell);
	}

	public void changeState(StateCell s){
		switch (s) {
		case StateCell.Passed: 
			text.text = "-";
			GetComponent<MeshRenderer> ().enabled = false;
			break;
		}
		stateCell = s;
	}

	public IEnumerator shine(){
		Material m = GetComponent<Renderer> ().material;
		for (int i = 0; i < 30; i++) {
			yield return new WaitForSeconds (0.01f);
			m.SetColor ("_EmissionColor", Color.yellow * i * 0.01f);
		}
		for (int i = 30; i > 0; i--) {
			yield return new WaitForSeconds (0.01f);
			m.SetColor ("_EmissionColor", Color.yellow * i * 0.01f);
		}

		for (int i = 0; i < 30; i++) {
			yield return new WaitForSeconds (0.01f);
			m.SetColor ("_EmissionColor", Color.yellow * i * 0.01f);
		}
		for (int i = 30; i > 0; i--) {
			yield return new WaitForSeconds (0.01f);
			m.SetColor ("_EmissionColor", Color.yellow * i * 0.01f);
		}

		for (int i = 0; i < 30; i++) {
			yield return new WaitForSeconds (0.01f);
			m.SetColor ("_EmissionColor", Color.yellow * i * 0.01f);
		}
		for (int i = 30; i > 0; i--) {
			yield return new WaitForSeconds (0.01f);
			m.SetColor ("_EmissionColor", Color.yellow * i * 0.01f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
