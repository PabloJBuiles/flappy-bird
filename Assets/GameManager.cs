using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
        HttpManager http = gameObject.GetComponent<HttpManager>();
        http.SetNewHihgScore();
    }
    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
}
