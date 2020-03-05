using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCreator : MonoBehaviour
{
    public Vector2 boardSize = Vector2.one;
    public GameObject cellConfig;
    //public Vector2 beginCell = Vector2.zero;
    public UILabel boardSizeLabel;

    string Scene = "";
    string SceneNumbers = "";
    string ScenePath = "";

    ArrayList board = new ArrayList();

    public DiceCreator dice;

    public UIInput[] importMatrix;
    public UIInput importSceneNumber;

    Transform cells;
    public UIInput diceLeft;
    public UIInput diceUp;
    public UIInput diceRight;

    Vector3 diceBeginValues = Vector3.one;
    // Use this for initialization
    void Start()
    {
        //PlayerPrefs.DeleteAll();

        cells = GameObject.Find("Cells").transform;
        Scene = PlayerPrefs.GetString("SceneTest");
        SceneNumbers = PlayerPrefs.GetString("SceneNumbersTest");
        ScenePath = PlayerPrefs.GetString("ScenePathTest");
        resetDice("1", "1", "1");

        if (Scene != "")
        {
            /*print(Scene);
            print(SceneNumbers);
            print(ScenePath);*/
            Debug.Log("Scene: "+Scene+"\n"+"SceneNumbers: "+SceneNumbers+"\n"+"ScenePath: "+ScenePath);
            //Se crea el board
            string[] aux = Scene.Split(new char[1] { '$' });
            string[] info = aux[0].Split(new char[1] { '|' });
            boardSize = new Vector2(int.Parse(info[1]), int.Parse(info[0]));
            createBoard();
            updateBoardLabel();

            composeBoard();
        }
        GlobalVariables.SetScenes();
        //dice.write(2, 3, 4);
    }

    public void addBoardSizeH() { boardSize.x = Mathf.Clamp(boardSize.x + 1, 1, 15); updateBoardLabel(); }
    public void removeBoardSizeH() { boardSize.x = Mathf.Clamp(boardSize.x - 1, 1, 15); updateBoardLabel(); }
    public void addBoardSizeV() { boardSize.y = Mathf.Clamp(boardSize.y + 1, 1, 15); updateBoardLabel(); }
    public void removeBoardSizeV() { boardSize.y = Mathf.Clamp(boardSize.y - 1, 1, 15); updateBoardLabel(); }
    /*
    public void addIniH() { beginCell.x = Mathf.Clamp(beginCell.x + 1, 0, 15); updateBoardLabel(); }
    public void removeIniH() { beginCell.x = Mathf.Clamp(beginCell.x - 1, 0, 15); updateBoardLabel(); }
    public void addIniV() { beginCell.y = Mathf.Clamp(beginCell.y + 1, 0, 15); updateBoardLabel(); }
    public void removeIniV() { beginCell.y = Mathf.Clamp(beginCell.y - 1, 0, 15); updateBoardLabel(); }
    */
    void updateBoardLabel()
    {
        boardSizeLabel.text = "Board: H" + boardSize.x + " , " + "V" + boardSize.y;
    }

    public void import()
    {
        if(importMatrix.Length == 3 && importMatrix[0].value.Length > 0 && importMatrix[1].value.Length > 0 && importMatrix[2].value.Length > 0)
        {
            string[] pathAux1 = importMatrix[2].value.Replace('"', ' ').Replace('+', ' ').Split(new char[1] { '|' });
            importMatrix[2].value = "";
            for(int i = 0; i < pathAux1.Length; i++)
            {
                string[] pathAux2 = pathAux1[i].Split(new char[1] { ',' });
                importMatrix[2].value += pathAux2[1] + "," + pathAux2[0] + "|";
            }
            importMatrix[2].value = importMatrix[2].value.Substring(0, importMatrix[2].value.Length - 1);
            PlayerPrefs.SetString("SceneTest", importMatrix[0].value.Replace('"', ' ').Replace('+', ' '));
            PlayerPrefs.SetString("SceneNumbersTest", importMatrix[1].value.Replace('"', ' ').Replace('+', ' '));
            PlayerPrefs.SetString("ScenePathTest", importMatrix[2].value);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            int levelRequested;
            if (importSceneNumber.value != "" && int.TryParse(importSceneNumber.value, out levelRequested))
            {
                PlayerPrefs.SetString("SceneTest", GlobalVariables.Scene[levelRequested]);
                PlayerPrefs.SetString("SceneNumbersTest", GlobalVariables.SceneNumbers[levelRequested]);
                PlayerPrefs.SetString("ScenePathTest", GlobalVariables.ScenePath[levelRequested]);

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void createBoard()
    {
        for (int i = 0; i < board.Count; i++)
        {
            Destroy((GameObject)board[i]);
        }
        board.Clear();
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                GameObject g = (GameObject)Instantiate(cellConfig, new Vector3(j, -0.1f, -i), Quaternion.identity);
                g.transform.parent = cells;
                CellLevelCreator c = g.GetComponent<CellLevelCreator>();
                /*if(j - beginCell.x == 0 && -i + beginCell.y == 0)
                {
                    c.cellType = -1;
                }*/
                board.Add(g);
            }
        }
        resetDice(diceLeft.value, diceUp.value, diceRight.value);
    }

    public void composeBoard()
    {
        //Scene = boardSize.y + "|" + boardSize.x;
        string[] SceneAux = Scene.Split(new char[1] { '$' })[1].Split(new char[1] { '|' });
        string[] SceneNumbersAux = SceneNumbers.Split(new char[1] { '$' })[1].Split(new char[1] { '|' });
        string[] ScenePathAux = ScenePath.Split(new char[1] { '|' });
        int contador = 0;
        for (int i = 0; i < (int)boardSize.y; i++)
        {
            for (int j = 0; j < (int)boardSize.x; j++)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(j, 0.5f, -i), -Vector3.up, out hit, 100.0f))
                {
                    //print(int.Parse(SceneAux[contador]));
                    //completa los tipos de celdas
                    hit.transform.GetComponent<CellLevelCreator>().cellType = int.Parse(SceneAux[contador]);

                    //completa los numeros de las celdas
                    if (SceneNumbersAux[contador].Contains("*"))
                    {
                        hit.transform.GetComponent<CellLevelCreator>().approx = true;
                        hit.transform.GetComponent<CellLevelCreator>().cellNumber = (int.Parse(SceneNumbersAux[contador].Remove(0, 2)));
                        hit.transform.GetComponent<CellLevelCreator>().updateType();
                    }
                    else
                    {
                        hit.transform.GetComponent<CellLevelCreator>().cellNumber = int.Parse(SceneNumbersAux[contador]);
                    }

                    //completa el path
                    hit.transform.GetComponent<CellLevelCreator>().pathNumber = getPathNumber(ScenePathAux, i, j);
                    contador++;
                }
            }
        }
        string[] diceNumbers = SceneNumbers.Split(new char[1] { '$' })[0].Split(new char[1] { '|' });
        diceLeft.value = diceNumbers[1];
        diceUp.value = diceNumbers[0];
        diceRight.value = diceNumbers[2];
        resetDice(diceNumbers[1], diceNumbers[0], diceNumbers[2]);
    }

    int getPathNumber(string[] vector, int hor, int ver)
    {
        
        for(int i = 0; i < vector.Length; i++)
        {
            string[] coordenadas = vector[i].Split(new char[1] { ',' });
            if (hor == int.Parse(coordenadas[0]) && ver == int.Parse(coordenadas[1]))
            {
                return i + 1;
            }
        }
        return -1; 
    }

    public void export()
    {
        cells.rotation = Quaternion.identity;
        cells.position = Vector3.zero;
        Scene = boardSize.y + "|" + boardSize.x;
        SceneNumbers = (int)diceBeginValues.y + "|" + (int)diceBeginValues.x + "|" + (int)diceBeginValues.z + "$";
        ScenePath = "";
        string SceneAux = "";
        string[] ScenePathAux = getScenePathVector();
        for(int i = 0; i < (int)boardSize.y; i++)
        {
            for (int j = 0; j < (int)boardSize.x; j++)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(j, 0.5f, -i), -Vector3.up, out hit, 100.0f))
                {
                    //completa los tipos de celdas
                    SceneAux += " " + hit.transform.GetComponent<CellLevelCreator>().cellType + " |";
                    //si es del tipo inicio, copia sus coordenadas al principio del string
                    if(hit.transform.GetComponent<CellLevelCreator>().cellType == -1)  Scene += "|" + (j) + "|" + (i) + "|0$";
                    
                    //completa los numeros de las celdas
                    SceneNumbers += " " + (hit.transform.GetComponent<CellLevelCreator>().approx?"*":"") + hit.transform.GetComponent<CellLevelCreator>().cellNumber + " |";
                    //completa el path
                    if (hit.transform.GetComponent<CellLevelCreator>().pathNumber != -1)
                        ScenePathAux[hit.transform.GetComponent<CellLevelCreator>().pathNumber - 1] = i + "," + j;
                }
            }
        }

        for(int i = 0; i < ScenePathAux.Length; i++)
        {
            ScenePath += ScenePathAux[i] + "|";
        }
        ScenePath = ScenePath.Substring(0, Mathf.Clamp(ScenePath.Length - 1,0,int.MaxValue));
        Scene += SceneAux;
        /*print(Scene);
        print(SceneNumbers);
        print(ScenePath);*/
        Debug.Log("Scene: "+Scene+"\n"+"SceneNumbers: "+SceneNumbers+"\n"+"ScenePath: "+ScenePath);
    }

    public string[] getScenePathVector()
    {
        int contador = 0;
        int quantity = 0;
        for (int i = 0; i < (int)boardSize.y; i++)
        {
            for (int j = 0; j < (int)boardSize.x; j++)
            {
                if(((GameObject)board[contador]).GetComponent<CellLevelCreator>().pathNumber != -1)
                {
                    quantity++;
                }
                contador++;
            }
        }
        string[] ret = new string[quantity];
        return ret;
    }

    public void test()
    {
        export();
        PlayerPrefs.SetString("SceneTest", Scene);
        PlayerPrefs.SetString("SceneNumbersTest", SceneNumbers);
        PlayerPrefs.SetString("ScenePathTest", ScenePath);

        SceneManager.LoadScene("InGameTest");
    }

    public void turnCells(bool CW)
    {
        cells.RotateAround(dice.referenceDice.transform.position, Vector3.up, (CW ? 90f : -90f));
    }

    public void resetDice(string left, string up, string right)
    {
        for(int i = 0; i < board.Count; i++)
        {
            if(((GameObject)board[i]).GetComponent<CellLevelCreator>().cellType == -1)
            {
                dice.setReferenceDice(((GameObject)board[i]).transform.position.x, ((GameObject)board[i]).transform.position.z);
            }
        }
        dice.setFaces(int.Parse(left), int.Parse(up), int.Parse(right));
        diceBeginValues = new Vector3(int.Parse(left), int.Parse(up), int.Parse(right));
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyUp(KeyCode.X))
        //    cells.RotateAround(dice.referenceDice.transform.position, Vector3.up, (true ? 90f : -90f));
    }
}
