using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxLifetime = 5f; 
    private float lifeTimer;
    public float damage;
    private float playerDamage;
    public float initial;
    private Vector3 previousPosition;

    public GameObject thisBullet;
    public GameObject Zap;
    public GameObject Big;
    public GameObject Enemy;
    public GameObject Bull;
    public GameObject Shockwave;
    public GameObject Orb;

    public Temporaryupgrades damageUpgrade;
    public CinemachineImpulseSource impulseSource;

    void Start()
    {
        damageUpgrade = FindAnyObjectByType<Temporaryupgrades>();
        lifeTimer = maxLifetime;
        previousPosition = transform.position;
        playerDamage = damageUpgrade.GetDamage();
        if (playerDamage <= 0f) playerDamage = 20f;
        Debug.Log(playerDamage);
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }

        Vector3 movementVector = transform.position - previousPosition;
        float distance = movementVector.magnitude;

        if (Physics.Raycast(previousPosition, movementVector.normalized, out RaycastHit hit, distance))
        {
            if (!hit.collider.CompareTag("Bullet"))
            {
                if(thisBullet == Zap || thisBullet == Big || thisBullet == Enemy || thisBullet == Shockwave || thisBullet == Orb)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Health health = hit.collider.GetComponent<Health>();
                        if (health != null)
                        {
                            health.TakeDamage(damage);
                            impulseSource.GenerateImpulse(0.5f);
                        }
                    }
                    Destroy(gameObject);
                }
                else if (thisBullet == Bull)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        Health health = hit.collider.GetComponent<Health>();
                        if (health != null)
                        {
                            health.TakeDamage(playerDamage);
                        }
                    }
                    Destroy(gameObject);
                }
            }
        }
        Debug.DrawRay(previousPosition, movementVector * distance, Color.red, 1f);

        previousPosition = transform.position;
    }

    public void InitialDamage(float dmg)
    {
        damage = dmg;
        initial = dmg;
    }

    public void SetDamage(float dmg)
    {
        damage = initial * dmg;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
            if (other.CompareTag("Enemy"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(playerDamage);
            }
        }
        if (thisBullet != Shockwave)
        {
            Destroy(gameObject);
        }
    }
}
