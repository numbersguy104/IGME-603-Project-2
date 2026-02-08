using UnityEngine;

public class DistractionManager : MonoBehaviour
{
    private bool gameStarted = false;
    private float time = 0.0f;
    private Vector2 screenSize;

    [Header("Popup")]
    [Tooltip("How often a popup will appear - every X seconds")]
    [SerializeField] private float popupRate = 1.0f;
    [Tooltip("Minimum popup duration, in seconds")]
    [SerializeField] private float popupDurationMinimum = 3.0f;
    [Tooltip("Maximum popup duration, in seconds")]
    [SerializeField] private float popupDurationMaximum = 5.0f;

    private float nextPopupTime;

    [Header("Prefabs")]
    [SerializeField] private GameObject popupPrefab;

    private void Start()
    {
        screenSize = GetComponent<RectTransform>().rect.size;
        nextPopupTime = popupRate;
        FindAnyObjectByType<FocusCircle>().GameStart.AddListener(StartGame);
    }

    private void Update()
    {
        if (!gameStarted) return;

        time += Time.deltaTime;

        if (time > nextPopupTime)
        {
            nextPopupTime += popupRate;

            GameObject popup = Instantiate(popupPrefab, transform);
            popup.GetComponent<Popup>().SetDuration(Random.Range(popupDurationMinimum, popupDurationMaximum));

            Vector2 popupSize = popup.GetComponent<RectTransform>().rect.size;
            Vector2 position = RandomScreenPosition(popupSize / 2);
            popup.transform.position = position;
        }
    }

    private void StartGame()
    {
        gameStarted = true;
    }

    private Vector2 RandomScreenPosition(float bufferX = 0, float bufferY = 0)
    {
        float x = Random.value * (screenSize.x - bufferX * 2) + bufferX;
        float y = Random.value * (screenSize.y - bufferY * 2) + bufferY;
        return new Vector2(x, y);
    }

    private Vector2 RandomScreenPosition(Vector2 buffer = default)
    {
        return RandomScreenPosition(buffer.x, buffer.y);
    }
}
