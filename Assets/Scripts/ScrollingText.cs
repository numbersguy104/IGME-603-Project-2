using TMPro;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    private bool gameStarted = false;
    private bool pausedTop = true;
    private bool pausedBottom = false;
    private float pauseTimer;
    private float stopScrollingAt;

    [Tooltip("How fast the text scrolls down, in units per second")]
    [SerializeField] private float scrollSpeed = 50.0f;
    [Tooltip("How long the text will stop scrolling after reaching the top or bottom, in seconds")]
    [SerializeField] private float pauseTime = 1.0f;

    private RectTransform _rectTransform;
    private void Start()
    {
        pauseTimer = pauseTime;

        _rectTransform = GetComponent<RectTransform>();

        float height = GetComponent<TextMeshProUGUI>().preferredHeight;
        float canvasHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        stopScrollingAt = height - canvasHeight;

        FindAnyObjectByType<FocusCircle>().GameStart.AddListener(StartGame);
    }

    void Update()
    {
        if (!gameStarted) return;

        if (pausedTop || pausedBottom)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                if (pausedBottom)
                {
                    Vector2 position = _rectTransform.anchoredPosition;
                    position.y = 0;
                    _rectTransform.anchoredPosition = position;

                    pausedTop = true;
                    pausedBottom = false;
                    pauseTimer = pauseTime;
                }
                else
                {
                    pausedTop = false;
                }
            }
        }
        else
        {
            Vector2 position = _rectTransform.anchoredPosition;

            position.y += scrollSpeed * Time.deltaTime;

            if (position.y >= stopScrollingAt)
            {
                position.y = stopScrollingAt;
                pausedBottom = true;
                pauseTimer = pauseTime;
            }

            _rectTransform.anchoredPosition = position;
        }
    }

    private void StartGame()
    {
        gameStarted = true;
    }
}
