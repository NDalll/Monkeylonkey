using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameoverUI : MonoBehaviour
{
    [Header("Generel")]
    public TextMeshProUGUI header;
    public TextMeshProUGUI time;
    public TextMeshProUGUI scoreText;
    [Header("Variables")]
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI enemyDefText;
    public TextMeshProUGUI bananaColText;
    public TextMeshProUGUI unusedBanText;
    private Gamecontroller gamecontroller;
    private float score;
    
    // Start is called before the first frame update
    void Start()
    {
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>();
        score = gamecontroller.bananasCollected * 55 + gamecontroller.bananas * 250 + gamecontroller.enemiesDefeated * 800 + gamecontroller.floorsBeaten * 10000;
        scoreText.text = score.ToString();
        float minutes = Mathf.FloorToInt(gamecontroller.timePlayed / 60);
        float seconds = Mathf.FloorToInt(gamecontroller.timePlayed % 60); 
        string displayTime = string.Format("{0:0}:{1:00}", minutes, seconds); 
        time.text = "Time: " + displayTime;
        string displayVaribles = gamecontroller.floorsBeaten.ToString();
        floorText.text = gamecontroller.floorsBeaten.ToString();
        enemyDefText.text = gamecontroller.enemiesDefeated.ToString();
        bananaColText.text = gamecontroller.bananasCollected.ToString();
        unusedBanText.text = gamecontroller.bananas.ToString();
    }
}
