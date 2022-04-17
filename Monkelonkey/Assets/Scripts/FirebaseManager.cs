using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System;
public class FirebaseManager : MonoBehaviour
{
    //Firebase variabler
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;
    public DatabaseReference DBreference;
    
    //Login Variabler
    [Header("Login")]
    public GameObject loginScreen;
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variabler
    [Header("Register")]
    public GameObject registerScreen;
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //Brugerdata Varibler
    [Header("UserData")]
    public TMP_Text usernameField;
    public GameObject scoreScreen;
    public GameObject scoreElement;
    public Transform scoreboardContent;
    private string currentOrderItem;

    public void RegisterScreen()//skifter til register UI
    {
        loginScreen.SetActive(false);
        registerScreen.SetActive(true);
        scoreScreen.SetActive(false);
    }
    public void LoginScreen()//skifter til login UI
    {
        loginScreen.SetActive(true);
        registerScreen.SetActive(false);
        scoreScreen.SetActive(false);
    }
    public void ScoreBoardScreen()//skifter til register UI
    {
        scoreScreen.SetActive(true);
    }
    public void CloseButton(GameObject window)//lukker specificiferet UI
    {
        window.SetActive(false);
    }
    public void ClearLoginFeilds()//rydder felterne for login UIen
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
        
    }
    public void ClearRegisterFeilds()//rydder felterene for register UIen
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }
    void Awake()//Køre når objectet bliver instancieret
    {
        //Tjekker systemet for at alle de nødvendige resourser er til stedet for at køre firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;//gemmer resultatet
            if (dependencyStatus == DependencyStatus.Available)// hvis at alt er godt
            {
                InitializeFirebase();//start Firebase
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);//Print fejl besked plus status
            }
        });
    }
    private void Start()//køre lige før det første frame update
    {
        //Tjekker om DataManangeren har en defintion på en bruger(det vil sige at vi allerede er logget ind)
        if(DataManager.instance.User != null)
        {
            //Hvis vi er logget ind så bruger vi henter vi brugeren for den bruger vi er logget ind med, og hvis login navnet
            User = DataManager.instance.User;
            usernameField.text = User.DisplayName;
        }
        else
        {
            //hvis ikke vi er logget ind så gemmer den database refferencen i DataManagerne(Dette gør så vi kan bruge databasen fra vores datamanger)
            LoginScreen();//derud over viser vi skærmen til at logge ind
        }
        DataManager.instance.DBreference = DBreference;//Efter som database referencen bliver er den samme lige meget hvilken bruger der er logget ind sætter vi den til det samme ved hver start
        currentOrderItem = "highScore";//sætter standard soteringsværdig at Scoreboard tabel
    }

    private void InitializeFirebase()//
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;//Sætter hvor authenticator(Den del af firebase er styre alt bruger realteret)
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;//Sætter vores databaserefference(Den del af firebase der styre at iforhold til lagering af data)
    }

    //Function for the login knappen
    public void LoginButton()
    {
        //Starter coroutinen Login, hvor den passer email og password med
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Starter coroutinen Register, hvor den passer email, username og password med
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    //Function for the logud knappen
    public void SignOutButton()
    {
        auth.SignOut();//kalder firebases logud funktion fra authenticatoren
        LoginScreen();//viser login skræm
        DataManager.instance.User = null;//sletter Useren fra vores DataManager
        ClearRegisterFeilds();
        ClearLoginFeilds();
        usernameField.text = "Not Logged in";
    }
    //Funktion for Scoreboard knappen
    public void ScoreboardButton()
    {
        //Starter coroutinen for at load alt scoreboard data, og sender navnet den skal sotere scorboard iforhold til med
        StartCoroutine(LoadScoreboardData(currentOrderItem));
    }

    //Ienumeratoren for vores Login Coroutine
    private IEnumerator Login(string _email, string _password)
    {
        //Kalder firebase authenticatorens login funktion, vores vi passer email og password med, og gemmer den Tasken under LoginTask
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        //Venter til at vores task er færdig
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        //Hvis der er nogle exeptions(fejl), henter vi der konkrete fejl og printer dem 
        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}"); //printer fejlen 

            //Henter den kontrete fejlkode
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";//sætter standard fejl medelelse
            switch (errorCode)//switch statement der ser hvis det er fordi felterene ikke er udfyldt korrekt, og hvis de ikke er erstatter den besked med den fejl den er fundet
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;//sætter fejlteksten i UIen at være den funende fejl
        }
        else
        {
            //Nu er brugeren succesfuldt logget ind
            //Henter resultatet fra tasken, og gemmen det under User 
            User = LoginTask.Result;
            DataManager.instance.User = User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);//printer brugeren der logget ind i debugkonsollen
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            StartCoroutine(LoadUserData());//loader bruger data
            

            yield return new WaitForSeconds(2); //venter 2 sekunder, så brugeren kan nå at se confirmationtext efter de har logget ind

            usernameField.text = User.DisplayName;//sætter bruger navnet
            CloseButton(loginScreen);
            confirmLoginText.text = "";
            ClearLoginFeilds();
            ClearRegisterFeilds();
        }
    }
    //Ienumeratoren for vores Register Coroutine
    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")//tjekker om der er indtastet bruger navn
        {
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)//tjekker om kodeordene matcher
        {
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Kalder firebase authenticatorens register funktion, hvor vi passer email og password med, og gemmer den Tasken under registerTask
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            //Venter på tasken færdiggøre
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)//fejbehandling bare for regitreing 
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //Der er nu lavet en bruger, og gemmen unde User
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Laver en bruger profil med displayname der er vores username(dette er en klasse der er inkluderet i firebase)
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Kalder Authenticatorens funktion for opdatering af bruger profil 
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);// Venter på at Tasken er færdig

                    if (ProfileTask.Exception != null)
                    {
                        //Hvis der er fejl, så print dem
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Brugernavnet er nu sat
                        DataManager.instance.User = User;//opdatere useren i Datamangeren
                        DataManager.instance.DBreference = DBreference;//Sikre der er DBreferencen er sat i Datamangeren
                        StartCoroutine(SetUsernameDatabase(User.DisplayName));//sæt brugernavnet i databasen også
                        StartCoroutine(DataManager.instance.UpdateBestTime(0));//sæt 0 som bedste tid(læg mærke til at UpdateBestTime routinen, lægger i DataManageren)
                        StartCoroutine(DataManager.instance.UpdateHighscore(0));//søt 0 som highscored(læg mærke til at UpdateHighscore routinen, lægger i DataManageren)
                        warningRegisterText.text = "";
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                        CloseButton(registerScreen);
                        StartCoroutine(Login(_email, _password));//logger ind med den oprettet bruger
                    }
                }
            }
        }
    }

    private IEnumerator SetUsernameDatabase(string _username)
    {
        //Opretter en Task der sætter bruger navnet i databasen, læg mærketil at der firebase allerede har lavet et bruger ID til vores bruger, under User.UserId.
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);
        //venter på tasken er færdig
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            //printer evt. fejl
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Bruger navnet er du sat i databasen
        }
    }

   

    private IEnumerator LoadUserData()
    {
        //Henter bruger brugeren fra databasen der pt er logget ind ud af brugeridet
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            //printer evt. fejl
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data blev modtager 

            DataSnapshot snapshot = DBTask.Result;//Gemmer det modtaget data i et snapshot 

            
            DataManager.instance.highScore = int.Parse(snapshot.Child("highScore").Value.ToString());//Sætter highscoren i Datamangeren til Highscoren fra Databasen
            if(float.Parse(snapshot.Child("bestTime").Value.ToString()) == 0)//Her tjekker jeg om bestTime er 0 da det betyder at der ikke er færdig gjort et run endnu og bestTime derfor skal være null istedet
            {
                DataManager.instance.bestTime = null;
            }
            else
            {
                //Hvis der er en reel tid så set besttime til den
                DataManager.instance.bestTime = float.Parse(snapshot.Child("bestTime").Value.ToString());
            }
            
        }
    }
    //Funktion til at skifte sortings væriden
    public void SwitchSort()
    {
        //Hvis sorterings værdigen er den ene så sæt den til den anden
        if(currentOrderItem == "highScore")
        {
            currentOrderItem = "bestTime";
        }
        else
        {
            currentOrderItem = "highScore";
        }
        //Reload Scoreboardet med den nu soterings værdig
        StartCoroutine(LoadScoreboardData(currentOrderItem));
    }
    private IEnumerator LoadScoreboardData(string orderItem)
    {
        //Hetnter data fra alle brugere og sotere det efter soterings værdigen(lav til høj)
        var DBTask = DBreference.Child("users").OrderByChild(orderItem).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Dataen er blevet modtager
            DataSnapshot snapshot = DBTask.Result;//Gemmer dataen i et snapshot

            List<DataSnapshot> childData = snapshot.Children.ToList();//Gemmer alle brugerne som en liste
            if(orderItem == "bestTime")//Hvis listen skal soteres efter tid, ville den have alle dem der har tid som 0(ikke gennemført) først, vores de faktisk skal være allersidst, dette bliver rettet her.
            {
                List<DataSnapshot> bData = childData.Where(i => i.Child("bestTime").Value.ToString() == "0").ToList();//Starter med at finde at Brugerne de har en tid på 0 en en buffer data List(bData)
                childData.RemoveAll(i => i.Child("bestTime").Value.ToString() == "0");//Derefter fjerne den alle brugerne fra listen der har en tid på 0
                childData.AddRange(bData);//Så tilføjer den så buffer listen, til bruger listen igen, her dette har så gjort at den lægge til slut i listen
            }
            else
            {
                //Hvis det bare er score sotering skal den bare reverse listen, så den har går fra høj til lav istedet
                childData = childData.Reverse<DataSnapshot>().ToList();
            }
            
            //Fjern alle eksiterende scoreboard elementer
            foreach (Transform child in scoreboardContent.transform)//køre igennem alle childs af vores scoreboard content
            {
                Destroy(child.gameObject);//destoryer gameobjektet der er ved
            }
            int rank = 0;//Variable til at holde styr på rangen
            foreach (DataSnapshot childSnapshot in childData) //køre igennm alle brugerne i listen
            {
                //henter brugernavn, besttime og highscore fra brugeren
                string username = childSnapshot.Child("username").Value.ToString();
                float bestTime = float.Parse(childSnapshot.Child("bestTime").Value.ToString());
                int highScore = int.Parse(childSnapshot.Child("highScore").Value.ToString());
                rank++;//pluser rangen med 1

                //Instantiere et scoreElement som child at scoreboardContent 
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                //Sætter vædigerne i scoreboard elementet
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, bestTime, highScore, rank);
            }

            //Viser Scoreboard skræmen
            ScoreBoardScreen();
        }
    }
}