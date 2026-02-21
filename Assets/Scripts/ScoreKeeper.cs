using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreKeeper : MonoBehaviour
{
    private bool gameStarted = false;
    private bool gameEnded = false;
    private int finalTime = 0;
    private float elapsedTime = 0.0f;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        QTE.completedQTEs = 0;
        QTE.totalQTETime = 0.0f;

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
            else if (currentScene != "MainGame"
                && currentScene != "EasierMainGame"
                && currentScene != "HarderMainGame")
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

        if (!File.Exists("Stats.txt"))
        {
            File.CreateText("Stats.txt");
        }
        File.AppendAllText("Stats.txt", "Game finished on scene " + SceneManager.GetActiveScene().name + "\n");
        File.AppendAllText("Stats.txt", "Score: " + finalTime + "\n");
        if (QTE.completedQTEs > 0)
        {
            float averageQTETime = QTE.totalQTETime / QTE.completedQTEs;
            File.AppendAllText("Stats.txt", "Average QTE completion time: " + averageQTETime + "\n\n");
        }
        else
        {
            File.AppendAllText("Stats.txt", "Average QTE completion time: N/A (no QTEs completed)\n\n");
        }
    }
}
