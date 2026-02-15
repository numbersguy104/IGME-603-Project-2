using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FocusCircle : MonoBehaviour
{
    public bool gameStarted = false;
    private float startingDiameter;
    private float canvasScale;
    private float shrinkTimer;
    //Yudong Added
    private float maxDiameter;

    private Vector2 destination;
    private Vector2 screenSize;
    private RectTransform _rectTransform;

    public UnityEvent GameStart;
    public UnityEvent GameEnd;

    [Tooltip("Speed of the circle's random movement, in units/second")]
    [SerializeField] private float speed = 100.0f;
    [Tooltip("How long the mouse can be off of the circle before it starts shrinking, in seconds")]
    [SerializeField] private float shrinkDelay = 0.5f;
    [Tooltip("Size the circle will lose per second (as a fraction of the total size) when the mouse is not in the circle")]
    [SerializeField] private float shrinkRate = 0.2f;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        startingDiameter = _rectTransform.rect.size.x;

        Transform canvas = transform.parent;

        canvasScale = canvas.GetComponent<Canvas>().scaleFactor;
        screenSize = canvas.GetComponent<RectTransform>().rect.size * canvasScale;
        //Yudong Added
        maxDiameter = startingDiameter;
    }
    private void Update()
    {
        Vector2 position = new Vector2(_rectTransform.position.x, _rectTransform.position.y);
        Vector2 mousePosition = Input.mousePosition;

        if (gameStarted)
        {
            if (Vector2.Distance(position, destination) < 0.01f)
            {
                NewDestination();
            }

            if (Vector2.Distance(position, mousePosition) > _rectTransform.rect.size.x / 2 * canvasScale)
            {
                if (shrinkTimer <= 0.0f)
                {
                    float newDiameter = _rectTransform.rect.size.x - (startingDiameter * shrinkRate * Time.deltaTime);
                    _rectTransform.sizeDelta = new Vector2(newDiameter, newDiameter);

                    if (_rectTransform.rect.size.x <= 0)
                    {
                        GameEnd.Invoke();
                        SceneManager.LoadScene("GameOver");
                    }
                }
                else
                {
                    shrinkTimer -= Time.deltaTime;
                }
            }
            else
            {
                shrinkTimer = shrinkDelay;
            }

            float tickSpeed = speed * Time.deltaTime * canvasScale;
            _rectTransform.position = Vector2.MoveTowards(_rectTransform.position, destination, tickSpeed);
        }
        else if (Vector2.Distance(position, mousePosition) <= _rectTransform.rect.size.x / 2)
        {
            gameStarted = true;
            GameStart.Invoke();
            NewDestination();
        }
    }

    private void NewDestination()
    {
        //Pick a random point on screen, but don't go off the edge of the screen
        float diameter = _rectTransform.rect.size.x;
        float x = Random.value * (screenSize.x - diameter) + diameter / 2;
        float y = Random.value * (screenSize.y - diameter) + diameter / 2;
        destination = new Vector2(x, y);
    }

    //Grows the circle by a fraction (from 0 to 1) of its initial size
    public void Grow(float increasePortion)
    {
        //Wont overgrowth
        float target = _rectTransform.rect.size.x + (startingDiameter * increasePortion);
        float newDiameter = Mathf.Min(target, maxDiameter);
        _rectTransform.sizeDelta = new Vector2(newDiameter, newDiameter);
    }
}
