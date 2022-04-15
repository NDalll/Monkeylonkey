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

    public void NewScoreElement (string _username, int _kills, int _deaths, int _xp)
    {
        usernameText.text = _username;
        timeText.text = _kills.ToString();
        highscoreText.text = _deaths.ToString();
        rankText.text = _xp.ToString();
    }

}
