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
            DataManager.instance.DBreference = DBreference;
            LoginScreen();//derud over viser vi skærmen til at logge ind
        }
        currentOrderItem = "highScore";//sætter standard soteringsværdig at Scoreboard tabel
    }

    private void InitializeFirebase()//
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;//Sætter hvor authenticator(Den del af firebase er styre alt bruger realteret)
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;//Sætter vores databaserefference(Den del af firebase der styre at iforhold til lagering af data)
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    //Function for the sign out button
    public void SignOutButton()
    {
        auth.SignOut();
        LoginScreen();
        DataManager.instance.User = null;
        DataManager.instance.DBreference = null;
        ClearRegisterFeilds();
        ClearLoginFeilds();
        usernameField.text = "Not Logged in";
    }
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData(currentOrderItem));
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
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
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            StartCoroutine(LoadUserData());
            

            yield return new WaitForSeconds(2);

            usernameField.text = User.DisplayName;
            CloseButton(loginScreen); // Change to user data UI
            confirmLoginText.text = "";
            ClearLoginFeilds();
            ClearRegisterFeilds();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
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
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        DataManager.instance.User = User;
                        DataManager.instance.DBreference = DBreference;
                        StartCoroutine(UpdateUsernameDatabase(User.DisplayName));
                        StartCoroutine(DataManager.instance.UpdateBestTime(0));
                        StartCoroutine(DataManager.instance.UpdateHighscore(0));
                        warningRegisterText.text = "";
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                        CloseButton(registerScreen);
                        StartCoroutine(Login(_email, _password));
                    }
                }
            }
        }
    }


    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

   

    private IEnumerator LoadUserData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Child("highScore").Value == null && DBTask.Result.Child("bestTime").Value == null)
        {
            //No data exists yet
            DataManager.instance.User = User;
            DataManager.instance.DBreference = DBreference;
            DataManager.instance.highScore = 0;
            DataManager.instance.bestTime = null;
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            DataManager.instance.User = User;
            DataManager.instance.DBreference = DBreference;
            DataManager.instance.highScore = int.Parse(snapshot.Child("highScore").Value.ToString());
            if(float.Parse(snapshot.Child("bestTime").Value.ToString()) == 0)
            {
                DataManager.instance.bestTime = null;
            }
            else
            {
                DataManager.instance.bestTime = float.Parse(snapshot.Child("bestTime").Value.ToString());
            }
            
        }
    }
    public void SwitchSort()
    {
        if(currentOrderItem == "highScore")
        {
            currentOrderItem = "bestTime";
        }
        else
        {
            currentOrderItem = "highScore";
        }
        StartCoroutine(LoadScoreboardData(currentOrderItem));
    }
    private IEnumerator LoadScoreboardData(string orderItem)
    {
        //Get all the users data ordered by kills amount
        var DBTask = DBreference.Child("users").OrderByChild(orderItem).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            List<DataSnapshot> childData = snapshot.Children.ToList();
            if(orderItem == "bestTime")
            {
                List<DataSnapshot> bData = childData.Where(i => i.Child("bestTime").Value.ToString() == "0").ToList();
                childData.RemoveAll(i => i.Child("bestTime").Value.ToString() == "0");
                childData.AddRange(bData);
            }
            else
            {
                childData = childData.Reverse<DataSnapshot>().ToList();
            }
            
            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }
            int rank = 0;
            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in childData)
            {
                string username = childSnapshot.Child("username").Value.ToString();
                float bestTime = float.Parse(childSnapshot.Child("bestTime").Value.ToString());
                int highScore = int.Parse(childSnapshot.Child("highScore").Value.ToString());
                rank++;

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, bestTime, highScore, rank);
            }

            //Go to scoareboard screen
            ScoreBoardScreen();
        }
    }
}