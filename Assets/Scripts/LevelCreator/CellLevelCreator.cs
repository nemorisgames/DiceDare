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
    public bool approx = false;

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
            if (cellType > 12) cellType = -2;
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

        if (Input.GetKey(KeyCode.Alpha8))
        {
            approx = !approx;
            updateType();
        }

        if (Input.GetKey(KeyCode.Alpha9))
        {
            pathNumber = -1;
            updateType();
        }

        //Celdas especificas
        if(Input.GetKey(KeyCode.Q)){
            cellType = 0;
            updateType();
        }

        if(Input.GetKey(KeyCode.E)){
            cellType = 1;
            updateType();
        }

        if(Input.GetKey(KeyCode.R)){
            cellType = 3;
            updateType();
        }

        if(Input.GetKey(KeyCode.T)){
            cellType = 4;
            updateType();
        }

        if(Input.GetKey(KeyCode.Y)){
            cellType = 5;
            updateType();
        }

        if(Input.GetKey(KeyCode.U)){
            cellType = 6;
            updateType();
        }

        if(Input.GetKey(KeyCode.F)){
            cellType = 7;
            updateType();
        }

        if(Input.GetKey(KeyCode.G)){
            cellType = 8;
            updateType();
        }

        if(Input.GetKey(KeyCode.I)){
            cellType = 9;
            updateType();
        }

        if(Input.GetKey(KeyCode.H)){
            cellType = 10;
            updateType();
        }

        if(Input.GetKey(KeyCode.J)){
            cellType = 11;
            updateType();
        }

        if(Input.GetKey(KeyCode.K)){
            cellType = 12;
            updateType();
        }

        if(Input.GetKey(KeyCode.O)){
            cellType = -2;
            updateType();
        }

        if(Input.GetKey(KeyCode.P)){
            cellType = -1;
            updateType();
        }

        /*
        if (Input.GetKey(KeyCode.Alpha8))
        {
            pathNumber = Mathf.Clamp(pathNumber - 1, -1, 1000);
            updateType();
        }*/
    }

    public void visit(bool v)
    {
        visited = v;
        if (v)
        {
            StartCoroutine(shine(100));
        }
    }

    public void updateType()
    {
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i] != null)
                types[i].SetActive(i - 2 == cellType);
        }
        numberLabel.text = (approx?"*":"") + cellNumber;
        pathLabel.text = "" + pathNumber;
    }

    public IEnumerator shine(int num)
    {
        print("shine");
        Material m = transform.Find("Shine").GetComponent<Renderer>().material;
        Color32 colorDefault = m.GetColor("_EmissionColor");
        for (int j = 0; j < num; j++)
        {
            for (int i = 0; i < 15; i++)
            {
                yield return new WaitForSeconds(0.01f);
                m.SetColor("_EmissionColor", Color.yellow * i * 0.01f);
            }
            for (int i = 15; i > 0; i--)
            {
                yield return new WaitForSeconds(0.01f);
                m.SetColor("_EmissionColor", Color.yellow * i * 0.01f);
            }
        }
        m.SetColor("_EmissionColor", colorDefault);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
