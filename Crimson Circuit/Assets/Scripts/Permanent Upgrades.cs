using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentUpgrades : MonoBehaviour
{
    private float initialHealth = 100f;
    private float healthUpgrade = 20f;
    public Health health;
    public HealthBarUI healthBar;
    public Score coins;

    private float initialDamage = 20f;
    private float damageIncrease = 1.1f;
    public Temporaryupgrades Temporaryupgrades;

    private int money;
    private int healthCost = 50;
    private int damageCost = 30;
    private int grenadeCost = 30;
    private int forcefieldCost = 30;

    private int count = 0;
    private int damageLevel = 0;
    private int grenadeLevel = 0;
    private int forcefieldLevel = 0;

    private float throwCooldown = 15f;
    private float forcefieldCooldown = 30f;
    public GrenadeAbility grenadeAbility;
    public Forcefield Forcefield;

    public GameObject healthVisual;
    public GameObject damageVisual;
    public GameObject grenadeVisual;
    public GameObject forcefieldVisual;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.UpdateHealthBar(initialHealth);
        //Forcefield.SetCooldown(forcefieldCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.K))
        //{
        //    DamageIncrease();
        //}
    }

    public void HealthIncrease()
    {
        money = coins.GetCoins();
        if (count >= 5 || money < healthCost)
        {
            return;
        }
        healthVisual.SetActive(true);
        health.PlayerHealth(healthUpgrade);
        initialHealth += healthUpgrade;
        healthBar.UpdateHealthBar(initialHealth);
        money -= healthCost;
        coins.SetCoins(money);
        healthCost += 50;
        count++;
        Debug.Log(count);
        StartCoroutine(upgradeDuration());
    }

    public void GrenadeUpgrade()
    {
        money = coins.GetCoins();
        if (money < grenadeCost || grenadeLevel >= 7)
        {
            return;
        }
        grenadeVisual.SetActive(true);
        throwCooldown *= 0.85f;
        grenadeAbility.SetCooldown(throwCooldown);
        money -= grenadeCost;
        coins.SetCoins(money);
        grenadeCost += 30;
        grenadeLevel++;
        StartCoroutine(upgradeDuration());
    }

    public void ForcefieldUpgrade()
    {
        money = coins.GetCoins();
        if (money < forcefieldCost || forcefieldLevel >= 7)
        {
            return;
        }
        forcefieldVisual.SetActive(true);
        forcefieldCooldown *= 0.85f;
        Forcefield.SetCooldown(forcefieldCooldown);
        money -= forcefieldCost;
        coins.SetCoins(money);
        forcefieldCost += 30;
        forcefieldLevel++;
        StartCoroutine(upgradeDuration());
    }

    private IEnumerator upgradeDuration()
    {
        yield return new WaitForSeconds(3f);

        healthVisual.SetActive(false);
        damageVisual.SetActive(false);
        grenadeVisual.SetActive(false);
        forcefieldVisual.SetActive(false);
    }

    public void DamageIncrease()
    {
        money = coins.GetCoins();
        if(money < damageCost)
        {
            return;
        }
        damageVisual.SetActive(true);
        initialDamage *= damageIncrease;
        Temporaryupgrades.InitialDamage(initialDamage);
        money -= damageCost;
        coins.SetCoins(money);
        damageCost += 30;
        damageLevel++;
        StartCoroutine(upgradeDuration());
    }

    public float GetDamage() {  return initialDamage; }

    public int GetDamageCost() { return damageCost; }
    public int GetHealthCost() { return healthCost; }
    public int GetCount() { return count; }
    public int GetDamageLevel() { return damageLevel; }

    public void SetDamageCost(int value)
    {
        damageCost = value;
    }
    public int GetGrenadeLevel() { return grenadeLevel; }
    public int GetGrenadeCost() { return grenadeCost; }

    public int GetForcefieldLevel() { return forcefieldLevel; }
    public int GetForcefieldCost() { return forcefieldCost; }


    public void SetHealthCost(int value) { healthCost = value; }
    public void SetCount(int value) { count = value; }
    public void SetDamageLevel(int value) 
    { 
        damageLevel = value;
    }
    public void SetGrenadeLevel(int value) 
    { 
        grenadeLevel = value;
        throwCooldown = grenadeAbility.GetCooldown();
    }
    public void SetGrenadeCost(int value) { grenadeCost = value; }

    public void SetForcefieldLevel(int value)
    {
        forcefieldLevel = value;
        forcefieldCooldown = Forcefield.GetCooldown();
    }

    public void SetForcefieldCost(int value)
    { 
        if (value == 0)
        {
            return;
        }
        forcefieldCost = value; 
    }
}
