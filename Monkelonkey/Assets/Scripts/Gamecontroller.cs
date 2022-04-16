using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamecontroller : MonoBehaviour
{
    [System.NonSerialized] public float timePlayed;
    [System.NonSerialized] public int enemiesDefeated;
    [System.NonSerialized] public int bananasCollected;
    [System.NonSerialized] public int bananas;
    [System.NonSerialized] public int floorsBeaten;
    [System.NonSerialized] public bool gameWon;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
