using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour {
	[HideInInspector]
	public bool onMovement = false;
	bool calculated = false;
	public enum Operation {Sum, Rest, Mult, Div, None};
	public Operation currentOperation = Operation.Sum;
	public enum Direction {Up, Down, Left, Right, None};
	public Direction lastDirection;
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
	public UITexture backgroundTexture;
	public ParticleSystem goodMove;
    bool dropped = false;
    // Use this for initialization
    bool swipe;
	public bool testDifficulty = false;
	public int opChanges = 0;
	public int spins = 0;
	public int passedCells = -1;
	private Renderer [] quads = new Renderer[6];
	private Animator animator;
	private Quaternion lastRotation;
	private Vector3 lastPosition;

	void Awake(){
		inGame = Camera.main.GetComponent<InGame> ();
		animator = GetComponent<Animator>();
        if (inGame.tutorial && transform.Find("QuadUp") != null)
        {
            quads[0] = transform.Find("QuadUp").GetComponent<Renderer>();
            quads[1] = transform.Find("QuadDown").GetComponent<Renderer>();
            quads[2] = transform.Find("QuadLeft").GetComponent<Renderer>();
            quads[3] = transform.Find("QuadRight").GetComponent<Renderer>();
            quads[4] = transform.Find("QuadForward").GetComponent<Renderer>();
            quads[5] = transform.Find("QuadBackward").GetComponent<Renderer>();
        }

    }

	void SetNumbers(){
		numbers.Add(transform.Find("TextUp").GetComponent<TextMesh>());
		numbers.Add(transform.Find("TextDown").GetComponent<TextMesh>());
		numbers.Add(transform.Find("TextLeft").GetComponent<TextMesh>());
		numbers.Add(transform.Find("TextRight").GetComponent<TextMesh>());
		numbers.Add(transform.Find("TextForward").GetComponent<TextMesh>());
		numbers.Add(transform.Find("TextBackward").GetComponent<TextMesh>());
		currentNumbers = numbers;
	}

	public void ResetNumberPositions(){
		transform.eulerAngles = new Vector3(
			(transform.eulerAngles.x > 0 ? Mathf.Floor(transform.eulerAngles.x) : Mathf.Ceil(transform.eulerAngles.x)),
			(transform.eulerAngles.y > 0 ? Mathf.Floor(transform.eulerAngles.y) : Mathf.Ceil(transform.eulerAngles.y)),
			(transform.eulerAngles.z > 0 ? Mathf.Floor(transform.eulerAngles.z) : Mathf.Ceil(transform.eulerAngles.z))
		);
		transform.Find("TextUp").localPosition = Vector3.up * 0.51f;
		transform.Find("TextDown").localPosition = Vector3.up * -0.51f;
		transform.Find("TextLeft").localPosition = Vector3.right * -0.51f;
		transform.Find("TextRight").localPosition = Vector3.right * 0.51f;
		transform.Find("TextForward").localPosition = Vector3.forward * -0.51f;
		transform.Find("TextBackward").localPosition = Vector3.forward * 0.51f;
	}

	public string OperationString(){
		switch(currentOperation){
			case Operation.Sum:
			return "+";
			case Operation.Rest:
			return "-";
			case Operation.Mult:
			return "x";
			case Operation.Div:
			return "/";
			default:
			return "";
		}
	}

	public void ExecAnim(string s){
		animator.SetTrigger(s);
	}

	void Start () {
		SetNumbers();
		plane = GameObject.Find ("Plane").GetComponent<Transform> ();
		line = GetComponent<LineRenderer> ();
		currentPos = transform.position;

        if (quads[0] != null)
        {
            for (int i = 0; i < quads.Length; i++)
            {
                quads[i].gameObject.SetActive(false);
            }
        }
		
		audio = GetComponent<AudioSource> ();

		nextOperation = currentOperation;

		Camera.main.backgroundColor = new Color32 (253, 130, 138, 0);
		backgroundMaterial.mainTexture = backgroundSum;

		StartCoroutine(applyRootMotion ());

		timeLastMove = Time.timeSinceLevelLoad;
		if (PlayerPrefs.GetInt ("Control",0) == 0) {
			ToggleSwipe (true);
		} else {
			ToggleSwipe (false);
		}

		if(inGame.daily) inGame.currentBlock.currentNumbers = faceNumbers();
		hintTime = 5 + getHintTime();
	}

	public void ShineFace(Vector3 face, int num){
		int index = 0;

		face.x = Mathf.Clamp(face.x,-1,1);
		face.y = Mathf.Clamp(face.y,-1,1);
		face.z = Mathf.Clamp(face.z,-1,1);
		if(face.x == 1)
			index = 3;
		else if(face.x == -1)
			index = 2;
		else if(face.y == 1)
			index = 0;
		else if(face.y == -1)
			index = 1;
		else if(face.z == 1)
			index = 5;
		else if(face.z == -1)
			index = 4;

		StartCoroutine(ShineFace(index,num));
	}

	public IEnumerator ShineFace(int index, int num){
		if(quads[index] == null)
			yield break;
		Material m = quads[index].material;
		quads[index].gameObject.SetActive(true);
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
		quads[index].gameObject.SetActive(false);
		m.SetColor ("_EmissionColor", colorDefault);
	}

	int getHintTime(){
		if(inGame.currentScene <= 5)
			return 5;
		if(inGame.currentScene > 5 && inGame.currentScene <= 10)
			return 10;
		if(inGame.currentScene > 10 && inGame.currentScene <= 15)
			return 15;
		if(inGame.currentScene > 15 && inGame.currentScene <= 20)
			return 20;
		if(inGame.currentScene > 20)
			return 25;
		else
			return 5;
	}

	IEnumerator applyRootMotion(){
		yield return new WaitForSeconds (1.6f);
		transform.rotation = Quaternion.identity;
		animator.applyRootMotion = true;
		if(!inGame.tutorial)
			inGame.UnPause();
		//if(inGame.daily)
            EnableTutorialSign(true);
		//if(inGame.tutorial)
		//	inGame.NextTutorial(false);
	}

	public void RollBack(){
		StartCoroutine(IRollBack());
	}

	IEnumerator IRollBack(){
		yield return new WaitForSeconds(0.25f);
		switch(lastDirection){
			case Direction.Down:
			StartCoroutine(turn(Direction.Up));
			break;
			case Direction.Up:
			StartCoroutine(turn(Direction.Down));
			break;
			case Direction.Left:
			StartCoroutine(turn(Direction.Right));
			break;
			case Direction.Right:
			StartCoroutine(turn(Direction.Left));
			break;
		}
	}

	public void RestoreLastRotation(){
		transform.rotation = lastRotation;
		transform.position = lastPosition;
	}

	public IEnumerator turn(Direction d){
		if(inGame.tutorial){
			//if(inGame.tutorialIndex == 0 && d != Direction.Left || inGame.tutorialIndex == 1 && d != Direction.Down || inGame.tutorialIndex == 3 && d != Direction.Left)
			//	yield break;
			//else
			//	inGame.NextTutorial(false);
			inGame.HideTutorialPanel();
		}
		//lastRotation = transform.rotation;
		//lastPosition = transform.position;
		//Debug.Log(lastPosition);
		EnableTutorialSign(false);
		onMovement = true;
		calculated = false;
		inGame.Pause();
		int nStemps = 10;
		RaycastHit r;
		//define el numero en la cara escondida
		switch(d){
		case Direction.Up:
			
			break;
		case Direction.Down:
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
			//if(!Physics.Raycast(transform.position - Vector3.forward + Vector3.up, new Vector3(0f, -1f, 0f), out r, 5f,LayerMask.GetMask(new string[1]{"Cell"}))){
				//Debug.Log("nope");
			//}
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

        //ejecuta la funcion para mostrar numeros con cada giro
        DissapearNumber[] dissapearNumbers = GameObject.FindObjectsOfType<DissapearNumber>();
        foreach(DissapearNumber dis in dissapearNumbers)
        {
            dis.Show();
        }
	}

	public int[] faceNumbers(){
		ArrayList list = currentNumbers;
		/*switch(lastDirection){
			case Direction.Down:
			TextMesh t = ((TextMesh)list [1]);
			list [1] = list [4];
			list [4] = list [0];
			list [0] = list [5];
			list [5] = t;
			break;
		case Direction.Left:
			t = ((TextMesh)list [1]);
			list [1] = list [2];
			list [2] = list [0];
			list [0] = list [3];
			list [3] = t;
			break;
		}*/
		//Debug.Log(int.Parse (((TextMesh)list [1]).text)+", "+int.Parse (((TextMesh)list [2]).text)+", "+int.Parse (((TextMesh)list [4]).text));
		return new int[]{int.Parse (((TextMesh)list [0]).text),int.Parse (((TextMesh)list [2]).text),int.Parse (((TextMesh)list [4]).text)};
	}

	void EvaluarDificultad(string tag, Cell c){
        if (c == null) return;
		switch(tag){
			case "Rotate90CW":
			case "Rotate90CCW":
			spins++;
			break;
			default:
			break;
		}
		if (onMovement || calculated)
			return;
		if(c.stateCell == Cell.StateCell.Passed)
			passedCells++;
		else{
			switch(tag){
				case "Sum":
				if(currentOperation != Operation.Sum)
					opChanges++;
				break;
				case "Substraction":
				if(currentOperation != Operation.Rest)
					opChanges++;
				break;
				case "Multiplication":
				if(currentOperation != Operation.Mult)
					opChanges++;
				break;
				case "Division":
				if(currentOperation != Operation.Div)
					opChanges++;
				break;
				default:
				break;
			}
		}
	}

	void OnTriggerStay(Collider c){
		Cell cell = c.GetComponent<Cell>();
		EvaluarDificultad(c.tag,cell);
		if (c.CompareTag ("Untagged") || c.CompareTag ("Sum") || c.CompareTag ("Substraction") || c.CompareTag ("Multiplication") || c.CompareTag ("Division")) {
			if (onMovement || calculated)
				return;
			//print (c.GetComponent<Cell> ().stateCell);
			if((!inGame.daily || (inGame.tutorial && cell.stateCell == Cell.StateCell.Passed && inGame.TutorialIndex() > 0)) && inGame.pause)
				inGame.UnPause();
			//comprueba que el calculo este bien
			//acepto para up y right, en ese caso comprueba que la celda haya sido pisada

			if (cell.stateCell == Cell.StateCell.Normal) {
				int cellValue = cell.number;
				int diceValueA = -100;
				int diceValueB = -1;
				switch (lastDirection) {
				case Direction.Up:
				//diceValueA = int.Parse (((TextMesh)currentNumbers [1]).text);
				//diceValueB = int.Parse (((TextMesh)currentNumbers [5]).text);
					cellValue = -1;
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
					cellValue = -1;
				//diceValueA = int.Parse (((TextMesh)currentNumbers [1]).text);
				//diceValueB = int.Parse (((TextMesh)currentNumbers [3]).text);
					break;
				}

 				print (diceValueA + " + " + diceValueB + " = " + cellValue);
				//inGame.currentBlock.currentNumbers = faceNumbers();
				bool correct = inGame.calculateResult (diceValueA, diceValueB, cellValue,cell);

				//Cambia el color del dado si toca una operacion
				print(c.tag);
				if(correct){
					switch (c.tag) {
						case "Sum":
							changeOperation (Operation.Sum);
							backgroundTexture.color = new Color (226f/255f, 54f/255f, 78f/255f, 255f/255f);
							backgroundMaterial.mainTexture = backgroundSum;
							break;
						case "Substraction":
							changeOperation (Operation.Rest);
							backgroundTexture.color = new Color (27f/255f, 88f/255f, 149f/255f, 255f/255f);
							backgroundMaterial.mainTexture = backgroundSubstraction;
							break;
						case "Multiplication":
							changeOperation (Operation.Mult);
							backgroundTexture.color = new Color (116f/255f, 20f/255f, 106f/255f, 255f/255f);
							backgroundMaterial.mainTexture = backgroundMultiplication;
							break;
						case "Division":
							changeOperation (Operation.Div);
							backgroundTexture.color = new Color (20f/255f, 116f/255f, 104f/255f, 255f/255f);
							backgroundMaterial.mainTexture = backgroundDivision;
							break;

						case "Death":
							break;
					}
				}
				
				if (nextOperation != currentOperation) {
					currentOperation = nextOperation;
					switch (currentOperation) {
					case Operation.Sum:
						audio.pitch = 1f;
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (255, 90, 118, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.3676471f), (int)(255 * 0.1621972f), (int)(255 * 0.191952f), (int)(255 * 0.3676471f)));
						//line.material = materialsLine [0];
						break;
					case Operation.Rest:
						audio.pitch = 1.1f;
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (90, 112, 255, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.1621972f), (int)(255 * 0.2146223f), (int)(255 * 0.3676471f), (int)(255 * 0.3676471f)));
						//line.material = materialsLine [1];
						break;
					case Operation.Mult:
						audio.pitch = 1.2f;
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (125, 0, 255, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.3223064f), (int)(255 * 0.1621972f), (int)(255 * 0.3676471f), (int)(255 * 0.3676471f)));
						//line.material = materialsLine [2];
						break;
					case Operation.Div:
						audio.pitch = 1.3f;
						GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (60, 113, 46, 255));
						GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32((int)(255 * 0.1308764f), (int)(255 * 0.1985294f), (int)(255 * 0.07590831f), (int)(255 * 0.3676471f)));
						//line.material = materialsLine [3];
						break;
					}
					audio.PlayOneShot(audioCubeChange);
				}
			}
			if (cell.stateCell == Cell.StateCell.EndCell) {
				cell.stateCell = Cell.StateCell.Passed;
				inGame.finishGame ();
				if(testDifficulty){
					Debug.Log("Giros: "+spins);
					Debug.Log("Cuadros retrocedidos: "+passedCells);
					Debug.Log("Cambios operacion: "+opChanges);
				}
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
			case "Death":
                    if (!dropped)
                    {
                        dropped = true;
                        StartCoroutine(Drop());
                    }
				break;
			}
		}
	}

	public void changeOperation(Operation op){
		nextOperation = op;
		UpdateTutorialSign(op);
		EnableTutorialSign(false);
		//if(inGame.daily){
			switch(op){
				case Dice.Operation.Sum:
					backgroundTexture.color = new Color (226f/255f, 54f/255f, 78f/255f, 255f/255f);
					backgroundMaterial.mainTexture = backgroundSum;
					GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (255, 90, 118, 255));
					GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.3676471f), (int)(255 * 0.1621972f), (int)(255 * 0.191952f), (int)(255 * 0.3676471f)));	
					break;
				case Dice.Operation.Div:
					backgroundTexture.color = new Color (20f/255f, 116f/255f, 104f/255f, 255f/255f);
					backgroundMaterial.mainTexture = backgroundDivision;
					GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (60, 113, 46, 255));
					GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32((int)(255 * 0.1308764f), (int)(255 * 0.1985294f), (int)(255 * 0.07590831f), (int)(255 * 0.3676471f)));
					break;
				case Dice.Operation.Mult:
					backgroundTexture.color = new Color (116f/255f, 20f/255f, 106f/255f, 255f/255f);
					backgroundMaterial.mainTexture = backgroundMultiplication;
					GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (125, 0, 255, 255));
					GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.3223064f), (int)(255 * 0.1621972f), (int)(255 * 0.3676471f), (int)(255 * 0.3676471f)));	
					break;
				case Dice.Operation.Rest:
					backgroundTexture.color = new Color (27f/255f, 88f/255f, 149f/255f, 255f/255f);
					backgroundMaterial.mainTexture = backgroundSubstraction;
					GetComponent<Renderer> ().material.SetColor ("_Color", new Color32 (90, 112, 255, 255));
					GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color32 ((int)(255 * 0.1621972f), (int)(255 * 0.2146223f), (int)(255 * 0.3676471f), (int)(255 * 0.3676471f)));	
					break;
			}
            if (currentOperation != op)
            {
                currentOperation = op;
                EnableTutorialSign(true);
                print("adentro");
            }
            else
            {
                EnableTutorialSign(false);
                print("afuera");
            }
		//}
	}
					

	IEnumerator Drop(){
        dropped = true;
		inGame.GetComponent<CameraControl> ().follow = false;
		for (int i = 0; i < 50; i++) {
			transform.position = new Vector3 (transform.position.x, transform.position.y - ((i*i)/20f / (50f)), transform.position.z);
			yield return new WaitForSeconds (0.001f);
			if (i == 25) {
				inGame.badMove ();
			}
		}
    }
		

	public void ToggleSwipe(bool b){
		if (b) {
			swipe = true;
			plane.gameObject.SetActive (false);
			PlayerPrefs.SetInt ("Control", 0);
		} else {
			swipe = false;
			plane.gameObject.SetActive (true);
			PlayerPrefs.SetInt ("Control", 1);
		}
	}

	Vector3 initialPosition;
	float timeSwipe;

	/*public void EnableAdjCells(){
		foreach (Transform t in inGame.adjacentCells) {
			t.GetComponent<AdjacentCellFinder> ().EnableCell (true);
		}
	}*/

	private int shineIndex = 0;
	void Update () {
		if(Input.GetKeyDown(KeyCode.L)){
			StartCoroutine(ShineFace(shineIndex,1));
			shineIndex++;
			if(shineIndex == 6)
				shineIndex = 0;
		}
		inGame.tutorialv2.position = transform.position;
		if (onMovement || inGame.rotating || Time.timeSinceLevelLoad < 2f || inGame.pause)
			return;
		if(!inGame.daily && timeLastMove <= Time.timeSinceLevelLoad - inGame.pauseTime - hintTime){
			StartCoroutine (inGame.lightPath (0));
			//fix
			hintTime += getHintTime();
		}
		if (Input.GetKeyDown (KeyCode.W)) { if(!inGame.daily || inGame.tutorial) StartCoroutine(turn (Direction.Up)); }
		if (Input.GetKeyDown (KeyCode.S)) { StartCoroutine(turn (Direction.Down)); }
		if (Input.GetKeyDown (KeyCode.A)) { StartCoroutine(turn (Direction.Left)); }
		if (Input.GetKeyDown (KeyCode.D)) { if(!inGame.daily || inGame.tutorial) StartCoroutine(turn (Direction.Right));}

		if (swipe) {
			if (Input.GetMouseButtonDown (0)) {
				initialPosition = Input.mousePosition;
				timeSwipe = Time.time;
			}
			if (Input.GetMouseButtonUp (0)) {
				if (timeSwipe + 1f > Time.time && Vector3.Distance (initialPosition, Input.mousePosition) >= 50f) {
					Vector3 dir = (Input.mousePosition - initialPosition).normalized;
					print ("swipe!" + Vector3.Angle (new Vector3 (1f, 0f, 0f), dir));
					float angle = Vector3.Angle (new Vector3 (1f, 0f, 0f), dir);
					if (angle >= 0f && angle < 90f) {
						if (dir.y > 0f) {
							if(!inGame.daily || inGame.tutorial) StartCoroutine (turn (Direction.Right));
						} else {
							StartCoroutine (turn (Direction.Down));
						}
					} else {
						if (dir.y > 0f) {
							if(!inGame.daily || inGame.tutorial) StartCoroutine (turn (Direction.Up));
						} else {
							StartCoroutine (turn (Direction.Left));
						}
					}
				}
			}
		}
		
	}

	void UpdateTutorialSign(Operation op){
		string sign = "";
		switch(op){
			case Operation.Sum:
			sign = "+";
			break;
			case Operation.Div:
			sign = "÷";
			break;
			case Operation.Mult:
			sign = "x";
			break;
			case Operation.Rest:
			sign = "-";
			break;
		}
		inGame.tutorialv2.Find("Canvas/LSign").GetComponent<Text>().text = sign;
		inGame.tutorialv2.Find("Canvas/DSign").GetComponent<Text>().text = sign;
	}

	public void EnableTutorialSign(bool b){
		/*if(inGame.pause)
			return;*/
		//Debug.Log("enable "+b);
		if(PlayerPrefs.GetInt("tutorialsDisabled",0) == 1){
			inGame.tutorialv2.gameObject.SetActive(false);
			return;
		}
		
		if(b)
			UpdateTutorialSign(currentOperation);
		
		//inGame.tutorialv2.gameObject.SetActive(b);
	}

	public void SetNumbers(int left, int forward, int top, int right, int backward, int bottom){
		((TextMesh)currentNumbers[0]).text = top.ToString();
		((TextMesh)currentNumbers[1]).text = bottom.ToString();
		((TextMesh)currentNumbers[2]).text = left.ToString();
		((TextMesh)currentNumbers[3]).text = right.ToString();
		((TextMesh)currentNumbers[4]).text = forward.ToString();
		((TextMesh)currentNumbers[5]).text = backward.ToString();
		//SetNumbers();
	}
}
