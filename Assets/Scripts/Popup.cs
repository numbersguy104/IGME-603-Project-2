using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    private float duration = 1.0f;
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    public void SetSprite(Sprite newSprite)
    {
        GetComponent<Image>().sprite = newSprite;
    }

    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
