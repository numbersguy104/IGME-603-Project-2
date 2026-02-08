using UnityEngine;

public class DistractionManager : MonoBehaviour
{
    private bool gameStarted = false;
    private float time = 0.0f;
    private Vector2 screenSize;
    private FocusCircle _circle;

    private float nextPopupTime;
    [Header("Popup")]
    [Tooltip("How often a popup will appear - every X seconds")]
    [SerializeField] private float popupRate = 1.0f;
    [Tooltip("Minimum popup duration, in seconds")]
    [SerializeField] private float popupDurationMinimum = 3.0f;
    [Tooltip("Minimum popup duration, in seconds")]
    [SerializeField] private float popupDurationMaximum = 5.0f;

    [Header("Prefabs")]
    [SerializeField] private GameObject popupPrefab;

    private void Start()
    {
        screenSize = GetComponent<RectTransform>().rect.size;

        _circle = FindAnyObjectByType<FocusCircle>();
        nextPopupTime = popupRate;
    }

    private void Update()
    {
        if (!gameStarted && _circle.gameStarted)
        {
            gameStarted = true;
        }

        if (gameStarted)
        {
            time += Time.deltaTime;

            if (time > nextPopupTime)
            {
                nextPopupTime += popupRate;

                GameObject popup = Instantiate(popupPrefab, transform);
                popup.GetComponent<Popup>().SetDuration(Random.Range(popupDurationMinimum, popupDurationMaximum));

                Vector2 position = RandomScreenPosition(50.0f, 50.0f);
                popup.transform.position = position;
            }
        }
    }

    private Vector2 RandomScreenPosition(float bufferX = 0, float bufferY = 0)
    {
        float x = Random.value * (screenSize.x - bufferX * 2) + bufferX;
        float y = Random.value * (screenSize.y - bufferY * 2) + bufferY;
        return new Vector2(x, y);
    }
}
