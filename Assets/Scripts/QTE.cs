using TMPro;
using UnityEngine;

public class QTE : MonoBehaviour
{
    private string keys = "";
    private int keyCountDone = 0;

    private void Update()
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
}
