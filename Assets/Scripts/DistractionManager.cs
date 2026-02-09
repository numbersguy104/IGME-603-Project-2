using UnityEngine;
using UnityEngine.UI;

public class DistractionManager : MonoBehaviour
{
    private bool gameStarted = false;
    private float time = 0.0f;
    private Vector2 screenSize;

    [Header("Popup")]
    [Tooltip("How often a popup will appear - every X seconds")]
    [SerializeField] private float popupRate = 1.0f;
    [Tooltip("List of sprites for popups")]
    [SerializeField] private Sprite[] popupSprites;
    [Tooltip("Minimum popup duration, in seconds")]
    [SerializeField] private float popupDurationMinimum = 3.0f;
    [Tooltip("Maximum popup duration, in seconds")]
    [SerializeField] private float popupDurationMaximum = 5.0f;

    private float nextPopupTime;

    [Header("QTE")]
    [Tooltip("How often a QTE will appear - every X seconds")]
    [SerializeField] private float qteRate = 5.0f;
    [Tooltip("Valid keys that can appear in QTEs, stored as a lowercase string without spaces")]
    [SerializeField] private string qteKeys = "wasd";
    [Tooltip("Minimum amount of key presses needed for QTE prompts")]
    [SerializeField] private int qteInputsMinimum = 3;
    [Tooltip("Maximum amount of key presses needed for QTE prompts")]
    [SerializeField] private int qteInputsMaximum = 7;

    private float nextQTETime;

    [Header("Prefabs")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private GameObject qtePrefab;

    private void Start()
    {
        screenSize = GetComponent<RectTransform>().rect.size * GetComponent<Canvas>().scaleFactor;

        nextPopupTime = popupRate;
        nextQTETime = qteRate;

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
            Popup popupScript = popup.GetComponent<Popup>();
            popupScript.SetDuration(Random.Range(popupDurationMinimum, popupDurationMaximum));
            popupScript.SetSprite(popupSprites[Random.Range(0, popupSprites.Length)]);

            Image popupImage = popup.GetComponent<Image>();
            Vector2 popupSize = new Vector2(popupImage.preferredHeight, popupImage.preferredWidth);
            Vector2 position = RandomScreenPosition(popupSize / 2);
            popup.transform.position = position;
        }

        if (time > nextQTETime)
        {
            nextQTETime += qteRate;

            GameObject qte = Instantiate(qtePrefab, transform);
            int keyCount = Random.Range(qteInputsMinimum, qteInputsMaximum + 1);
            string chosenKeys = "";
            for (int i = 0; i < keyCount; i++)
            {
                int keyIndex = Random.Range(0, qteKeys.Length);
                chosenKeys = chosenKeys + qteKeys[keyIndex];
            }
            while (chosenKeys.Contains("ass")) {
                chosenKeys = chosenKeys.Replace("ass", "sas");
            }
            qte.GetComponent<QTE>().SetKeys(chosenKeys);

            Vector2 qteSize = qte.GetComponent<RectTransform>().rect.size;
            Vector2 position = RandomScreenPosition(qteSize / 2);
            qte.transform.position = position;
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
