using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//vigigt da sciptet skal kunne skifte scene

public class TutorialClear : MonoBehaviour //dette er scriptet der styre hvad der sker når spilleren hopper ud af banen
{
    private Gamecontroller gamecontroller;
    private Player player;
    public bool isTutorial;
    void Start()//kaldes på første frame
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();//reference til player
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>();//reference til gamecontrolleren
    }
    private void OnTriggerEnter2D(Collider2D collision)//kaldes på collision
    {

        if (collision.CompareTag("Player")) //hvis det er spilleren
        {
            if (isTutorial) //hvis det er tutorialen (defineret i inspektoren)
            {
                gamecontroller.timePlayed = 0;//sikre at variblerne bliver genstartet
                gamecontroller.enemiesDefeated = 0; //sikre at variblerne bliver genstartet
                gamecontroller.bananas = 0; //sikre at variblerne bliver genstartet
                gamecontroller.bananasCollected = 0; //sikre at variblerne bliver genstartet
                gamecontroller.floorsBeaten = 0; //sikre at variblerne bliver genstartet
                SceneManager.LoadScene("Gameplay"); //loader spil scenen
            }
            else
            {
                player.LoadGameOverScene(); //kalder gameover funktionen i playerscriptet
            } 
        }
        
    }
}
