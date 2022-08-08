using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpManager : MonoBehaviour
{
    [SerializeField]
    private Text playerNames, playerValues;
    [SerializeField]
    private string URL;

    [SerializeField] private List<ScoreData> ScoreData;

    private List<ScoreData> UpDownSD = new List<ScoreData>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }

    IEnumerator GetScores()
    {
        string url = URL + "/leaders";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);

            foreach (ScoreData score in resData.scores)
            {
                Debug.Log(score.userId +" | "+score.value);
                UpDownSD.Add(score);
            }

            UpDownSD = UpDownSD.OrderByDescending(x => x.value).ToList();
            playerNames.text = UpDownSD[0].name + "\n" +
                               UpDownSD[1].name + "\n" +
                               UpDownSD[2].name + "\n" +
                               UpDownSD[3].name + "\n" +
                               UpDownSD[4].name + "\n";
            
            playerValues.text = UpDownSD[0].value + "\n" +
                               UpDownSD[1].value + "\n" +
                               UpDownSD[2].value + "\n" +
                               UpDownSD[3].value + "\n" +
                               UpDownSD[4].value + "\n";


        }
        else
        {
            Debug.Log(www.error);
        }
    }


}


[System.Serializable]
public class ScoreData
{
    public int userId;
    public int value;
    public string name;

}

[System.Serializable]
public class Scores
{
    public ScoreData[] scores;
}

