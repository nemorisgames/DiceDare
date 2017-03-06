using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour {
	[HideInInspector]
	public bool onMovement = false;
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
	AudioSource audio;
	public AudioClip audioRotation;
	public AudioClip audioCubeChange;
	Transform plane;
	public int steps = 0;
	public Material backgroundMaterial;
	public Texture backgroundSum;
	public Texture backgroundSubstraction;
	public Texture backgroundMultiplication;
	public Texture backgroundDivision;
	float timeLastMove;
	float hintTime = 10f;
	LineRenderer line;
	public Material[] materialsLine;
	// Use this for initialization
	void Start () {
		plane = GameObject.Find ("Plane").GetComponent<Transform> ();
		line = GetComponent<LineRenderer> ();
		currentPos = transform.position;
		numbers.Add(transform.FindChild("TextUp").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextDown").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextLeft").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextRight").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextForward").GetComponent<TextMesh>());
		numbers.Add(transform.FindChild("TextBackward").GetComponent<TextMesh>());

		currentNumbers = numbers;

		inGame = Camera.main.GetComponent<InGame> ();
		audio = GetComponent<AudioSource> ();

		nextOperation = currentOperation;

		Camera.main.backgroundColor = new Color32 (253, 130, 138, 0);
		backgroundMaterial.mainTexture = backgroundSum;

		StartCoroutine(applyRootMotion ());

		timeLastMove = Time.timeSinceLevelLoad;
	}

	IEnumerator applyRootMotion(){
		yield return new WaitForSeconds (1.6f);
		transform.rotation = Quaternion.identity;
		GetComponent<Animator> ().applyRootMotion = true;
	}

	public IEnumerator turn(Direction d){
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
				if(plane != null)
					plane.position = new Vector3 (plane.position.x, plane.position.y, plane.position.z + 0.1f);
				break;
			case Direction.Down:
				transform.RotateAround (currentPos + new Vector3 (0f, -0.5f, -0.5f), Vector3.right, -90f / nStemps);
				if(plane != null)
				plane.position = new Vector3 (plane.position.x, plane.position.y, plane.position.z - 0.1f);
				break;
			case Direction.Left:
				transform.RotateAround (currentPos + new Vector3 (-0.5f, -0.5f, 0f), Vector3.forward, 90f / nStemps);
				if(plane != null)
				plane.position = new Vector3 (plane.position.x - 0.1f, plane.position.y, plane.position.z);
				break;
			case Direction.Right:
				transform.RotateAround (currentPos + new Vector3 (0.5f, -0.5f, 0f), Vector3.forward, -90f / nStemps);
				if(plane != null)
				plane.position = new Vector3 (plane.position.x + 0.1f, plane.position.y, plane.position.z);
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
		audio.pitch = Random.Range (0.95f, 1.05f);
		audio.PlayOneShot(audioRotation);
		steps++;
		timeLastMove = Time.timeSinceLevelLoad;
		hintTime = 10f;
	}

	void OnTriggerStay(Collider c){
		if (c.CompareTag ("Untagged") || c.CompareTag ("Sum") || c.CompareTag ("Substraction") || c.CompareTag ("Multiplication") || c.CompareTag ("Division")) {
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
				print(c.tag);
				switch (c.tag) {
				case "Sum":
					changeOperation (Operation.Sum);
					Camera.main.backgroundColor = new Color32 (253, 130, 138, 0);
					backgroundMaterial.mainTexture = backgroundSum;
					break;
				case "Substraction":
					changeOperation (Operation.Rest);
					Camera.main.backgroundColor = new Color32 (101, 193, 255, 0);
					backgroundMaterial.mainTexture = backgroundSubstraction;
					break;
				case "Multiplication":
					changeOperation (Operation.Mult);
					Camera.main.backgroundColor = new Color32(255, 168, 255, 0);
					backgroundMaterial.mainTexture = backgroundMultiplication;
					break;
				case "Division":
					changeOperation (Operation.Div);
					Camera.main.backgroundColor = new Color32(144, 255, 139, 0);
					backgroundMaterial.mainTexture = backgroundDivision;
					break;
				}
				if (nextOperation != currentOperation) {
					currentOperation = nextOperation;
					switch (currentOperation) {
					case Operation.Sum:
						audio.pitch = 1f;
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (255, 90, 118, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.3676471f), (int)(255 * 0.1621972f), (int)(255 * 0.191952f), (int)(255 * 0.3676471f)));
						line.material = materialsLine [0];
						break;
					case Operation.Rest:
						audio.pitch = 1.1f;
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (90, 112, 255, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.1621972f), (int)(255 * 0.2146223f), (int)(255 * 0.3676471f), (int)(255 * 0.3676471f)));
						line.material = materialsLine [1];
						break;
					case Operation.Mult:
						audio.pitch = 1.2f;
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (125, 0, 255, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.3223064f), (int)(255 * 0.1621972f), (int)(255 * 0.3676471f), (int)(255 * 0.3676471f)));
						line.material = materialsLine [2];
						break;
					case Operation.Div:
						audio.pitch = 1.3f;
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (60, 113, 46, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32((int)(255 * 0.1308764f), (int)(255 * 0.1985294f), (int)(255 * 0.07590831f), (int)(255 * 0.3676471f)));
						line.material = materialsLine [3];
						break;
					}
					audio.PlayOneShot(audioCubeChange);
				}
			}
			if (c.GetComponent<Cell> ().stateCell == Cell.StateCell.EndCell) {
				inGame.finishGame ();
			}
			calculated = true;
		} else {
			switch (c.tag) {
			case "Rotate90CW":
				Destroy (c.gameObject);
				StartCoroutine (inGame.rotateCells (true));
				break;
			case "Rotate90CCW":
				Destroy (c.gameObject);
				StartCoroutine (inGame.rotateCells (false));
				break;
			}
		}
	}

	void changeOperation(Operation op){
		nextOperation = op;
	}

	// Update is called once per frame
	void Update () {
		if (onMovement || inGame.rotating || Time.timeSinceLevelLoad < 2f || inGame.pause)
			return;
		if(timeLastMove <= Time.timeSinceLevelLoad - inGame.pauseTime - hintTime){
			StartCoroutine (inGame.lightPath (0));
			hintTime += 5f;
		}
		if (Input.GetKeyDown (KeyCode.W)) { StartCoroutine(turn (Direction.Up)); }
		if (Input.GetKeyDown (KeyCode.S)) { StartCoroutine(turn (Direction.Down)); }
		if (Input.GetKeyDown (KeyCode.A)) { StartCoroutine(turn (Direction.Left)); }
		if (Input.GetKeyDown (KeyCode.D)) { StartCoroutine(turn (Direction.Right)); }
	}
}
