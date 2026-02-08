using UnityEngine;

public class Popup : MonoBehaviour
{
    private float duration = 1.0f;
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
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
