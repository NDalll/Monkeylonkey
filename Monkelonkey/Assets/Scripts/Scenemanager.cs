using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour//scriptet på knapperne der tillader dem at skifte scene
{
    private Gamecontroller gamecontroller;
    void Start() //kaldes på første frame
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
            GameObject.FindGameObjectWithTag("FirebaseManager").GetComponent<FirebaseManager>().LoginScreen(); //viser log in skærmen
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
    public void ToHomeScreen() //går til startskærmen
    {
        SceneManager.LoadScene("Start");
    }
    public void ToGameOverScene() //går til gameover skærmen
    {
        SceneManager.LoadScene("Gameover");
    }
}
