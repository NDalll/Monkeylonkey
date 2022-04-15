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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
