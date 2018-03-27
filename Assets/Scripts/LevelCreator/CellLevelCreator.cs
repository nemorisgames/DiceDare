using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellLevelCreator : MonoBehaviour {
    public GameObject[] types;
    public int cellNumber = 0;
    public int cellType = 0;
    public bool inPath = false;
    public TextMesh numberLabel;
    public int pathNumber = -1;
    public TextMesh pathLabel;
    public bool visited = false;

    LevelCreator levelCreator;
    //public Vector2 position = Vector2.zero;
	// Use this for initialization
	void Start () {
        updateType();
	}

    void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            cellType++;
            if (cellType > 8) cellType = -2;
            updateType();
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            cellNumber += 1;
            updateType();
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            cellNumber += 10;
            updateType();
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            cellNumber -= 1;
            updateType();
        }

        if (Input.GetKey(KeyCode.Alpha5))
        {
            cellNumber -= 10;
            updateType();
        }

        if (Input.GetKey(KeyCode.Alpha6))
        {
            pathNumber += 1;
            updateType();
        }

        if (Input.GetKey(KeyCode.Alpha7))
        {
            pathNumber = Mathf.Clamp(pathNumber - 1, -1, 1000);
            updateType();
        }

        /*
        if (Input.GetKey(KeyCode.Alpha8))
        {
            pathNumber = Mathf.Clamp(pathNumber - 1, -1, 1000);
            updateType();
        }*/
    }

    void updateType()
    {
        for(int i = 0; i < types.Length; i++)
        {
            types[i].SetActive(i - 2 == cellType);
        }
        numberLabel.text = "" + cellNumber;
        pathLabel.text = "" + pathNumber;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
