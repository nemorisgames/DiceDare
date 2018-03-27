using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : MonoBehaviour {
    string[] day;
    int daysPassedSinceBegin = 0;
    int daysPassedSinceLastPlay = 0;
    
    // Use this for initialization
    void Start () {
        //PlayerPrefs.DeleteAll();
        print(System.DateTime.Now.ToShortDateString().ToString());
        if (!PlayerPrefs.HasKey("calendar"))
        {
            string days = "";
            //uno menos para eliminar el | del final que suma uno extra
            for (int i = 1; i < 30; i++)
            {
                days += "|";
            }
            PlayerPrefs.SetString("calendar", days);
            PlayerPrefs.SetString("calendarBegin", System.DateTime.Now.ToShortDateString().ToString());
            PlayerPrefs.SetString("calendarLastPlay", System.DateTime.Now.ToShortDateString().ToString());
        }

        day = PlayerPrefs.GetString("calendar").Split(new char[] { '|' });
        daysPassedSinceBegin = (System.DateTime.Parse(System.DateTime.Now.ToShortDateString()) - System.DateTime.Parse(PlayerPrefs.GetString("calendarBegin"))).Days;
        daysPassedSinceLastPlay = (System.DateTime.Parse(System.DateTime.Now.ToShortDateString()) - System.DateTime.Parse(PlayerPrefs.GetString("calendarLastPlay"))).Days;

        //llena el arreglo inicial
        string[] aux = new string[30];
        int contador = 0;
        if (daysPassedSinceBegin > 29)
        {
            for (int i = daysPassedSinceLastPlay + 1; i <= 30; i++)
            {
                //print("adding " + (i - 1));
                aux[contador] = day[i - 1];
                contador++;
            }
            day = aux;
        }


        //print("days " + daysPassedSinceBegin);
        updateButtons();
        int beforeSaving = PlayerPrefs.GetInt("dailyScore");
        if (PlayerPrefs.HasKey("dailyScore"))
        {
            saveScore(PlayerPrefs.GetInt("dailyScore"));
            PlayerPrefs.DeleteKey("dailyScore");
        }
        print(beforeSaving + " , " + PlayerPrefs.GetInt("todayRecord", 0));
        int i2 = Mathf.Max(beforeSaving, PlayerPrefs.GetInt("todayRecord", 0));
        PlayerPrefs.SetInt("todayRecord", i2);
        print("days " + (System.DateTime.Parse(System.DateTime.Now.ToShortDateString()) - System.DateTime.Parse(PlayerPrefs.GetString("calendarLastPlay"))).Days);
        if ((System.DateTime.Parse(System.DateTime.Now.ToShortDateString()) - System.DateTime.Parse(PlayerPrefs.GetString("calendarLastPlay"))).Days > 0)
            PlayerPrefs.DeleteKey("todayRecord");
    }

    void updateButtons()
    {
        for (int i = 1; i <= 30; i++)
        {
            if (day[i - 1] == null) day[i - 1] = "";
            transform.Find("ButtonCalendario" + i + "/Highlight").gameObject.GetComponent<UITweener>().style = ((i - 1) == Mathf.Clamp(daysPassedSinceBegin, 0, 29)) ? UITweener.Style.Loop : UITweener.Style.Once;//gameObject.SetActive((i - 1) == Mathf.Clamp(daysPassedSinceBegin, 0, 29));
            //print(day[i - 1] + " " + i + " " + (day[i - 1].Length <= 0));
            transform.Find("ButtonCalendario" + i + "/Label").GetComponent<UILabel>().text = (day[i - 1].Length <= 0) ? "-" : day[i - 1] + "%";
            transform.Find("ButtonCalendario" + i).GetComponent<UIButton>().isEnabled = (day[i - 1].Length > 0);
        }
    }

    //mientras no se haga este metodo, no se guarda nada
    void saveScore(int score)
    {
        string scores = "";

        //condicion para que el puntaje no se reescriba
        //if(day[Mathf.Clamp(daysPassedSinceBegin, 0, 29)] == "")
        //condicion para poner el mejor puntaje
        if (day[Mathf.Clamp(daysPassedSinceBegin, 0, 29)] == null || day[Mathf.Clamp(daysPassedSinceBegin, 0, 29)] == "")
        {
            day[Mathf.Clamp(daysPassedSinceBegin, 0, 29)] = "" + score;
        }
        else
        {
            if (int.Parse(day[Mathf.Clamp(daysPassedSinceBegin, 0, 29)]) < score)
            {
                day[Mathf.Clamp(daysPassedSinceBegin, 0, 29)] = "" + score;
            }
        }
        for (int i = 1; i <= 30; i++)
        {
            scores += day[i - 1] + "|";
        }
        scores = scores.Substring(0, scores.Length - 1);
        updateButtons();
        PlayerPrefs.SetString("calendarLastPlay", System.DateTime.Now.ToShortDateString());
        PlayerPrefs.SetString("calendar", scores);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
