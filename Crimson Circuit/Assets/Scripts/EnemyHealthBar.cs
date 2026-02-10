using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Health health;
    private float enemyHealth;
    private float maxHealth;
    private float healthPercentage;

    public Image healthBar;
    public GameObject mainCam;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        maxHealth = health.getEnemyHealth();
        Debug.Log(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - mainCam.transform.position);
        enemyHealth = health.getEnemyHealth();
        healthPercentage = enemyHealth/maxHealth;
        float fill = Mathf.Clamp01(healthPercentage);
        healthBar.fillAmount = fill;
    }
}
