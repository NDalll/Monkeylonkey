using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour
{
    private Gamecontroller gamecontroller;
    void Start()
    {
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>();
    }
    public void StartGame()
    {
        if(DataManager.instance.User != null)
        {
            gamecontroller.timePlayed = 0;
            gamecontroller.enemiesDefeated = 0;
            gamecontroller.bananas = 0;
            gamecontroller.bananasCollected = 0;
            gamecontroller.floorsBeaten = 0;
            SceneManager.LoadScene("Gameplay");
        }
        else
        {
            GameObject.FindGameObjectWithTag("FirebaseManager").GetComponent<FirebaseManager>().LoginScreen(); 
        }
        
    } 
    public void LoadTestStage()
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
    public void RestartGame()
    {
            gamecontroller.timePlayed = 0;
            gamecontroller.enemiesDefeated = 0;
            gamecontroller.bananas = 0;
            gamecontroller.bananasCollected = 0;
            gamecontroller.floorsBeaten = 0;
    }
    public void ToHomeScreen()
    {
        SceneManager.LoadScene("Start");
    }
    public void ToGameOverScene()
    {
        SceneManager.LoadScene("Gameover");
    }
}
