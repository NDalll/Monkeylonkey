using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{

    public TMP_Text usernameText;
    public TMP_Text timeText;
    public TMP_Text highscoreText;
    public TMP_Text rankText;

    public void NewScoreElement (string _username, float _time, int _score, int _rank)//funktion til at sætte indholdet af score elementet ud fra se passed værdiger
    {
        usernameText.text = _username;
        if(_time != 0)//hvis at tiden ikke er nul
        {
            float minutes = Mathf.FloorToInt(_time / 60);//udrenger hvor mange minutter der er blevet brugt
            float seconds = Mathf.FloorToInt(_time % 60);//udregner for mange sekunder der er blevet brugt
            string displayTime = string.Format("{0:0}:{1:00}", minutes, seconds);//laver en formateret string så den hvis den i en digital ur stil
            timeText.text = displayTime;
        }
        else
        {
            //hvis at tiden at 0 skal den bare hvis det som om der igen tid er
            string displayTime = "--:--";
            timeText.text = displayTime;
        }
        
        highscoreText.text = _score.ToString();
        rankText.text = _rank.ToString();
    }

}
