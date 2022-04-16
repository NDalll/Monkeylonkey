using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
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
    void Start()
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
        bananaScoreUI.text = bananaScore.ToString();
        grappleScoreUI.text = grappleScore.ToString();
    }

    public void SendVariables()
    {

    }

    
}
