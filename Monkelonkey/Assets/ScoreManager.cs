using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI BananaScoreUI;
    public static int bananaScore;
    void Start()
    {
        bananaScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        BananaScoreUI.text = bananaScore.ToString();
    }

    
}
