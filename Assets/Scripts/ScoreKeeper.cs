using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreKeeper : MonoBehaviour
{
    private bool gameStarted = false;
    private bool gameEnded = false;
    private float elapsedTime = 0.0f;
    private int finalTime = 0;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        FocusCircle circle = FindAnyObjectByType<FocusCircle>();
        circle.GameStart.AddListener(StartTimer);
        circle.GameEnd.AddListener(StopTimer);
    }

    private void Update()
    {
        if (!gameStarted) return;
        if (gameEnded)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "GameOver")
            {
                GameObject.Find("End Text").GetComponent<TextMeshProUGUI>().text = "Game over!\n\nYour score: " + finalTime;
            }
            else if (currentScene != "MainGame")
            {
                Destroy(gameObject);
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }

    private void StartTimer()
    {
        gameStarted = true;
    }

    private void StopTimer()
    {
        gameEnded = true;
        finalTime = Mathf.RoundToInt(elapsedTime);
    }
}
