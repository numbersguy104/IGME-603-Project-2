using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTE : MonoBehaviour
{
    public static int completedQTEs = 0;
    public static float totalQTETime;

    private bool isActive = true;
    private string keys = "";
    private string possibleInputs = "";
    private int keyCountDone = 0;
    private float timeActive = 0.0f;
    private float timeDisplayRed = 0.0f;

    Image _popupSprite;
    Image _popupOutline;

    private void Start()
    {
        _popupSprite = transform.Find("Popup Sprite").GetComponent<Image>();
        _popupOutline = transform.Find("Outline").GetComponent<Image>();
        int siblingCount = transform.parent.childCount;
        int siblingIndex = transform.GetSiblingIndex();
        if (siblingCount > 1)
        {
            //Render behind other QTEs that spawned earlier
            transform.SetSiblingIndex(0);

            SetActive(false);
        }
    }

    private void Update()
    {
        if (isActive)
        {
            if (timeDisplayRed > 0.0f)
            {
                timeDisplayRed -= Time.deltaTime;
                timeDisplayRed = Mathf.Max(timeDisplayRed, 0.0f);
                UpdateKeyDisplay();
            }
            string nextKey = keys[keyCountDone].ToString();

            if (Input.GetKeyDown(nextKey))
            {
                keyCountDone++;
                if (keyCountDone == keys.Length)
                {
                    completedQTEs++;
                    totalQTETime += timeActive;
                    Destroy(gameObject);
                }
                else
                {
                    UpdateKeyDisplay();
                }
            }
            else
            {
                foreach (char wrongChar in possibleInputs)
                {
                    string wrongInput = wrongChar.ToString();
                    if (wrongInput != nextKey && Input.GetKeyDown(wrongInput))
                    {
                        keyCountDone = 0;
                        timeDisplayRed = 0.5f;
                        UpdateKeyDisplay();
                        break;
                    }
                }
            }
        }
        else
        {
            //If this popup is in front, become active
            int siblingCount = transform.parent.childCount;
            int siblingIndex = transform.GetSiblingIndex();

            if (siblingIndex == siblingCount - 1)
            {
                SetActive(true);
            }
        }

        timeActive += Time.deltaTime;
    }

    public void SetKeys(string newKeys)
    {
        keys = newKeys;
        UpdateKeyDisplay();
    }

    public void SetAllInputs(string inputs)
    {
        possibleInputs = inputs;
    }

    private void UpdateKeyDisplay()
    {
        string result = "<color=green>";

        for (int i = 0; i < keys.Length; i++)
        {
            if (i == keyCountDone)
            {
                Color textColor = Color.black;
                textColor.r = timeDisplayRed / 0.5f;
                string colorCode = ColorUtility.ToHtmlStringRGB(textColor);
                result = result + "<color=#" + colorCode + ">";
            }
            result = result + "[" + keys[i].ToString().ToUpper() + "]";
            if (i < keys.Length - 1)
            {
                result = result + " ";
            }
        }

        GetComponentInChildren<TextMeshProUGUI>().text = result;
    }

    //Sets whether this QTE is currently the one receiving inputs
    //Also changes the QTE's appearance based on this
    private void SetActive(bool active)
    {
        isActive = active;

        if (active)
        {
            _popupSprite.color = Color.white;
            _popupOutline.enabled = true;
        }
        else
        {
            _popupSprite.color = Color.gray;
            _popupOutline.enabled = false;
        }
    }
}
