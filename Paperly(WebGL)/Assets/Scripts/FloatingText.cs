using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float moveUpSpeed = 1f;
    public float fadeDuration = 1f;

    private TextMeshProUGUI text;
    private Color startColor;
    private Vector3 startPosition;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        startPosition = transform.position; // Store original position
    }

    public void Setup(float amount)
    {
        // Reset to starting position and full opacity
        transform.position = startPosition;
        text.color = Color.white; // Reset color first
        gameObject.SetActive(true);

        text.text = "+" + amount.ToString("0");

        // Color based on bonus amount
        if (amount >= 10)
            startColor = new Color(1f, 0.5f, 0f);        // Orange
        else if (amount >= 5)
            startColor = new Color(1f, 0.84f, 0f);       // Gold
        else
            startColor = new Color(1f, 0.84f, 0f);                   // Normal bonus

        text.color = startColor;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            // Move upward
            transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

            // Fade
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // Hide instead of destroy for reuse
        gameObject.SetActive(false);
    }
}
