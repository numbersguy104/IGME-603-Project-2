using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTE : MonoBehaviour
{
    private bool isActive = true;
    private string keys = "";
    private int keyCountDone = 0;

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
            string nextKey = keys[keyCountDone].ToString();

            if (Input.GetKeyDown(nextKey))
            {
                keyCountDone++;
                if (keyCountDone == keys.Length)
                {
                    Destroy(gameObject);
                }
                else
                {
                    UpdateKeyDisplay();
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
    }

    public void SetKeys(string newKeys)
    {
        keys = newKeys;
        UpdateKeyDisplay();
    }

    private void UpdateKeyDisplay()
    {
        string result = "<color=green>";

        for (int i = 0; i < keys.Length; i++)
        {
            if (i == keyCountDone)
            {
                result = result + "<color=black>";
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
