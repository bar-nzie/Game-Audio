using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeAbility : MonoBehaviour
{
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    private float throwCooldown = 15f;

    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;
    private PlayerControls controls;
    bool combatMode = false;
    bool isUnlocked = false;

    public GameObject Logo;

    public Image throwCooldownImage;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Ability1.performed += ctx => Throw();
        controls.Player.CombatMode.performed += ctx => OnCombat();
    }

    // Start is called before the first frame update
    void Start()
    {
        readyToThrow = true;
        throwCooldownImage.fillAmount = 0f;
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

    public bool grenadeUnlocked() { return isUnlocked; }

    public void SetCooldown(float value)
    {
        throwCooldown = value;
    }

    public float GetCooldown()
    {
        return throwCooldown;
    }

    public void unlockCheck(bool check)
    {
        isUnlocked = check;
    }

    public bool GetUnlockedStatus() { return isUnlocked; }

    private void Throw()
    {
        if (!readyToThrow || !combatMode || !isUnlocked)
        {
            return;
        }
        Debug.Log("throw");
        readyToThrow = false;

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        StartCoroutine(HandleCooldown());
    }

    private IEnumerator HandleCooldown()
    {
        float elapsed = 0f;
        throwCooldownImage.fillAmount = 1f;

        while (elapsed < throwCooldown)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(1f - (elapsed / throwCooldown));
            throwCooldownImage.fillAmount = fill;
            yield return null;
        }

        throwCooldownImage.fillAmount = 0f;
        ResetThrow();
    }

    private void ResetThrow()
    {
        readyToThrow=true;
    }

    private void OnCombat()
    {
        combatMode = !combatMode;
    }
}
