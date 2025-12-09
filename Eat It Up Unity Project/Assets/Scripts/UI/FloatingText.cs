using TMPro;    
using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float duration = 1f;
    public float moveUpAmount = 1f;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        StartCoroutine(Animate());
    }

    public void SetText(string t)
    {
        text.text = t;
    }

    IEnumerator Animate()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, moveUpAmount, 0);
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, time / duration);

            // Fade out
            Color c = text.color;
            c.a = 1 - (time / duration);
            text.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }

    public void SetScreenPosition(Vector3 screenPos)
    {
        rectTransform.position = screenPos;
    }
}
