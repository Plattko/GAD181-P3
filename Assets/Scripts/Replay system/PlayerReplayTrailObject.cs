using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReplayTrailObject : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2.0f; // Duration in seconds for the object to completely fade away
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float timer;

    void Start()
    {
        // Get the SpriteRenderer component of the object
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        timer = 0f;
    }

    void Update()
    {
        if (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, timer / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
        else
        {
            Destroy(gameObject); // Destroy the object when it's fully transparent
        }
    }
}
