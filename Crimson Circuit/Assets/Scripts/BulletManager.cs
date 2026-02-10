using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class BulletManager : MonoBehaviour
{
    //Combat Mode
    bool combatMode = false;

    //New Input System
    private PlayerControls controls;
    InputAction shootAction;
    InputAction reloadAction;
    InputAction CombatMode;

    //bullet
    public GameObject bulletPrefab;
    public AudioSource audioSource;
    public AudioClip shootSound;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShoting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //References
    public Camera playerCam;
    public Transform bulletSpawnPoint;

    //Graphics
    public GameObject bulletTrailPrefab;
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //bug fixing
    public bool allowInvoke = true;

    private void Awake()
    {
        //set bullets left
        controls = new PlayerControls();
        bulletsLeft = magazineSize;
        readyToShoot = true;
        shootAction = controls.Player.Fire;
        reloadAction = controls.Player.Reload;
        CombatMode = controls.Player.CombatMode;
    }

     private void OnEnable()
    {
        controls.Enable();
        shootAction.Enable();
        reloadAction.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
        shootAction.Disable();
        reloadAction.Disable();
    }

    private void Update()
    {
        if (CombatMode.triggered)
        {
            combatMode = !combatMode;
            Debug.Log("Combat Mode: " + combatMode);
        }

        if (combatMode)
        {
            Input();
        }

        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.text = bulletsLeft + " / " + magazineSize;
        }
    }

    private void Input()
    {
        if (allowButtonHold) shooting = shootAction.IsPressed();
        else shooting = shootAction.WasPressedThisFrame();

        //Reloading
        if (reloadAction.WasPressedThisFrame() && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }
        //Autpmatically reload if out of bullets
        if (bulletsLeft <= 0 && !reloading && shooting)
        {
            Reload();
        }

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        audioSource.PlayOneShot(shootSound);

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector3 targetPoint;
        targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - bulletSpawnPoint.position;

        GameObject currentBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithoutSpread.normalized;
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCam.transform.up * upwardForce, ForceMode.Impulse);

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShoting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        if (reloading) return;

        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
