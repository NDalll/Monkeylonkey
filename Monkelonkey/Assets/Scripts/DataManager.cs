using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class DataManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static DataManager instance;

    public float bestTime;
    public int highScore;
    public FirebaseUser User;
    public DatabaseReference DBreference;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(this);
        }
    }

    public void CheckScores(float time, int score)
    {
        if(time > bestTime)
        {
            bestTime = time;
        }
        if(score > highScore)
        {
            highScore = score;
        }
    }
}
