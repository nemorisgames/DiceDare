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

	public void Init(int[] numbers){
		//Debug.Log(numbers[0]+", "+numbers[1]+", "+numbers[2]);
		int index = Random.Range(1,3);
		int sum = numbers[0] + numbers[index];
		int wrong = sum + (int)(Mathf.Sign(Random.Range(-1,1)))*numbers[(index == 1 ? 2 : 1)];
		while(wrong == numbers[0] + numbers[1])
			wrong += (int)(Mathf.Sign(Random.Range(-1,1)))*1;

		if(index == 1){
			Cell leftCell = instantiateCell(sum, inGame.cellNormal, LCell);
			Cell downCell = instantiateCell(wrong, inGame.cellNormal, DCell);
		}
		else{
			Cell leftCell = instantiateCell(wrong, inGame.cellNormal, LCell);
			Cell downCell = instantiateCell(sum, inGame.cellNormal, DCell);
		}
		
	}

	IEnumerator raiseCell(Cell cell, Transform target){
		float aux = target.position.y;
		float pos = cell.transform.position.y;
		while(pos < aux){
			pos += 0.2f;
			cell.transform.position = new Vector3(cell.transform.position.x, pos, cell.transform.position.z);
			yield return new WaitForSeconds(Time.deltaTime);
		}
		cell.transform.position = new Vector3(cell.transform.position.x, aux, cell.transform.position.z);
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
}
