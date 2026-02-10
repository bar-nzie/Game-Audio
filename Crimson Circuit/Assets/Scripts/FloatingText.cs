using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public Transform mainCam;
    public Transform unit;
    public Transform canvas;
    public Transform player;
    public GameObject Image;
    public GameObject interact;
    public PermanentUpgrades permanent;
    public TextMeshProUGUI damage;

    private PlayerControls controls;
    public Image interactImage;

    private float range = 10f;
    private Vector3 finalScale = new Vector3(0.1f, 0.1f, 0.1f);
    private float chargeTime = 0.3f;
    private bool inRange = false;
    private float holdDuration;
    private bool isHolding;
    private float holdTimeRequired = 1f;

    public bool isDamage;
    public bool isHealth;
    public bool isGrenade;
    public bool isForcefield;
    private int damageCost;
    private int healthCost;
    private int grenadeCost;
    private int forcefieldCost;
    private int count;
    private int grenadeLevel;
    private int forcefieldLevel;
    private bool forcefieldUnlocked;
    private bool grenadeUnlocked;

    public Forcefield forcefield;
    public GrenadeAbility grenadeAbility;
    

    public Vector3 offset;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Interact.performed += ctx => StartHolding();
        controls.Player.Interact.canceled += ctx => StopHolding();
    }
    private void OnDisable()
    {
        controls.Player.Interact.performed -= ctx => StartHolding();
        controls.Player.Interact.canceled -= ctx => StopHolding();
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        damageCost = permanent.GetDamageCost();
        healthCost = permanent.GetHealthCost();
        grenadeCost = permanent.GetGrenadeCost();
        forcefieldCost = permanent.GetForcefieldCost();
        forcefieldUnlocked = forcefield.GetUnlockedStatus();
        grenadeUnlocked = grenadeAbility.GetUnlockedStatus();
        transform.SetParent(canvas);
        if (isDamage)
        {
            damage.text = "Permanently increase damage by 10%\n Cost: " +  damageCost.ToString();
        }
        if (isHealth)
        {
            damage.text = "Permanently increase health by 20hp\n Cost: " + healthCost.ToString();
        }
        if (isGrenade && grenadeUnlocked)
        {
            damage.text = "Permanently decrease grenade cooldown:  " + grenadeCost.ToString();
        }
        if (isGrenade && !grenadeUnlocked)
        {
            damage.text = "LOCKED";
        }
        if (isForcefield && forcefieldUnlocked)
        {
            damage.text = "Permanently decrease forcefield cooldown:  " + forcefieldCost.ToString();
        }
        if (isForcefield && !forcefieldUnlocked)
        {
            damage.text = "LOCKED";
        }
    }

    private void StartHolding()
    {
        if (inRange)
        {
            isHolding = true;
            holdDuration = 0f;
        }
    }

    private void StopHolding()
    {
        isHolding = false;
        holdDuration = 0f;
        interactImage.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        forcefieldUnlocked = forcefield.GetUnlockedStatus();
        grenadeUnlocked = grenadeAbility.GetUnlockedStatus();
        canvas.rotation = Quaternion.LookRotation(unit.position - mainCam.transform.position);
        canvas.position = unit.position + offset;

        float distance = Vector3.Distance(unit.position, player.position);

        if (distance <= range)
        {
            inRange = true;
        } 
        else
        {
            inRange= false;
        }

        if (inRange)
        {
            Image.transform.localScale = finalScale;
            interact.SetActive(true);
            
        }
        if (!inRange)
        {
            Image.transform.localScale = Vector3.zero;
            interact.SetActive(false);
        }

        if (isHolding && inRange)
        {
            holdDuration += Time.deltaTime;

            float fill = Mathf.Clamp01(holdDuration / holdTimeRequired);
            interactImage.fillAmount = fill;

            if (holdDuration >= holdTimeRequired)
            {
                Upgrade(); 
                isHolding = false;
                holdDuration = 0f;
                interactImage.fillAmount = 0f;
            }
        }
        else
        {
            interactImage.fillAmount = Mathf.MoveTowards(interactImage.fillAmount, 0f, Time.deltaTime * 3f);
        }

        count = permanent.GetCount();
        grenadeLevel = permanent.GetGrenadeLevel();
        forcefieldLevel = permanent.GetForcefieldLevel();

        if (isDamage)
        {

            damageCost = permanent.GetDamageCost();
            damage.text = "Permanently increase damage by 10%\n Cost: " + damageCost.ToString();
        }
        if (count > 4 && isHealth)
        {

            damage.text = "Permanently increase health by 20hp\n MAX LVL";
            return;
        }
        if (isHealth)
        {

            healthCost = permanent.GetHealthCost();
            damage.text = "Permanently increase health by 20hp\n Cost: " + healthCost.ToString();
        }
        if (grenadeLevel > 6 && isGrenade)
        {

            damage.text = "Permanently decrease grenade cooldown: MAX LVL";
            return;
        }
        if (isGrenade && grenadeUnlocked)
        {

            grenadeCost = permanent.GetGrenadeCost();
            damage.text = "Permanently decrease grenade cooldown:  " + grenadeCost.ToString();
        }
        if (isGrenade && !grenadeUnlocked)
        {
            damage.text = "LOCKED";
        }
        if (forcefieldLevel > 6 && isForcefield)
        {

            damage.text = "Permanently decrease forcefield cooldown: MAX LVL";
            return;
        }
        if (isForcefield && forcefieldUnlocked)
        {

            forcefieldCost = permanent.GetForcefieldCost();
            damage.text = "Permanently decrease forcefield cooldown:  " + forcefieldCost.ToString();
        }
        if (isForcefield && !forcefieldUnlocked)
        {
            damage.text = "LOCKED" + forcefieldCost.ToString();
        }

    }

    public void Upgrade()
    {
        count = permanent.GetCount();
        grenadeLevel = permanent.GetGrenadeLevel();
        forcefieldLevel = permanent.GetForcefieldLevel();
        
        if(isDamage)
        {
            permanent.DamageIncrease();
            damageCost = permanent.GetDamageCost();
            damage.text = "Permanently increase damage by 10%\n Cost: " + damageCost.ToString();
        }
        if (count > 4 && isHealth)
        {
            permanent.HealthIncrease();
            damage.text = "Permanently increase health by 20hp\n MAX LVL";
            return;
        }
        if(isHealth)
        {
            permanent.HealthIncrease();
            healthCost = permanent.GetHealthCost();
            damage.text = "Permanently increase health by 20hp\n Cost: " + healthCost.ToString();
        }
        if (grenadeLevel > 6 && isGrenade)
        {
            permanent.GrenadeUpgrade();
            damage.text = "Permanently decrease grenade cooldown: MAX LVL";
            return;
        }
        if (isGrenade && grenadeUnlocked)
        {
            permanent.GrenadeUpgrade();
            grenadeCost = permanent.GetGrenadeCost();
            damage.text = "Permanently decrease grenade cooldown:  " + grenadeCost.ToString();
        }
        if (isGrenade && !grenadeUnlocked)
        {
            damage.text = "LOCKED";
        }
        if (forcefieldLevel > 6 && isForcefield)
        {
            permanent.ForcefieldUpgrade();
            damage.text = "Permanently decrease forcefield cooldown: MAX LVL";
            return;
        }
        if (isForcefield && forcefieldUnlocked)
        {
            permanent.ForcefieldUpgrade();
            forcefieldCost = permanent.GetForcefieldCost();
            damage.text = "Permanently decrease forcefield cooldown:  " + forcefieldCost.ToString();
        }
        if(isForcefield && !forcefieldUnlocked)
        {
            damage.text = "LOCKED" + forcefieldCost.ToString();
        }
    }
}
