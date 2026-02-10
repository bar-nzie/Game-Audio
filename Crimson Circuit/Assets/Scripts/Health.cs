using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public float health;
    public bool isPlayer;
    public HealthBarUI healthBarUI;
    private Renderer rend;
    private Color originalColor;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip shootSound2;
    private float regenValue = 0f;
    public float regenDelay = 5f;
    public float regenRate = 1f;
    private float lastDamageTime;
    private bool isRegenerating;
    private float maxHealth = 100f;
    public GameInfoManager gameInfoManager;

    private void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    private void Update()
    {

        if (isPlayer && health > 0 && health < maxHealth) // Change 100 to your max health if needed
        {
            if (Time.time >= lastDamageTime + regenDelay && !isRegenerating)
            {
                StartCoroutine(RegenerateHealth());
            }
        }
    }

    public void SetHealth(float buff)
    {
        health = health * buff;
        healthBarUI?.UpdateHealthBar(health);
        Debug.Log(health);
    }
    public void TakeDamage(float damage)
    {
        lastDamageTime = Time.time;

        if (isRegenerating)
        {
            StopAllCoroutines();
            isRegenerating = false;
        }

        StartCoroutine(Flash());
        health -= damage;
        audioSource.PlayOneShot(shootSound);

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(health);
        }

        if(health <= 0 && gameObject.CompareTag("Player") == true)
        {
            Debug.Log("playerDead");
            SceneManager.LoadScene("Respawn");
        }

        if (health <= 0 )
        {
            audioSource.PlayOneShot(shootSound2);
            Destroy(gameObject);
        }
    }

    public void RegenerationUpgrade(float value)
    {
        regenValue = value;
    }

    public float GetMaxHealth() { return maxHealth; }

    public void PlayerHealth(float value)
    {
        if (!isPlayer) return;
        maxHealth += value;
        health = maxHealth;
    }

    public void SetPlayerHealth(float value)
    {
        maxHealth = value;
        healthBarUI.UpdateHealthBar(maxHealth);
        health = maxHealth;
    }

    public float getEnemyHealth() { return health; }

    private IEnumerator RegenerateHealth()
    {
        if (!isPlayer) yield break;
        isRegenerating = true;

        yield return new WaitForSeconds(regenDelay);

        while (health < maxHealth)
        {
            health += regenValue;
            health = Mathf.Min(health, maxHealth);

            if (healthBarUI != null)
            {
                healthBarUI.UpdateHealthBar(health);
            }

            yield return new WaitForSeconds(3f);
        }

        isRegenerating = false;
    }

    private IEnumerator Flash()
    {
        if (rend != null)
        {
            rend.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            rend.material.color = originalColor;
        }
    }
}
