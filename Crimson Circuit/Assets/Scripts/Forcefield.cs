using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Forcefield : MonoBehaviour
{
    public GameObject forcefield;
    private PlayerControls controls;
    bool combatMode = false;
    public bool isUnlocked = false;
    public GameObject Logo;
    private float Cooldown = 30f;
    public Image LogoImage;
    private bool readyToThrow;
    private bool forcefieldActivated;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Ability2.performed += ctx => activation();
        controls.Player.CombatMode.performed += ctx => OnCombat();
    }
    // Start is called before the first frame update
    void Start()
    {
        readyToThrow = true;
        LogoImage.fillAmount = 0f;
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Logo.SetActive(isUnlocked);
    }

    public void unlock()
    {
        isUnlocked = true;
    }

    public bool forcefieldUnlocked() { return isUnlocked; }

    public void SetCooldown(float value)
    {
        if(value == 0f)
        {
            return;
        }
        Cooldown = value;
    }

    public float GetCooldown()
    {
        return Cooldown;
    }

    public void unlockCheck(bool check)
    {
        isUnlocked = check;
    }

    public bool GetUnlockedStatus() { return isUnlocked; }

    public bool IsForcefieldActive() { return forcefieldActivated; }

    private void activation()
    {
        if (!readyToThrow || !combatMode || !isUnlocked)
        {
            return;
        }
        Debug.Log(Cooldown);
        readyToThrow = false;
        forcefieldActivated = true;
        forcefield.SetActive(true);

        StartCoroutine(Duration());
    }

    private IEnumerator Duration()
    {
        LogoImage.fillAmount = 1f;
        yield return new WaitForSeconds(5f);

        forcefield.SetActive(false);
        StartCoroutine(HandleCooldown());
    }

    private IEnumerator HandleCooldown()
    {
        forcefieldActivated = false;
        float elapsed = 0f;
        LogoImage.fillAmount = 1f;

        while (elapsed < Cooldown)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(1f - (elapsed / Cooldown));
            LogoImage.fillAmount = fill;
            yield return null;
        }

        LogoImage.fillAmount = 0f;
        ResetThrow();
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    private void OnCombat()
    {
        combatMode = !combatMode;
    }
}
