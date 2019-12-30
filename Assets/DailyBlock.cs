using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBlock : MonoBehaviour {
	public InGame inGame;
	public int [] currentNumbers;
	Transform LCell;
	Transform DCell;

	void Awake(){
		inGame = Camera.main.GetComponent<InGame>();
		LCell = transform.Find("LCell");
		DCell = transform.Find("DCell");
	}

	// Use this for initialization
	void Start () {
		/*Cell leftCell = instantiateCell(inGame.cellNormal, LCell);
		Cell downCell = instantiateCell(inGame.cellNormal, DCell);*/
	}

	public void Init(int[] numbers, Dice.Operation operation){
		//Debug.Log(numbers[0]+", "+numbers[1]+", "+numbers[2]);
		int index = Random.Range(1,3);
		int rightOperation = 0, wrongOperation = 0;
		switch(operation){
			case Dice.Operation.Sum:
				rightOperation = numbers[0] + numbers[index];
				wrongOperation = numbers[0] + numbers[(index == 1 ? 2 : 1)] + wrongRange();
				while(wrongOperation == numbers[0] + numbers[1])
					wrongOperation += (int)(Mathf.Sign(Random.Range(-1,1)))*1;
				break;
			case Dice.Operation.Mult:
				rightOperation = numbers[0] * numbers[index];
				wrongOperation = numbers[0] * numbers[(index == 1 ? 2 : 1)] + (int)(Mathf.Sign(Random.Range(-1,1)))*numbers[(index == 1 ? 2 : 1)];
				while(wrongOperation == numbers[0] + numbers[1])
					wrongOperation += (int)(Mathf.Sign(Random.Range(-1,1)))*1;
				break;
			case Dice.Operation.Div:
				rightOperation = (int)(numbers[0] / numbers[index]);
				if(numbers[(index == 1 ? 2 : 1)] != 0)
					wrongOperation = (int)(numbers[0] / numbers[(index == 1 ? 2 : 1)]) + wrongRange();
				else
					wrongOperation = (int)(numbers[0] / numbers[(index == 1 ? 2 : 1)] + 1) + wrongRange();
				while(wrongOperation == numbers[0] + numbers[1])
					wrongOperation += (int)(Mathf.Sign(Random.Range(-1,1)))*1;
				break;
			case Dice.Operation.Rest:
				rightOperation = numbers[0] - numbers[index];
				wrongOperation = numbers[0] - numbers[(index == 1 ? 2 : 1)] + wrongRange();
				while(wrongOperation == numbers[0] + numbers[1])
					wrongOperation += (int)(Mathf.Sign(Random.Range(-1,1)))*1;
			break;
		}

		if(index == 1){
			Cell leftCell = instantiateCell(rightOperation, inGame.cellNormal, LCell);
			Cell downCell = instantiateCell(wrongOperation, inGame.cellNormal, DCell);
		}
		else{
			Cell leftCell = instantiateCell(wrongOperation, inGame.cellNormal, LCell);
			Cell downCell = instantiateCell(rightOperation, inGame.cellNormal, DCell);
		}
	}

	public void InitOperationCell(int pos, Dice.Operation operation, int cellNumber){
		pos = Mathf.Clamp(pos,0,1);
		//Debug.Log(pos);
		GameObject cell = inGame.cellNormal;
		switch(operation){
			case Dice.Operation.Div:
				cell = inGame.cellDivision;
			break;
			case Dice.Operation.Mult:
				cell = inGame.cellMultiplication;
			break;
			case Dice.Operation.Rest:
				cell = inGame.cellSubstraction;
			break;
			case Dice.Operation.Sum:
				cell = inGame.cellSum;
			break;
		}
		Cell leftCell = instantiateCell(cellNumber, cell, (pos == 0 ? LCell : DCell));
	}

	public void InitNormalCell(int pos, int cellNumber, bool endCell = false){
		pos = Mathf.Clamp(pos,0,1);
		//Debug.Log(pos);

		Cell leftCell = instantiateCell(cellNumber, (endCell ? inGame.cellEnd : inGame.cellNormal), (pos == 0 ? LCell : DCell));
	}
	

	public void InitCellDown(int[] numbers, Dice.Operation operation, int cellNumber){
		Cell downCell = instantiateCell(cellNumber, inGame.cellNormal, DCell);
	}

	int wrongRange(){
		int randomSign = (int)(Mathf.Sign(Random.Range(-1,1)));
		int randomNumber = Random.Range(1,3);
		return randomSign * randomNumber;
	}

	IEnumerator raiseCell(Cell cell, Transform target){
		float aux = target.position.y;
		float pos = cell.transform.position.y;
		while(pos < aux){
			pos += 0.4f;
			cell.transform.position = new Vector3(cell.transform.position.x, pos, cell.transform.position.z);
			yield return new WaitForSeconds(Time.deltaTime);
		}
		cell.transform.position = new Vector3(cell.transform.position.x, aux, cell.transform.position.z);
		if(inGame.pause)
			inGame.UnPause();
	}

	Cell instantiateCell(int sum, GameObject go, Transform pos){
		GameObject aux = (GameObject)Instantiate(go, new Vector3(pos.position.x, pos.position.y - 8f, pos.position.z), go.transform.rotation,this.transform);
		Cell c = aux.GetComponent<Cell>();
		c.Init(sum);
		//Debug.Log(numbers[0] + numbers[2]);
		StartCoroutine(raiseCell(c,pos));
		return c;
	}

	public void DropRemainingBlocks(){
		Cell[] cells = GetComponentsInChildren<Cell>();
		for(int i=0;i<cells.Length;i++){
			if(cells[i].stateCell != Cell.StateCell.Passed || cells[i].name == "CellBegin"){
				cells[i].GetComponent<Rigidbody>().isKinematic = false;
				cells[i].GetComponent<Rigidbody>().useGravity = true;
			}
		}
	}

	public void DropPassedBlocks(){
		Cell[] cells = GetComponentsInChildren<Cell>();
		for(int i=0;i<cells.Length;i++){
			if(cells[i].stateCell == Cell.StateCell.Passed){
				cells[i].GetComponent<Rigidbody>().isKinematic = false;
				cells[i].GetComponent<Rigidbody>().useGravity = true;
			}
		}
	}

	IEnumerator destroyAfterSeconds(float f){
		yield return new WaitForSeconds(2f);
		Destroy(this.gameObject);
	}
}
