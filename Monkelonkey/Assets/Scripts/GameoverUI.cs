using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using TMPro; //vigtigt da der benyttes TextMeshPro elementer

public class GameoverUI : MonoBehaviour //Dette script er ansvarlig for at vise dataen på gameover skærmen og også beregne scoren
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
    private int score;
    [System.NonSerialized]
    private bool win;
    
    // Start is called before the first frame update
    void Start()
    {
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>();//reference til gamecontrolleren
        win = gamecontroller.gameWon; //tjekker om spilleren vandt
        score = gamecontroller.bananasCollected * 51 + gamecontroller.bananas * 250 + gamecontroller.enemiesDefeated * 1242 + gamecontroller.floorsBeaten * 10000; //beregner scoren ud fra hvad spilleren gjorde
        scoreText.text = score.ToString(); //viser scoren
        if (win) //hvis spilleren vandt
        {
            header.text = "YOU WON!"; //fortæller at de vandt
            float minutes = Mathf.FloorToInt(gamecontroller.timePlayed / 60); //beregner mængden af minutter spilleren spillede
            float seconds = Mathf.FloorToInt(gamecontroller.timePlayed % 60); //beregner hvor mange sekunder der var til overs
            string displayTime = string.Format("{0:0}:{1:00}", minutes, seconds); //sætter formatet af timeren
            time.text = "Time: " + displayTime; //viser tiden
            DataManager.instance.CheckScores(gamecontroller.timePlayed, score); //gemmer scoren og tiden
        }
        else
        {
            string displayTime = "-- :--"; //sætter tiden som blank da de ikke klarede banen
            time.text = "Time: " + displayTime; //viser tiden
            DataManager.instance.CheckScores(null, score); //gemmer tiden som null og scoren
        }
        
        floorText.text = gamecontroller.floorsBeaten.ToString(); //viser mængden af baner klaret
        enemyDefText.text = gamecontroller.enemiesDefeated.ToString(); //viser mængden af fjender besejret
        bananaColText.text = gamecontroller.bananasCollected.ToString(); //viser mængden af bananer samlet i alt
        unusedBanText.text = gamecontroller.bananas.ToString(); //viser mængden af bananer samlet
    }
}
