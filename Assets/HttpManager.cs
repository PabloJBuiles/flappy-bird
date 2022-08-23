using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HttpManager : MonoBehaviour
{
    [SerializeField]
    private Text playerNames, playerValues;
    [SerializeField]
    private string URL;

    [SerializeField] private List<ScoreData> ScoreData;
    

    [SerializeField]  UnityEvent _eventLogIn;
    [SerializeField] UnityEvent _eventSignIn;
    [SerializeField] UnityEvent _eventLogOut;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        Token = PlayerPrefs.GetString("token");
        UserName = PlayerPrefs.GetString("username");
        HighScore = PlayerPrefs.GetInt("highScore");
        Debug.Log("TOKEN: " + Token);
        if (Token != null)
        {
            StartCoroutine(GetPerfil());
        }
    }

    public string Token;
    public string UserName;
    public int HighScore;


    public void LogOut()
    {
        PlayerPrefs.SetString("token", null);
        PlayerPrefs.SetString("username", null);
        Token = PlayerPrefs.GetString("token");
        UserName = PlayerPrefs.GetString("username");
        
        Time.timeScale = 0;

        
        

    }
    public void ClickGetScores()
    {
       StartCoroutine(GetScores());
    }    
    public void ClickSingUp()
    {
        var postData = GetInputData();
        StartCoroutine(SignUp(postData));
        
        //lo de arriba es un casteo
    }

    private string GetInputData()
    {
        string userName = GameObject.Find("InputFieldUser").GetComponent<InputField>().text;
        string password = GameObject.Find("InputFieldPassword").GetComponent<InputField>().text;
        AuthData data = new AuthData();
        data.username = userName;
        data.password = password;
        string postData = JsonUtility.ToJson(data);
        return postData;
    }

    public void ClickLogIn()
    {
        var postData = GetInputData();
        StartCoroutine(LogIn(postData));
        
        //lo de arriba es un casteo
    }

    IEnumerator GetScores()
    {
        string url = URL + "/api/usuarios?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("content-type","application/json");
        www.SetRequestHeader("x-token",Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            Debug.Log(www.downloadHandler.text);
            //Root resData = JsonUtility.FromJson<Root>(www.downloadHandler.text);
            string myJsonResponse = www.downloadHandler.text;
            Root resData = JsonConvert.DeserializeObject<Root>(myJsonResponse);
            foreach (Usuario score in resData.usuarios)
            {
                Debug.Log(score +" | "+score.score);
                playerNames.text += score.username + "\n";
                playerValues.text += score.score + "\n";
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    
    IEnumerator SignUp(string pd)
    {
        Debug.Log(pd);
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, pd);
        www.method = "POST";
        //el metodo get no lleva body, pero el post lleva body asi sea vacio, toca espesificarle que es un json y no un string
        www.SetRequestHeader("content-type","application/json");
        //post por defectos cambia el encabezado de contetn type
        
        

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("Registrado " + resData.usuario.username);
            StartCoroutine(LogIn(pd));
            _eventSignIn.Invoke();
    
    //despues se loguea, se recibe el token en una nueva corutina, el token estaria en resdata.token, llega en la peticion de log in
    


        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);

        }
    }

    public void SetNewHihgScore()
    {
        Debug.LogWarning("Seteando nuevo higscore");
        Debug.LogWarning(PlayerPrefs.GetInt("highScore") +"     aaa    d" + Score.score);
        if (PlayerPrefs.GetInt("highScore") < Score.score)
        {
            ScoreData data = new ScoreData();
            data.username = UserName;
            data.score = Score.score;
            PlayerPrefs.SetInt("highScore", Score.score);
            string postData = JsonUtility.ToJson(data);
            StartCoroutine(PatchScore(postData));
            Debug.LogWarning("Puntuacion maxima superada");

        }
    }
    IEnumerator PatchScore(string pd)
    {
        Debug.Log("Patch");
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, pd);
        www.method = "PATCH";
        //el metodo get no lleva body, pero el post lleva body asi sea vacio, toca espesificarle que es un json y no un string
        www.SetRequestHeader("content-type","application/json");
        www.SetRequestHeader("x-token",Token);
        //post por defectos cambia el encabezado de contetn type
        
        

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("ScoreMax: " + resData.usuario.score);
            Debug.LogWarning("ScoreMax: " + resData.usuario.score);

            
            //despues se loguea, se recibe el token en una nueva corutina, el token estaria en resdata.token, llega en la peticion de log in
    


        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);            
            Debug.LogWarning("Algo no fue bien");


        }
    }
    
    IEnumerator GetPerfil()
    {
        string url = URL + "/api/usuarios/"+ UserName;
        UnityWebRequest www = UnityWebRequest.Get(url);
        //el metodo get no lleva body, pero el post lleva body asi sea vacio, toca espesificarle que es un json y no un string
        www.SetRequestHeader("x-token",Token);
        //post por defectos cambia el encabezado de contetn type
        
        

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("Token valido " + resData.usuario.username);

            IniciarJuego();
            _eventLogIn.Invoke();
            //despues se loguea, se recibe el token en una nueva corutina, el token estaria en resdata.token, llega en la peticion de log in
    


        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);

        }
    }
    IEnumerator LogIn(string pd)
    {
        Debug.Log("Log in " + pd);
        string url = URL + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url, pd);
        www.method = "POST";
        //el metodo get no lleva body, pero el post lleva body asi sea vacio, toca espesificarle que es un json y no un string
        www.SetRequestHeader("content-type","application/json");
        //post por defectos cambia el encabezado de contetn type
        
        

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("Autenticado  " + resData.usuario.username + ", ID: " + resData.usuario._id);
            Debug.Log("TOKEN  " + resData.token);
            PlayerPrefs.SetString("token", resData.token);
            PlayerPrefs.SetString("username", resData.usuario.username);
            
            
            //despues se loguea, se recibe el token en una nueva corutina, el token estaria en resdata.token, llega en la peticion de log in
            IniciarJuego();
            _eventLogIn.Invoke();


        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);

        }
    }

    public void IniciarJuego()
    {
        Time.timeScale = 1;
    }
    

}



[System.Serializable]
public class Root
{
    public List<Usuario> usuarios { get; set; }
}
[System.Serializable]
public class Usuario
{
    public string _id { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public bool estado { get; set; }
    public int score { get; set; }
}


[System.Serializable]
public class ScoreData
{
   // public int userId;
    [FormerlySerializedAs("name")] public string username;
    [FormerlySerializedAs("value")] public int score;


}

[System.Serializable]
public class Scores
{
    public ScoreData[] scores;
}


[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public UserData usuario;
    public string token;
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool estado;
    public int score;
}