using UnityEngine;
using UnityEngine.UI;

public class BarMinigame : MonoBehaviour
{
    //Ratio of the height of the inside of the bar's sprite to that of its full bounding box
    //The bounding box is slightly too big
    private const float HEIGHT_BUFFER_MULT = 370.0f / 384.0f;

    private float cooldownTimer = 0.0f;

    private float arrowSpeed;
    private float growthOnPerfect;

    private float barTop;
    private float barBottom;

    private float successTop;
    private float successBottom;
    private float perfectTop;
    private float perfectBottom;

    private RectTransform arrow;

    private void Start()
    {
        arrow = transform.Find("Arrow").GetComponent<RectTransform>();

        RectTransform barRect = GetComponent<RectTransform>();
        float barHeight = barRect.sizeDelta.y * HEIGHT_BUFFER_MULT;
        barTop = barHeight / 2;
        barBottom = -barTop;
        
        Vector2 arrowPos = arrow.localPosition;
        arrowPos.y = barBottom;
        arrow.localPosition = arrowPos;
    }

    private void Update()
    {
        if (cooldownTimer > 0.0f)
        {
            Color color = Color.white;
            color.g = 1.0f - cooldownTimer;
            color.b = color.g;
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0.0f)
            {
                color = Color.white;
            }
            GetComponent<Image>().color = color;
        }
        else
        {
            GetComponent<Image>().color = Color.white;

            Vector2 arrowPos = arrow.localPosition;

            if (Input.GetKeyDown("space"))
            {
                if ((arrowPos.y > successBottom) && (arrowPos.y < successTop))
                {
                    if ((arrowPos.y > perfectBottom) && (arrowPos.y < perfectTop))
                    {
                        FindAnyObjectByType<FocusCircle>().Grow(growthOnPerfect);
                    }
                    Destroy(gameObject);
                }
                else
                {
                    arrowPos.y = barBottom;
                    cooldownTimer = 1.0f;
                }
            }

            arrowPos.y += arrowSpeed * Time.deltaTime;
            if (arrowPos.y > barTop)
            {
                arrowPos.y -= (barTop - barBottom);
            }
            arrow.localPosition = arrowPos;
        }        
    }

    public void SetPerfectGrowth(float growthPortion)
    {
        growthOnPerfect = growthPortion;
    }

    //Sets the moving arrow's speed to a percentage of the bar per second
    public void SetArrowSpeed(float percentagePerSecond)
    {
        float barHeight = GetComponent<RectTransform>().rect.height * HEIGHT_BUFFER_MULT;
        arrowSpeed = barHeight * percentagePerSecond / 100;
    }

    //Resizes the success area and perfect area to their respective portions.
    //Inputs are fractions of the bar's height (should be between 0 and 1).
    //Also randomly positions the areas on the bar, staying within bounds.
    public void SetAreas(float successPortion, float perfectPortion)
    {
        float barHeight = GetComponent<RectTransform>().rect.height * HEIGHT_BUFFER_MULT;

        //Calculate random center position of the areas
        float randomRange = (barHeight * (1.0f - successPortion)) / 2.0f;

        float areaHeight = Random.Range(-randomRange, randomRange);

        RectTransform successArea = transform.Find("Success Area").GetComponent<RectTransform>();
        RectTransform perfectArea = transform.Find("Perfect Area").GetComponent<RectTransform>();

        Vector2 pos = successArea.localPosition;
        pos.y = areaHeight;
        successArea.localPosition = pos;
        perfectArea.localPosition = pos;

        Vector2 sizeS = successArea.sizeDelta;
        sizeS.y = barHeight * successPortion;
        successArea.sizeDelta = sizeS;
        Vector2 sizeP = perfectArea.sizeDelta;
        sizeP.y = barHeight * perfectPortion;
        perfectArea.sizeDelta = sizeP;

        successTop = pos.y + sizeS.y / 2.0f;
        successBottom = pos.y - sizeS.y / 2.0f;
        perfectTop = pos.y + sizeP.y / 2.0f;
        perfectBottom = pos.y - sizeP.y / 2.0f;
    }
}
