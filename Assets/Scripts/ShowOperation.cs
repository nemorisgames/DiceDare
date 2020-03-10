using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOperation : MonoBehaviour
{
    public static ShowOperation Instance;
    public GameObject opLabelGO;
    private Dice dice;
    private List<OperationText> operationTexts = new List<OperationText>();

    void Awake(){
        if(Instance != null)
            Destroy(Instance);
        Instance = this;
    }

    void Start()
    {
        dice = GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>();
    }

    public void RemoveOperationText(OperationText ot){
        operationTexts.Remove(ot);
        Destroy(ot.gameObject);
    }

    public void ShowOp(bool b, int num1, int num2, int res, int resto){
        if(Mathf.Abs(num1) == 999 && Mathf.Abs(num2) == 999)
            return;
        OperationText t = ((GameObject)Instantiate(opLabelGO,dice.transform.position + Vector3.up * (resto == 0 ? 1f : 1.2f),opLabelGO.transform.rotation,transform)).GetComponent<OperationText>();
        t.transform.eulerAngles = new Vector3(/*Camera.main.transform.eulerAngles.x/2f*/ 0,Camera.main.transform.eulerAngles.y/2f,0);
        t.Init(b,num1,num2,res,dice.CurrentOpString(),resto);
        foreach(OperationText ot in operationTexts){
            ot.DimOperation(0.7f);
        }
        operationTexts.Add(t);
    }
}
