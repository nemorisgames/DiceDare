using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacentCellFinder : MonoBehaviour {
	InGame ingame;

	// Use this for initialization
	void Start () {
		ingame = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<InGame>();
	}

	void OnTriggerStay(Collider c){
		if (!ingame.rotating && c.name.Substring (0, 4) == "Cell") {
			Cell cell = c.GetComponent<Cell> ();
			if (cell.stateCell != Cell.StateCell.Passed)
				//StartCoroutine(cell.shine (1));
				Debug.Log(this.name+": "+cell.number);
		}
	}

	void OnTriggerExit(Collider c){
		if (c.name.Substring (0, 4) == "Cell") {
			Cell cell = c.GetComponent<Cell> ();
			Debug.Log (this.name + ": " + cell.number);
		}
	}
}
