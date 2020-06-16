using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net.Http;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    #region Singleton
    private static DatabaseManager instance = null;
    public static DatabaseManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private void Start()
    {
        StartCoroutine(Database.GetHttp("ServerLogin.php?ID="+ Database.ServerID + "&Password=" + Database.ServerPassword));
    }


    public void ClientLogin(string _username, string _password)
    {
        string url = "UserLogin.php?Session_ID ="+ Database.SessionID + "&Username=" + _username + "&Password=" + _password;

        StartCoroutine(LoginRoutine(url));

    }

    //Wait for URL response as we are working with coroutines. 
    private IEnumerator LoginRoutine(string _url)
    {
        yield return StartCoroutine(Database.GetHttp(_url));

        if (Database.ServerResponse != "0" && Database.ServerResponse != "" && Database.ServerResponse != "null")
        {
            LoginData loginData = new LoginData();
            loginData = JsonUtility.FromJson<LoginData>(Database.ServerResponse);

            SceneManager.LoadScene("StartGame");
        }
    }

    public void ServerSubmitScore(int _score, int _user_id)
    {
        string url = "InsertQuery.php?Score=" + _score.ToString() + "&UserID=" + _user_id.ToString();

        StartCoroutine(SubmitRoutine(url));
        

    }

    private IEnumerator SubmitRoutine(string _url)
    {
        yield return StartCoroutine(Database.GetHttp(_url));
        Debug.Log(_url);
    }
}

public static class Database
{
    public static string Database_URL = "https://studenthome.hku.nl/~tim.peeters/Database/";

    [Header("Server Details")]
    public static int ServerID = 1;
    public static string ServerPassword = "DungeonDroolers";

    [Header("User Details")]
    public static string SessionID;
    public static int UserID = -1;
    public static string Username = "null";

    public static string ServerResponse;

    //Credits naar Nathan voor deze.
    public static IEnumerator GetHttp(string _url = "url")
    {
        var request = UnityWebRequest.Get(Database_URL + _url);
        {
            yield return request.SendWebRequest();

            if (request.isDone && !request.isHttpError)
            {
                ServerResponse = request.downloadHandler.text;
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}

[System.Serializable]
public class LoginData
{
    public int ID;
    public string Username;
}
