using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialClear : MonoBehaviour
{
    private Gamecontroller gamecontroller;
    public bool isTutorial;
    void Start()
    {
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if (isTutorial)
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
                SceneManager.LoadScene("Gameover");
            } 
        }
        
    }
}
