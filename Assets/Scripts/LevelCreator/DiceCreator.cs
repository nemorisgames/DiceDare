using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCreator : MonoBehaviour {
    public TextMesh upLabel;
    public TextMesh rightLabel;
    public TextMesh leftLabel;
    public TextMesh downLabel;
    public TextMesh backwardLabel;
    public TextMesh forwardLabel;
    public Transform referenceDice;

    public Material[] materialOperations;
    public MeshRenderer meshRenderer;

    LevelCreator levelCreator;

    int up = 1;
    int down = 1;
    int left = 1;
    int right = 1;
    int forward = 1;
    int backward = 1;
    // Use this for initialization
    void Start () {
        levelCreator = Camera.main.GetComponent<LevelCreator>();

    }

    public void setFaces(int left, int up, int right)
    {
        write(up, 1, left, 1, 1, right);
    }

    public void setReferenceDice(float x, float y)
    {
        referenceDice.position = new Vector3(x, 0.2f, y);
    }

    public void write(int up, int down, int left, int right, int backward, int forward)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
        this.backward = backward;
        this.forward = forward;
        upLabel.text = "" + up;
        leftLabel.text = "" + left;
        rightLabel.text = "" + right;
        downLabel.text = "" + down;
        backwardLabel.text = "" + backward;
        forwardLabel.text = "" + forward;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RaycastHit hit;
            if (Physics.Raycast(referenceDice.position + referenceDice.right, -Vector3.up, out hit, 100.0f))
            {
                CellLevelCreator cellLevelCreator = hit.transform.GetComponent<CellLevelCreator>();
                string valueNextCell = hit.transform.Find("Text").GetComponent<TextMesh>().text;
                write(int.Parse((cellLevelCreator.visited) ?"" + right: valueNextCell), left, up, down, backward, forward);
                referenceDice.position = referenceDice.position + referenceDice.right;
                if (!cellLevelCreator.visited && cellLevelCreator.cellType >= 3 && cellLevelCreator.cellType <= 8)
                {
                    switch (cellLevelCreator.cellType)
                    {
                        //suma
                        case 3:
                            meshRenderer.material = materialOperations[0];
                            break;
                        //resta
                        case 4:
                            meshRenderer.material = materialOperations[1];
                            break;
                        //multi
                        case 5:
                            meshRenderer.material = materialOperations[2];
                            break;
                        //division
                        case 6:
                            meshRenderer.material = materialOperations[3];
                            break;
                        //giroCW
                        case 7:
                            levelCreator.turnCells(true);
                            break;
                        //giroCCW
                        case 8:
                            levelCreator.turnCells(false);
                            break;
                    }
                }
                cellLevelCreator.visit(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RaycastHit hit;
            if (Physics.Raycast(referenceDice.position + referenceDice.forward, -Vector3.up, out hit, 100.0f))
            {
                CellLevelCreator cellLevelCreator = hit.transform.GetComponent<CellLevelCreator>();
                string valueNextCell = hit.transform.Find("Text").GetComponent<TextMesh>().text;
                write(int.Parse((cellLevelCreator.visited) ? "" + backward : valueNextCell), forward, left, right, down, up);
                referenceDice.position = referenceDice.position + referenceDice.forward;
                if (!cellLevelCreator.visited && cellLevelCreator.cellType >= 3 && cellLevelCreator.cellType <= 8)
                {
                    switch (cellLevelCreator.cellType)
                    {
                        //suma
                        case 3:
                            meshRenderer.material = materialOperations[0];
                            break;
                        //resta
                        case 4:
                            meshRenderer.material = materialOperations[1];
                            break;
                        //multi
                        case 5:
                            meshRenderer.material = materialOperations[2];
                            break;
                        //division
                        case 6:
                            meshRenderer.material = materialOperations[3];
                            break;
                        //giroCW
                        case 7:
                            levelCreator.turnCells(true);
                            break;
                        //giroCCW
                        case 8:
                            levelCreator.turnCells(false);
                            break;
                    }
                }
                cellLevelCreator.visit(true);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RaycastHit hit;
            if (Physics.Raycast(referenceDice.position - referenceDice.right, -Vector3.up, out hit, 100.0f))
            {
                CellLevelCreator cellLevelCreator = hit.transform.GetComponent<CellLevelCreator>();
                //string valueNextCell = hit.transform.Find("Text").GetComponent<TextMesh>().text;
                write(left, right, down, up, backward, forward);
                referenceDice.position = referenceDice.position - referenceDice.right;
                cellLevelCreator.visit(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RaycastHit hit;
            if (Physics.Raycast(referenceDice.position - referenceDice.forward, -Vector3.up, out hit, 100.0f))
            {
                CellLevelCreator cellLevelCreator = hit.transform.GetComponent<CellLevelCreator>();
                //string valueNextCell = hit.transform.Find("Text").GetComponent<TextMesh>().text;
                write(forward, backward, left, right, up, down);
                referenceDice.position = referenceDice.position - referenceDice.forward;
                cellLevelCreator.visit(true);
            }
        }
    }
}
