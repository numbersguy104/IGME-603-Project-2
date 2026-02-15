using UnityEngine;
using UnityEngine.UI;

//Data for the QTE prompts for each stage of difficulty.

[System.Serializable]
struct PopupStageData
{
    [Tooltip("How often a popup will appear in this stage - every X seconds")]
    public float interval;

    [Tooltip("How long each popup will last in this stage, in seconds")]
    public float duration;
}

[System.Serializable]
struct QTEStageData
{
    [Tooltip("Number of key presses required for QTEs in this stage")]
    public int inputCount;

    [Tooltip("Letter keys that can appear in QTEs for this stage, stored as a string")]
    public string validKeys;
}

[System.Serializable]
struct BarStageData
{
    [Tooltip("Speed of the arrow per second, as a percentage of the bar's total height")]
    public float arrowSpeedPercentage;

    [Tooltip("Size of the success area in this stage, as a percentage of the bar's total height")]
    public float successPercentage;

    [Tooltip("Size of the perfect area in this stage, as a percentage of the bar's total height")]
    public float perfectPercentage;
}
public class DistractionManager : MonoBehaviour
{
    private bool gameStarted = false;
    private int difficultyStage = 0;
    private float time = 0.0f;
    private Vector2 screenSize;

    [Header("General")]
    [Header("Note: The first value for this should always be 0!")]
    [Tooltip("Time threshold for each stage of difficulty. The first value should always be 0, representing the first stage starting 0 seconds into gameplay, or immediately.")]
    [SerializeField] private float[] difficultyTimes = { 0f };

    [Header("Popup")]
    [Tooltip("Rules for popups (their frequency and duration) for each difficulty stage")]
    [SerializeField] private PopupStageData[] popupStageData;
    [Tooltip("Sprites that popups can use")]
    [SerializeField] private Sprite[] popupSprites;
    [Tooltip("Minimum scale of popups relative to the base image")]
    [SerializeField] private float popupScaleMinimum = 0.7f;
    [Tooltip("Maximum scale of popups relative to the base image")]
    [SerializeField] private float popupScaleMaximum = 1.3f;

    private float nextPopupTime;

    [Header("QTE")]
    [Tooltip("Rules for QTE inputs for each difficulty stage")]
    [SerializeField] private QTEStageData[] qteStageData;
    [Tooltip("Time between each QTE appearing")]
    [SerializeField] private float qteInterval = 5.0f;

    private float nextQTETime;

    [Header("Bar Minigame")]
    [Tooltip("Rules for the bar minigame for each difficulty stage")]
    [SerializeField] private BarStageData[] barStageData;
    [Tooltip("How much the circle should grow for each perfect completion of the bar minigame, as a fraction (0 to 1) of the circle's starting size")]
    [SerializeField] private float barGrowthOnPerfect = 0.05f;
    [Tooltip("Time between each bar minigame appearing")]
    [SerializeField] private float barInterval = 11.0f;

    private float nextBarTime;

    [Header("Layers")]
    [Header("Note: Layers range from 1 to 5. Higher numbered layers render in front.")]
    [Tooltip("Layer for popups")]
    [SerializeField] private int popupLayer = 1;
    [Tooltip("Layer for QTEs")]
    [SerializeField] private int qteLayer = 3;
    [Tooltip("Layer for bar minigames")]
    [SerializeField] private int barLayer = 5;

    [Header("Prefabs")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private GameObject qtePrefab;
    [SerializeField] private GameObject barPrefab;

    private void Start()
    {
        screenSize = GetComponent<RectTransform>().rect.size * GetComponent<Canvas>().scaleFactor;

        nextPopupTime = popupStageData[0].interval;
        nextQTETime = qteInterval;
        nextBarTime = barInterval;

        FindAnyObjectByType<FocusCircle>().GameStart.AddListener(StartGame);
    }

    private void Update()
    {
        if (!gameStarted) return;

        time += Time.deltaTime;

        if ((difficultyStage < difficultyTimes.Length - 1)
            && (time > difficultyTimes[difficultyStage + 1]))
        {
            //Increase the difficulty without throwing off the time between timed events (such as QTEs)
            //This code ensures there is no extended gap between timed events when difficulty increases
            nextPopupTime -= popupStageData[difficultyStage].interval;
            difficultyStage++;
            nextPopupTime += popupStageData[difficultyStage].interval;

            //Debug.Log("Increasing difficulty to stage " + difficultyStage + "!");
        }

        if (time > nextPopupTime)
        {
            PopupStageData data = popupStageData[difficultyStage];
            nextPopupTime += data.interval;

            Transform layerParent = GetLayerParent(popupLayer);
            GameObject popup = Instantiate(popupPrefab, layerParent);

            Popup popupScript = popup.GetComponent<Popup>();
            
            popupScript.SetDuration(data.duration);
            Sprite randomSprite = popupSprites[Random.Range(0, popupSprites.Length)];
            popupScript.SetSprite(randomSprite);

            float randomScale = Random.Range(popupScaleMinimum, popupScaleMaximum);
            popup.transform.localScale *= randomScale;

            Image popupImage = popup.GetComponent<Image>();
            Vector2 popupSize = new Vector2(popupImage.preferredHeight, popupImage.preferredWidth);
            Vector2 position = RandomScreenPosition(popupSize / 2);
            popup.transform.position = position;
        }

        if (time > nextQTETime)
        {
            nextQTETime += qteInterval;

            Transform layerParent = GetLayerParent(qteLayer);
            GameObject qte = Instantiate(qtePrefab, layerParent);

            QTEStageData data = qteStageData[difficultyStage];
            int keyCount = data.inputCount;
            string validKeys = data.validKeys;
            string chosenKeys = "";
            for (int i = 0; i < keyCount; i++)
            {
                int keyIndex = Random.Range(0, validKeys.Length);
                chosenKeys = chosenKeys + validKeys[keyIndex];
            }
            while (chosenKeys.Contains("ass")) {
                chosenKeys = chosenKeys.Replace("ass", "sas");
            }
            qte.GetComponent<QTE>().SetKeys(chosenKeys);

            Vector2 qteSize = qte.GetComponent<RectTransform>().rect.size;
            Vector2 position = RandomScreenPosition(qteSize / 2);
            qte.transform.position = position;
        }

        if (time > nextBarTime)
        {
            nextBarTime += barInterval;

            Transform layerParent = GetLayerParent(barLayer);
            GameObject bar = Instantiate(barPrefab, layerParent);

            BarStageData data = barStageData[difficultyStage];
            float successPortion = data.successPercentage / 100.0f;
            float perfectPortion = data.perfectPercentage / 100.0f;
            bar.GetComponent<BarMinigame>().SetAreas(successPortion, perfectPortion);
            bar.GetComponent<BarMinigame>().SetPerfectGrowth(barGrowthOnPerfect);
            bar.GetComponent<BarMinigame>().SetArrowSpeed(data.arrowSpeedPercentage);

            Vector2 barSize = bar.GetComponent<RectTransform>().rect.size;
            Vector2 position = RandomScreenPosition(barSize / 2);
            bar.transform.position = position;
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

    private Transform GetLayerParent(int layerNumber)
    {
        Transform result = transform.Find("Layer " + layerNumber);
        if (result)
        {
            return result;
        }
        else
        {
            Debug.LogWarning("Invalid layer specified! Drawing to canvas directly...");
            return transform;
        }
    }
}
