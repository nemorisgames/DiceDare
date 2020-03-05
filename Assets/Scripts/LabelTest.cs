using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelTest : MonoBehaviour
{
    public Transform dice;
    public GameObject labelGO;
    public Transform panel;

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            Transform t = ((GameObject)Instantiate(labelGO,dice.transform.position,labelGO.transform.rotation,panel)).GetComponent<Transform>();
            t.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x,Camera.main.transform.eulerAngles.y,0);
        }
    }
}
