using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticChangeNumber : MonoBehaviour
{
    Cell cell;
    public float timeChange = 1.5f;
    float currentTimeChange;
    int correctNumber;
    public int numberOfNumbers = 4;
    int[] numbers;
    int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        cell = GetComponent<Cell>();
        correctNumber = cell.number;
        numbers = new int[numberOfNumbers + 1];
        numbers[0] = correctNumber;
        for (int i = 1; i < numbers.Length; i++)
        {
            numbers[i] = correctNumber + Random.Range(-10, 10);
        }

        currentTimeChange = Time.time + timeChange;
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
        }
    }
}
