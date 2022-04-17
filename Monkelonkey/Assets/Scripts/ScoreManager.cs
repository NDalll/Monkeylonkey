using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour//til forskel fra gamecontroller og datamanager s� er scoremangeren destroyet med scenereload, hvilet g�r at vi bruger den til at holde scoren p� det enkelte spil
{
    // Start is called before the first frame update
    public TextMeshProUGUI bananaScoreUI;
    public static int bananaScore;
    public TextMeshProUGUI grappleScoreUI;
    public static int grappleScore;
    public static int totalBananas;
    public static int enemiesDefeated;
    public static int stagesCleared;
    public int startingBananas;
    public int startingGrapples;
    void Start()//s�tter startv�diger for en score
    {
        bananaScore = startingBananas;
        grappleScore = startingGrapples;
        totalBananas = startingBananas;
        enemiesDefeated = 0;
        stagesCleared = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //opadtere uien c�rt frame
        bananaScoreUI.text = bananaScore.ToString();
        grappleScoreUI.text = grappleScore.ToString();
    }

    
}
