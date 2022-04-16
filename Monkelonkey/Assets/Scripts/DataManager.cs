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
        if(bestTime != 0 && time <= bestTime && time != 0)
        {
            bestTime = time;
            StartCoroutine(UpdateBestTime(time));
        }
        if(score >= highScore)
        {
            highScore = score;
            StartCoroutine(UpdateHighscore(score));
        }
    }
    public IEnumerator UpdateHighscore(int _highscore)
    {
        //Set the currently logged in user xp
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("highScore").SetValueAsync(_highscore);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Xp is now updated
        }
    }

    public IEnumerator UpdateBestTime(float _bestTime)
    {
        //Set the currently logged in user kills
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("bestTime").SetValueAsync(_bestTime);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Kills are now updated
        }
    }
}
