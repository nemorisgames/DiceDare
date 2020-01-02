using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
	TextMesh text;
	public enum StateCell {Normal, Passed, EndCell};
	public StateCell stateCell = StateCell.Normal;
	public int number = 3;
	Color32 defaultColor;
	public bool operation = false;
	// Use this for initialization
	void Start () {
        SetNumber(number);
		changeState (stateCell);
		defaultColor = GetComponent<Renderer> ().material.GetColor ("_EmissionColor");
	}

    public void SetNumber(int number)
    {
        this.number = number;
        text = transform.Find("Text").GetComponent<TextMesh>();
        SetText("" + number);
    }

	public void changeState(StateCell s){
		switch (s) {
		case StateCell.Passed:
            SetText("-");
			GetComponent<MeshRenderer> ().enabled = false;
            //elimina el script para cambio de numero automatico
            AutomaticChangeNumber a = GetComponent<AutomaticChangeNumber>();
            if (a != null) Destroy(a);
            //elimina el script para cambio de numero invisible
            DissapearNumber d = GetComponent<DissapearNumber>();
            if (d != null) Destroy(d);
            break;
		}
		stateCell = s;
	}

    public void SetText(string text)
    {
        this.text.text = text;
    }

	public IEnumerator shine(int num){
		Material m = GetComponent<Renderer> ().material;
		Color32 colorDefault = m.GetColor ("_EmissionColor");
		for (int j = 0; j < num; j++) {
			for (int i = 0; i < 15; i++) {
				yield return new WaitForSeconds (0.01f);
				m.SetColor ("_EmissionColor", Color.yellow * i * 0.1f);
			}
			for (int i = 15; i > 0; i--) {
				yield return new WaitForSeconds (0.01f);
				m.SetColor ("_EmissionColor", Color.yellow * i * 0.1f);
			}
		}
		m.SetColor ("_EmissionColor", colorDefault);
	}

	public void unshine(){
		GetComponent<Renderer> ().material.SetColor ("_EmissionColor", defaultColor);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init(int n){
		number = n;
		text = transform.Find ("Text").GetComponent<TextMesh> ();
		text.text = "" + number;
	}
}
