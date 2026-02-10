using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    public PermanentUpgrades PermanentUpgrades;
    public SaveData Data;
    public Temporaryupgrades temporaryupgrades;
    public Health health;
    public Score Score;
    public SaveManager SaveManager;
    public GrenadeAbility GrenadeAbility;
    public Forcefield forcefield;
    public RuneAbillity RuneAbillity;

    private void Start()
    {
        Loading();
    }

    public void Saving()
    {
        Data = new SaveData();
        Data.playerDamage = temporaryupgrades.GetSaveDamage();
        Data.playerHealth = health.GetMaxHealth();
        Data.coins = Score.GetCoins();
        Data.healthLevel = PermanentUpgrades.GetCount();
        Data.damageLevel = PermanentUpgrades.GetDamageLevel();
        Data.damageCost = PermanentUpgrades.GetDamageCost();
        Data.healthCost = PermanentUpgrades.GetHealthCost();
        Data.grenadeAbility = GrenadeAbility.grenadeUnlocked();
        Data.grenadeLevel = PermanentUpgrades.GetGrenadeLevel();
        Data.grenadeCost = PermanentUpgrades.GetGrenadeCost();
        Data.throwCooldown = GrenadeAbility.GetCooldown();
        Data.forcefieldAbility = forcefield.forcefieldUnlocked();
        Data.forcefieldLevel = PermanentUpgrades.GetForcefieldLevel();
        Data.forcefieldCost = PermanentUpgrades.GetForcefieldCost();
        Data.forcefieldCooldown = forcefield.GetCooldown();
        Data.runeAbility = RuneAbillity.forcefieldUnlocked();

        SaveManager.SaveGame(Data);
    }

    public void  Loading()
    {
        SaveData data = SaveManager.LoadGame();
        if(data != null)
        {
            temporaryupgrades.InitialDamage(data.playerDamage);
            health.SetPlayerHealth(data.playerHealth);
            Score.SetCoins(data.coins);
            PermanentUpgrades.SetCount(data.healthLevel);
            PermanentUpgrades.SetDamageLevel(data.damageLevel);
            PermanentUpgrades.SetDamageCost(data.damageCost);
            PermanentUpgrades.SetHealthCost(data.healthCost);
            GrenadeAbility.unlockCheck(data.grenadeAbility);
            PermanentUpgrades.SetGrenadeLevel(data.grenadeLevel);
            PermanentUpgrades.SetGrenadeCost(data.grenadeCost);
            GrenadeAbility.SetCooldown(data.throwCooldown);
            forcefield.unlockCheck(data.forcefieldAbility);
            PermanentUpgrades.SetForcefieldCost(data.forcefieldCost);
            PermanentUpgrades.SetForcefieldLevel(data.forcefieldLevel);
            forcefield.SetCooldown(data.forcefieldCooldown);
            RuneAbillity.unlockCheck(data.runeAbility);
        }
    }
}
