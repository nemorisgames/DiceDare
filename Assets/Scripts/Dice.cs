﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour {
	bool onMovement = false;
	bool calculated = false;
	public enum Operation {Sum, Rest, Mult, Div};
	public Operation currentOperation = Operation.Sum;
	public enum Direction {Up, Down, Left, Right};
	Direction lastDirection;
	public Vector3 currentPos;
	ArrayList numbers = new ArrayList();
	ArrayList currentNumbers = new ArrayList();
	Operation nextOperation;
	InGame inGame;
	public int steps = 0;
	// Use this for initialization
	void Start () {
		currentPos = transform.position;
		numbers.Add(transform.FindChild("TextUp").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextDown").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextLeft").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextRight").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextForward").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextBackward").GetComponent<TextMesh>());

		currentNumbers = numbers;

		inGame = Camera.main.GetComponent<InGame> ();

		nextOperation = currentOperation;
	}

	IEnumerator turn(Direction d){
		onMovement = true;
		calculated = false;
		int nStemps = 10;
		//define el numero en la cara escondida
		switch(d){
		case Direction.Up:
			
			break;
		case Direction.Down:
			RaycastHit r;
			if(Physics.Raycast(transform.position - Vector3.forward + Vector3.up, new Vector3(0f, -1f, 0f), out r, 5f,LayerMask.GetMask(new string[1]{"Cell"}))){
				if (r.collider.GetComponent<Cell> ().stateCell == Cell.StateCell.Normal) {
					print (r.collider.name);
					((TextMesh)currentNumbers [5]).text = "" + inGame.checkOperationResult (int.Parse (((TextMesh)currentNumbers [4]).text), int.Parse (((TextMesh)currentNumbers [0]).text));
				}
			}
			break;
		case Direction.Left:
			if (Physics.Raycast (transform.position - Vector3.right + Vector3.up, new Vector3 (0f, -1f, 0f), out r, 5f,LayerMask.GetMask(new string[1]{"Cell"}))) {
				if (r.collider.GetComponent<Cell> ().stateCell == Cell.StateCell.Normal) {
					print (r.collider.name);
					((TextMesh)currentNumbers [3]).text = "" + inGame.checkOperationResult (int.Parse (((TextMesh)currentNumbers [2]).text), int.Parse (((TextMesh)currentNumbers [0]).text));
				}
			}
			break;
		case Direction.Right:
			
			break;
		}
		//gira en dado
		for (int i = 1; i <= nStemps; i++) {
			yield return new WaitForSeconds (0.01f);
			switch(d){
			case Direction.Up:
				transform.RotateAround (currentPos + new Vector3 (0f, -0.5f, 0.5f), Vector3.right, 90f / nStemps);
				break;
			case Direction.Down:
				transform.RotateAround (currentPos + new Vector3 (0f, -0.5f, -0.5f), Vector3.right, -90f / nStemps);
				break;
			case Direction.Left:
				transform.RotateAround (currentPos + new Vector3 (-0.5f, -0.5f, 0f), Vector3.forward, 90f / nStemps);
				break;
			case Direction.Right:
				transform.RotateAround (currentPos + new Vector3 (0.5f, -0.5f, 0f), Vector3.forward, -90f / nStemps);
				break;
			}
		}

		//ordena las caras y las almacena
		switch(d){
		case Direction.Up:
			TextMesh t = ((TextMesh)currentNumbers [5]);
			currentNumbers [5] = currentNumbers [0];
			currentNumbers [0] = currentNumbers [4];
			currentNumbers [4] = currentNumbers [1];
			currentNumbers [1] = t;
			break;
		case Direction.Down:
			t = ((TextMesh)currentNumbers [1]);
			currentNumbers [1] = currentNumbers [4];
			currentNumbers [4] = currentNumbers [0];
			currentNumbers [0] = currentNumbers [5];
			currentNumbers [5] = t;
			break;
		case Direction.Left:
			t = ((TextMesh)currentNumbers [1]);
			currentNumbers [1] = currentNumbers [2];
			currentNumbers [2] = currentNumbers [0];
			currentNumbers [0] = currentNumbers [3];
			currentNumbers [3] = t;
			break;
		case Direction.Right:
			t = ((TextMesh)currentNumbers [3]);
			currentNumbers [3] = currentNumbers [0];
			currentNumbers [0] = currentNumbers [2];
			currentNumbers [2] = currentNumbers [1];
			currentNumbers [1] = t;
			break;
		}
		//gira las caras
		Transform pos;
		switch(d){
		case Direction.Up:
			pos = ((TextMesh)currentNumbers [1]).transform;
			((TextMesh)currentNumbers [1]).transform.RotateAround (pos.position, pos.forward, -90);
			pos = ((TextMesh)currentNumbers [2]).transform;
			((TextMesh)currentNumbers [2]).transform.RotateAround (pos.position, pos.forward, -90);
			pos = ((TextMesh)currentNumbers [3]).transform;
			((TextMesh)currentNumbers [3]).transform.RotateAround (pos.position, pos.forward, 90);
			pos = ((TextMesh)currentNumbers [4]).transform;
			((TextMesh)currentNumbers [4]).transform.RotateAround (pos.position, pos.forward, 90);
			break;
		case Direction.Down:
			pos = ((TextMesh)currentNumbers [1]).transform;
			((TextMesh)currentNumbers [1]).transform.RotateAround (pos.position, pos.forward, -90);
			pos = ((TextMesh)currentNumbers [2]).transform;
			((TextMesh)currentNumbers [2]).transform.RotateAround (pos.position, pos.forward, 90);
			pos = ((TextMesh)currentNumbers [3]).transform;
			((TextMesh)currentNumbers [3]).transform.RotateAround (pos.position, pos.forward, -90);
			pos = ((TextMesh)currentNumbers [5]).transform;
			((TextMesh)currentNumbers [5]).transform.RotateAround (pos.position, pos.forward, 90);
			break;
		case Direction.Left:
			for (int i = 0; i < currentNumbers.Count; i++) {
				if (i != 3 && i != 5 && i != 1) {
					pos = ((TextMesh)currentNumbers [i]).transform;
					((TextMesh)currentNumbers [i]).transform.RotateAround (pos.position, pos.forward, -90);
				}
				if (i == 1) {
					pos = ((TextMesh)currentNumbers [i]).transform;
					((TextMesh)currentNumbers [i]).transform.RotateAround (pos.position, pos.forward, 180);
				}
				if (i == 5) {
					pos = ((TextMesh)currentNumbers [i]).transform;
					((TextMesh)currentNumbers [i]).transform.RotateAround (pos.position, pos.forward, 90);
				}
			}
			break;
		case Direction.Right:
			for (int i = 0; i < currentNumbers.Count; i++) {
				if (i != 1 && i != 5 && i != 2) {
					pos = ((TextMesh)currentNumbers [i]).transform;
					((TextMesh)currentNumbers [i]).transform.RotateAround (pos.position, pos.forward, 90);
				}
				if (i == 2) {
					pos = ((TextMesh)currentNumbers [i]).transform;
					((TextMesh)currentNumbers [i]).transform.RotateAround (pos.position, pos.forward, 180);
				}
				if (i == 5) {
					pos = ((TextMesh)currentNumbers [i]).transform;
					((TextMesh)currentNumbers [i]).transform.RotateAround (pos.position, pos.forward, -90);
				}
			}
			break;
		}
		lastDirection = d;
		onMovement = false;
		currentPos = transform.position;
		steps++;
	}

	void OnTriggerStay(Collider c){
		if (c.CompareTag ("Untagged")) {
			if (onMovement || calculated)
				return;
			print (c.GetComponent<Cell> ().stateCell);
			//comprueba que el calculo este bien
			//acepto para up y right, en ese caso comprueba que la celda haya sido pisada
			if (c.GetComponent<Cell> ().stateCell == Cell.StateCell.Normal) {
				int cellValue = c.GetComponent<Cell> ().number;
				int diceValueA = -100;
				int diceValueB = -1;
				switch (lastDirection) {
				case Direction.Up:
				//diceValueA = int.Parse (((TextMesh)currentNumbers [1]).text);
				//diceValueB = int.Parse (((TextMesh)currentNumbers [5]).text);
					cellValue = 0;
					break;
				case Direction.Down:
					diceValueA = int.Parse (((TextMesh)currentNumbers [1]).text);
					diceValueB = int.Parse (((TextMesh)currentNumbers [4]).text);
					break;
				case Direction.Left:
					diceValueA = int.Parse (((TextMesh)currentNumbers [1]).text);
					diceValueB = int.Parse (((TextMesh)currentNumbers [2]).text);
					break;
				case Direction.Right:
					cellValue = 0;
				//diceValueA = int.Parse (((TextMesh)currentNumbers [1]).text);
				//diceValueB = int.Parse (((TextMesh)currentNumbers [3]).text);
					break;
				}

 				print (diceValueA + " + " + diceValueB + " = " + cellValue);

				inGame.calculateResult (diceValueA, diceValueB, cellValue);

				c.GetComponent<Cell> ().changeState (Cell.StateCell.Passed);
				//Cambia el color del dado si toca una operacion
				if (nextOperation != currentOperation) {
					currentOperation = nextOperation;
					switch (currentOperation) {
					case Operation.Sum:
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (255, 116, 116, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 (255, 116, 116, 1));
						break;
					case Operation.Rest:
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (88, 179, 255, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 (88, 179, 255, 1));
						break;
					case Operation.Mult:
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (255, 88, 250, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 (255, 88, 250, 1));
						break;
					case Operation.Div:
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (138, 255, 116, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 (138, 255, 116, 1));
						break;
					}
				}
			}
			if (c.GetComponent<Cell> ().stateCell == Cell.StateCell.EndCell) {
				inGame.finishGame ();
			}
			calculated = true;
		} else {
			switch (c.tag) {
			case "Sum":
				changeOperation(Operation.Sum);
				break;
			case "Substraction":
				changeOperation(Operation.Rest);
				break;
			case "Multiplication":
				changeOperation(Operation.Mult);
				break;
			case "Division":
				changeOperation(Operation.Div);
				break;
			case "Rotate90CW":
				StartCoroutine (inGame.rotateCells (true));
				break;
			case "Rotate90CCW":
				StartCoroutine (inGame.rotateCells (false));
				break;
			}
			Destroy (c.gameObject);
		}
	}

	void changeOperation(Operation op){
		nextOperation = op;
	}

	// Update is called once per frame
	void Update () {
		if (onMovement || inGame.rotating)
			return;
		if (Input.GetKeyDown (KeyCode.W)) { StartCoroutine(turn (Direction.Up)); }
		if (Input.GetKeyDown (KeyCode.S)) { StartCoroutine(turn (Direction.Down)); }
		if (Input.GetKeyDown (KeyCode.A)) { StartCoroutine(turn (Direction.Left)); }
		if (Input.GetKeyDown (KeyCode.D)) { StartCoroutine(turn (Direction.Right)); }
	}
}
