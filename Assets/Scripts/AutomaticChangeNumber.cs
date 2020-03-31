using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticChangeNumber : MonoBehaviour
{
    private static List<AutomaticChangeNumber> cells = new List<AutomaticChangeNumber>();
    Cell cell;
    public float timeChange = 1.5f;
    float currentTimeChange;
    int correctNumber;
    public int numberOfNumbers = 4;
    int[] numbers;
    int currentIndex = 0;
    private List<int> options = new List<int>();
    private TweenScale tweenScale;
    public float scaleTime = 0.5f;
    public AudioClip audioClip;
    
    void Awake(){
        tweenScale = GetComponentInChildren<TweenScale>();
        cells.Add(this);
    }

    void Start()
    {
        for(int i = 1; i < 11; i++){
            options.Add(i);
            options.Add(-i);
        }
        cell = GetComponent<Cell>();
        correctNumber = cell.number;
        numbers = new int[numberOfNumbers + 1];
        numbers[0] = correctNumber;
        for (int i = 1; i < numbers.Length; i++)
        {
            int index = Random.Range(0,options.Count);
            numbers[i] = correctNumber + options[index];
            options.RemoveAt(index);
        }

        currentTimeChange = Time.time + timeChange;
        tweenScale.duration = scaleTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTimeChange < Time.time)
        {
            currentIndex++;
            if (currentIndex >= numbers.Length) currentIndex = 0;
            cell.SetNumber(numbers[currentIndex]);
            currentTimeChange = Time.time + timeChange;
            tweenScale.PlayForward();
            StartCoroutine(reduceScale());
            if(cells[0] == this)
                InGame.Instance.PlayFX(audioClip,0.7f);
        }
    }

    IEnumerator reduceScale(){
        yield return new WaitForSeconds(tweenScale.duration);
        tweenScale.PlayReverse();
    }

    void OnDestroy(){
        cells.Remove(this);
    }
}
