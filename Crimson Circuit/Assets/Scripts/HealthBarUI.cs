using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image[] segments; // Assign your 5 segment images in order
    private float maxHealth = 100f;
    public Health Health;

    private void Start()
    {
        maxHealth = Health.GetMaxHealth();
        UpdateHealthBar(maxHealth);
    }

    public void UpdateHealthBar(float currentHealth)
    {
        maxHealth = Health.GetMaxHealth();
        int segs = Mathf.FloorToInt(maxHealth/20f);
        int fullSegments = Mathf.FloorToInt(currentHealth / 20f);
        float partial = currentHealth % 20f;

        for (int i = 0; i < segs; i++)
        {
            if (i < fullSegments)
            {
                SetAlpha(segments[i], 1f); // fully visible
            }
            else if (i == fullSegments && partial > 0)
            {
                SetAlpha(segments[i], partial / 20f); // partially visible
            }
            else
            {
                SetAlpha(segments[i], 0f); // invisible
            }
        }
        for (int i = segs;i < segments.Length; i++)
        {
            SetAlpha(segments[i], 0f);
        }
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
