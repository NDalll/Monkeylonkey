using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamecontroller : MonoBehaviour //Dette er controlleren der ikke fjernes når spillet skifter scene og kan bruges til at sende variabler over til andre scener
{
    //Variablerne der kan sendes over:
    [System.NonSerialized] public float timePlayed;
    [System.NonSerialized] public int enemiesDefeated;
    [System.NonSerialized] public int bananasCollected;
    [System.NonSerialized] public int bananas;
    [System.NonSerialized] public int floorsBeaten;
    [System.NonSerialized] public bool gameWon;

    void Awake() //kaldes inden første frame
    {
        DontDestroyOnLoad(this.gameObject); //fortæller at den ikke skal fjernes når der loades scener
    }
}
