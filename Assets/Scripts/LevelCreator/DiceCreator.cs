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

    int up = 1;
    int down = 1;
    int left = 1;
    int right = 1;
    int forward = 1;
    int backward = 1;
    // Use this for initialization
    void Start () {
		
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
                cellLevelCreator.visited = true;
                referenceDice.position = referenceDice.position + referenceDice.right;
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
                cellLevelCreator.visited = true;
                referenceDice.position = referenceDice.position + referenceDice.forward;
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
            }
        }
    }
}
