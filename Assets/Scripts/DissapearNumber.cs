using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissapearNumber : MonoBehaviour
{
    Cell cell;
    public float timeShowing = 1f;
    public TweenScale tweenScale;
    float currentTimeShowing;
    bool showing = true;
    int correctNumber;
    // Start is called before the first frame update
    void Start()
    {
        cell = GetComponent<Cell>();
        correctNumber = cell.number;

        currentTimeShowing = Time.time + timeShowing;
    }

    public void Show()
    {
        showing = true;
        currentTimeShowing = Time.time + timeShowing;
        cell.SetText("" + correctNumber);
        tweenScale.ResetToBeginning();
        tweenScale.PlayForward();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTimeShowing < Time.time && showing)
        {
            cell.SetText("");
            showing = false;
        }
    }
}
