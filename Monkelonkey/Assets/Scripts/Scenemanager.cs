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
        gamecontroller.timePlayed = 0;
        gamecontroller.enemiesDefeated = 0;
        gamecontroller.bananas = 0;
        gamecontroller.bananasCollected = 0;
        gamecontroller.floorsBeaten = 0;
        SceneManager.LoadScene("Gameplay");
    } 
    public void LoadTestStage()
    {
        gamecontroller.timePlayed = 0;
        gamecontroller.enemiesDefeated = 0;
        gamecontroller.bananas = 0;
        gamecontroller.bananasCollected = 0;
        gamecontroller.floorsBeaten = 0;
        SceneManager.LoadScene("Main");
    } 
    public void ToGameOverScene()
    {
        SceneManager.LoadScene("Gameover");
    }
}
