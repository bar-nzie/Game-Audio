using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Temporaryupgrades : MonoBehaviour
{
    private float initialDamage = 20;
    private float increaseFactor = 1.05f;
    public Bullet Bullet;

    private float initialSpeed = 10f;
    private float increaseSpeedFactor = 1.07f;
    public Movement player;

    private float initialRegen = 0f;
    private float increaseRegen = 1f;
    public Health health;

    private float initialCooldown = 2f;
    private float cooldownFactor = 0.8f;

    public Score coins;

    public GameObject damageVisual;
    public GameObject speedVisual;
    public GameObject regenVisual;
    public GameObject dashVisual;
    public GameObject coinVisual;
    public GameObject upgradeScreen;
    public GameObject UI;

    private int dmgCount = 0;
    private int speedCount = 0;
    private int regenCount = 0;
    private bool dashUpgrade = false;
    private float defaultDamage;

    public GameObject coinButton;

    // Start is called before the first frame update
    void Start()
    {
        upgradeScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    coins.CoinsUpdate(100);
        //}
    }

    public void DamageIncrease()
    {
        initialDamage *= increaseFactor;
        upgradeScreen.SetActive(false);
        Time.timeScale = 1f;
        UI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void InitialDamage(float damage)
    {
        initialDamage = damage;
        defaultDamage = damage;
    }

    public float GetSaveDamage()
    {
        return defaultDamage;
    }

    public float GetDamage()
    {
        return initialDamage;
    }

    public void speedIncrease()
    {
        initialSpeed *= increaseSpeedFactor;
        player.SpeedUpgrade(initialSpeed);
        upgradeScreen.SetActive(false);
        Time.timeScale = 1f;
        UI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void regenIncrease()
    {
        initialRegen += increaseRegen;
        health.RegenerationUpgrade(initialRegen);
        upgradeScreen.SetActive(false);
        Time.timeScale = 1f;
        UI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DashCooldownUpgrade()
    {
        initialCooldown *= cooldownFactor;
        player.CooldownUpgrade(initialCooldown);
        upgradeScreen.SetActive(false);
        Time.timeScale = 1f;
        UI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PermanentCoin()
    {
        coins.CoinsUpdate(20);
        upgradeScreen.SetActive(false);
        Time.timeScale = 1f;
        UI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpgradeTime()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        upgradeScreen.SetActive(true);

        damageVisual.SetActive(false);
        speedVisual.SetActive(false);
        regenVisual.SetActive(false);
        dashVisual.SetActive(false);
        coinVisual.SetActive(false);

        bool upgradeChosen = false;
        int attempts = 0;

        if (dmgCount == 10 && speedCount == 10 && regenCount == 10 && dashUpgrade)
        {
            coinVisual.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        while (!upgradeChosen && attempts < 10)
        {
            int random = Random.Range(0, 4); 
            switch (random)
            {
                case 0:
                    if (dmgCount < 10)
                    {
                        damageVisual.SetActive(true);
                        dmgCount++;
                        upgradeChosen = true;
                    }
                    break;
                case 1:
                    if (speedCount < 10)
                    {
                        speedVisual.SetActive(true);
                        speedCount++;
                        upgradeChosen = true;
                    }
                    break;
                case 2:
                    if (regenCount < 10)
                    {
                        regenVisual.SetActive(true);
                        regenCount++;
                        upgradeChosen = true;
                    }
                    break;
                case 3:
                    if (!dashUpgrade)
                    {
                        dashVisual.SetActive(true);
                        dashUpgrade = true;
                        upgradeChosen = true;
                    }
                    break;
            }

            attempts++;
        }

        coinVisual.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(coinButton);
    }



}
