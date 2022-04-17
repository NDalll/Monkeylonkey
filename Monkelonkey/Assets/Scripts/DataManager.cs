using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class DataManager : MonoBehaviour //denne klasse bliver brugt til at komunikere med databasen fra alle scener, og den bliver ikke ødlagt ved et nyt sceneload som alle andere gameobjekter
{
    // Start is called before the first frame update
    public static DataManager instance; //opretter et static instance af databasemanageren(Den er static da det betyder at alle værdigerne i statisk variable er gobale for alle klasser af klasse typen, og derfor ikke behøves en specik objekt refference)

    public float? bestTime;//obs! bestTime er blev sat til at være en nullable float, da jeg bruger null som tiden for at der ikke er blevet gennemført nogle runs på brugeren
    [System.NonSerialized] public int highScore;
    public FirebaseUser User;
    public DatabaseReference DBreference;
    private void Awake()
    {
        DontDestroyOnLoad(this);//gøre klassen til dontdestory on load
        if (instance == null)// hvis instance ikke er blevet defineret endu så sætte der instance lig dem dette specifike objekt, ellers så sletter den bare dette object(Sikkre bare at der ikke ved en fejl kan blive oprettet instances af Datamanger) 
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(this);
        }
    }

    public void CheckScores(float? time, int score)//tjekker scoresne, der bliver passed med funktionen og updatere den enkelste socre hvis den er bedre 
    {
        if(bestTime != null && time < bestTime)//Hvis han allerde har gennemførst et run før og det her var hurtigere
        {
            bestTime = time;//opdatere besttime 
            StartCoroutine(UpdateBestTime(time));//opdatere tiden i databasen
        }else if(bestTime == null)//hvis han aldrig har gennemført et run før nu er det nok til at databasen skal opdateteres selv den nuværende tid også bare er null
        {
            bestTime = time;
            StartCoroutine(UpdateBestTime(time));
        }
        if(score >= highScore)// hvis runnet har en bedre 
        {
            highScore = score;
            StartCoroutine(UpdateHighscore(score));
        }
    }
    public IEnumerator UpdateHighscore(int _highscore)
    {
        //Sætter highscoren på den nyværnde bruger
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("highScore").SetValueAsync(_highscore);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)//fejltjek
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Highscoren er nu opdateret
        }
    }

    public IEnumerator UpdateBestTime(float? _bestTime)
    {
        //tjekker om bestTime er null for så er runnet ikke gennemført, og derfor skal bestime bare være 0 i selve databasen
        if (_bestTime == null)
        {
            _bestTime = 0;
        }

        //Sætter bestTime på den nyværnde bruger
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("bestTime").SetValueAsync(_bestTime);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //bestime er nu opdateret
        }
    }
}
