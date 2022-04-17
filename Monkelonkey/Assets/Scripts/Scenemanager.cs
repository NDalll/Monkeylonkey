using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour//scriptet p� knapperne der tillader dem at skifte scene
{
    private Gamecontroller gamecontroller;
    void Start() //kaldes p� f�rste frame
    {
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>();//reference til gamecontrolleren
    }
    public void StartGame()//starter spillet hvis man er logget ind
    {
        if(DataManager.instance.User != null)
        {
            gamecontroller.timePlayed = 0; //sikre at variablerne er genstartede
            gamecontroller.enemiesDefeated = 0; //sikre at variablerne er genstartede
            gamecontroller.bananas = 0; //sikre at variablerne er genstartede
            gamecontroller.bananasCollected = 0; //sikre at variablerne er genstartede
            gamecontroller.floorsBeaten = 0; //sikre at variablerne er genstartede
            SceneManager.LoadScene("Gameplay"); //skifter scenen
        }
        else
        {
            GameObject.FindGameObjectWithTag("FirebaseManager").GetComponent<FirebaseManager>().LoginScreen(); //viser log in sk�rmen
        }
    } 

    public void LoadTestStage() //tager til tutorialen
    {
        if (DataManager.instance.User != null)
        {
            gamecontroller.timePlayed = 0;
            gamecontroller.enemiesDefeated = 0;
            gamecontroller.bananas = 0;
            gamecontroller.bananasCollected = 0;
            gamecontroller.floorsBeaten = 0;
            SceneManager.LoadScene("Main");
        }
        else
        {
            GameObject.FindGameObjectWithTag("FirebaseManager").GetComponent<FirebaseManager>().LoginScreen();
        }
    }
    public void RestartGame() //starter spillet forfra
    {
            gamecontroller.timePlayed = 0;
            gamecontroller.enemiesDefeated = 0;
            gamecontroller.bananas = 0;
            gamecontroller.bananasCollected = 0;
            gamecontroller.floorsBeaten = 0;
    }
    public void ToHomeScreen() //g�r til startsk�rmen
    {
        SceneManager.LoadScene("Start");
    }
    public void ToGameOverScene() //g�r til gameover sk�rmen
    {
        SceneManager.LoadScene("Gameover");
    }
}
