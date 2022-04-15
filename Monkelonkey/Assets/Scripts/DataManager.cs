using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static DataManager instance;

    public float bestTime;
    public int highScore;
    void Start()
    {
        if(instance == null)
        {
            instance = gameObject.AddComponent<DataManager>();
        }
    }

    public void CheckScores(float time, int Score)
    {
        if(time - bestTime <= 0)
        {
            bestTime = time;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
