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
    // Use this for initialization
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        Scene = PlayerPrefs.GetString("SceneTest");
        SceneNumbers = PlayerPrefs.GetString("SceneNumbersTest");
        ScenePath = PlayerPrefs.GetString("ScenePathTest");
        
        if (Scene != "")
        {
            //print(Scene);
            //print(SceneNumbers);
            //print(ScenePath);
            //Se crea el board
            string[] aux = Scene.Split(new char[1] { '$' });
            string[] info = aux[0].Split(new char[1] { '|' });
            boardSize = new Vector2(int.Parse(info[1]), int.Parse(info[0]));
            createBoard();
            updateBoardLabel();

            composeBoard();
        }

        resetDice();
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
                CellLevelCreator c = g.GetComponent<CellLevelCreator>();
                /*if(j - beginCell.x == 0 && -i + beginCell.y == 0)
                {
                    c.cellType = -1;
                }*/
                board.Add(g);
            }
        }
        resetDice();
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
                    //completa los tipos de celdas
                    hit.transform.GetComponent<CellLevelCreator>().cellType = int.Parse(SceneAux[contador]);

                    //completa los numeros de las celdas
                    hit.transform.GetComponent<CellLevelCreator>().cellNumber = int.Parse(SceneNumbersAux[contador]);

                    //completa el path
                    hit.transform.GetComponent<CellLevelCreator>().pathNumber = getPathNumber(ScenePathAux, j, i);
                    contador++;
                }
            }
        }
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
        Scene = boardSize.y + "|" + boardSize.x;
        SceneNumbers = "1|1|1$";
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
                    SceneNumbers += " " + hit.transform.GetComponent<CellLevelCreator>().cellNumber + " |";
                    //completa el path
                    if (hit.transform.GetComponent<CellLevelCreator>().pathNumber != -1)
                        ScenePathAux[hit.transform.GetComponent<CellLevelCreator>().pathNumber - 1] = j + "," + i;
                }
            }
        }

        for(int i = 0; i < ScenePathAux.Length; i++)
        {
            ScenePath += ScenePathAux[i] + "|";
        }
        ScenePath = ScenePath.Substring(0, ScenePath.Length - 1);
        Scene += SceneAux;
        print(Scene);
        print(SceneNumbers);
        print(ScenePath);
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

    public void resetDice()
    {
        for(int i = 0; i < board.Count; i++)
        {
            if(((GameObject)board[i]).GetComponent<CellLevelCreator>().cellType == -1)
            {
                dice.setReferenceDice(((GameObject)board[i]).transform.position.x, ((GameObject)board[i]).transform.position.z);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
