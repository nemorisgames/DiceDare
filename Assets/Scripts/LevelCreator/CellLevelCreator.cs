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
            if (cellType > 9) cellType = -2;
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

    public void visit(bool v)
    {
        visited = v;
        if (v)
        {
            StartCoroutine(shine(100));
        }
    }

    void updateType()
    {
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i] != null)
                types[i].SetActive(i - 2 == cellType);
        }
        numberLabel.text = "" + cellNumber;
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
