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

    public void NewScoreElement (string _username, float _time, int _score, int _rank)
    {
        usernameText.text = _username;
        float minutes = Mathf.FloorToInt(_time / 60);
        float seconds = Mathf.FloorToInt(_time % 60);
        string displayTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        timeText.text = displayTime;
        highscoreText.text = _score.ToString();
        rankText.text = _rank.ToString();
    }

}
