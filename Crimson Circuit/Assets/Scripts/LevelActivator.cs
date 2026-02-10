using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class LevelActivator : MonoBehaviour
{
    public GameObject EntryCollider;
    public GameObject ExitCollider;
    public GameObject Gate;
    public GameObject[] spawners;
    public GameObject[] enemies;

    public GameObject baseEnemy;
    public GameObject flyingEnemy;
    public GameObject lightningEnemy;

    public Bullet Zap;
    public Bullet Big;
    public Bullet Bullet;

    private EnemyBehaviour enemyBehaviour;
    private FlyingEnemies FlyingEnemies;
    private LightningBehaviour lightningBehaviour;

    public RoomManager regen;

    public bool levelCompletion = false;
    public float openSpeed = 1f;
    private bool hasFinishedSpawning = false;
    private bool allowExtraEnemies;
    private int whichBoss;

    public GameObject extraEnemy1;     
    public GameObject extraEnemy2;      


    public float difficultyScaleFactor = 1f; // 1 = normal, >1 = harder, <1 = easier

    private bool hasSpawned = false; // prevent multiple spawns
    private List<GameObject> activeEnemies = new List<GameObject>();

    public bool isBoss;
    public GameObject bossPrefab;
    public GameObject boss2Prefab;
    public GameObject boss3Prefab;
    public GameObject bossSpawner;
    private GameObject bossInstance;

    private Temporaryupgrades upgrade;
    public GameObject UI;

    public Score score;
    public GrenadeAbility grenadeAbility;
    public GameInfoManager gameInfoManager;

    public AudioSource audioSource;

    private void Start()
    {
        upgrade = FindAnyObjectByType<Temporaryupgrades>();
        grenadeAbility = FindAnyObjectByType<GrenadeAbility>();
        regen = FindAnyObjectByType<RoomManager>();
        gameInfoManager = FindAnyObjectByType<GameInfoManager>();
        score = FindAnyObjectByType<Score>();
        UI = GameObject.Find("UI");
        whichBoss = regen.fetchBossStatus();
    }

    private void Update()
    {
        Vector3 pos = Gate.transform.position;
        allowExtraEnemies = regen.allowNewEnemies;
        // Check if all enemies are dead and mark level as complete
        if (!isBoss && !levelCompletion && hasSpawned && hasFinishedSpawning && AllEnemiesCleared())
        {
            levelCompletion = true;
            score.CoinsUpdate(10);
            upgrade.UpgradeTime();
            gameInfoManager.Saving();
            UI.SetActive(false);
        }

        // Animate gate opening
        if (levelCompletion)
        {
            audioSource.Stop();
            ExitCollider.SetActive(false);
            if (pos.y < 20f)
            {
                pos.y += openSpeed * Time.deltaTime;
                //Debug.Log(pos.y.ToString());
                Gate.transform.position = pos;
                
            }
        }
        if ( pos.y >= 20f)
        {
            gameObject.SetActive(false);
        }

        // Debug key to force level completion
        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    levelCompletion = true;
        //}

        if (isBoss && hasSpawned && bossInstance == null && !levelCompletion && hasFinishedSpawning)
        {
            audioSource.Stop();
            levelCompletion = true;
            score.CoinsUpdate(50);
            grenadeAbility.unlock();
            upgrade.UpgradeTime();
            gameInfoManager.Saving();
            UI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpawned)
        {
            EntryCollider.SetActive(true);

            if (!isBoss)
            {
                StartCoroutine(SpawnEnemiesWithDelay());
            }
            else
            {
                StartCoroutine(SpawnBossWithDelay());
            }

            hasSpawned = true;

            Zap.SetDamage(difficultyScaleFactor);
            Big.SetDamage(difficultyScaleFactor);
            Bullet.SetDamage(difficultyScaleFactor);
        }
    }

    private IEnumerator SpawnEnemiesWithDelay()
    {
        yield return new WaitForSeconds(5f); // Delay before spawning
        SpawnEnemies();
        hasFinishedSpawning = true;
    }

    private void SpawnEnemies()
    {
        List<GameObject> usedSpawners = new List<GameObject>();
        int guaranteedEnemies = 0;

        audioSource.Play();
        audioSource.volume = 0.5f;

        // Spawn at least 3 enemies at unique random spawners
        while (guaranteedEnemies < 3 && usedSpawners.Count < spawners.Length)
        {
            GameObject selectedSpawner = spawners[Random.Range(0, spawners.Length)];
            if (usedSpawners.Contains(selectedSpawner)) continue;

            usedSpawners.Add(selectedSpawner);
            SpawnEnemyAt(selectedSpawner);
            guaranteedEnemies++;
        }

        // Chance-based extra spawns based on difficulty
        foreach (GameObject spawner in spawners)
        {
            if (usedSpawners.Contains(spawner)) continue;

            float chance = 0.3f * difficultyScaleFactor;
            if (Random.value < chance)
            {
                SpawnEnemyAt(spawner);
            }
        }
    }

    private void SpawnEnemyAt(GameObject spawner)
    {
        // Build the list of available enemies based on the bool
        List<GameObject> availableEnemies = new List<GameObject>(enemies);
        if (allowExtraEnemies)
        {
            if (extraEnemy1 != null) availableEnemies.Add(extraEnemy1);
            if (extraEnemy2 != null) availableEnemies.Add(extraEnemy2);
        }

        // Randomly choose an enemy from the updated list
        GameObject enemy = availableEnemies[Random.Range(0, availableEnemies.Count)];

        // Ensure spawn position is valid on NavMesh
        if (NavMesh.SamplePosition(spawner.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            GameObject newEnemy = Instantiate(enemy, hit.position, Quaternion.Euler(0, Random.Range(0f, 360f), 0));

            if (enemy == baseEnemy)
            {
                enemyBehaviour = newEnemy.GetComponent<EnemyBehaviour>();
                enemyBehaviour.ApplyBuff(difficultyScaleFactor);
            }
            else if (enemy == flyingEnemy)
            {
                FlyingEnemies = newEnemy.GetComponent<FlyingEnemies>();
                FlyingEnemies.ApplyBuff(difficultyScaleFactor);
            }
            else if (enemy == lightningEnemy)
            {
                lightningBehaviour = newEnemy.GetComponent<LightningBehaviour>();
                lightningBehaviour.ApplyBuff(difficultyScaleFactor);
            }
            // You can add additional checks here for extraEnemy1 and extraEnemy2 if needed

            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogWarning($"Spawner at {spawner.transform.position} is not on the NavMesh!");
        }
    }


    private bool AllEnemiesCleared()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count == 0;
    }

    public void SetDifficulty(float difficulty)
    {
        difficultyScaleFactor = difficulty;
    }

    private IEnumerator SpawnBossWithDelay()
    {
        yield return new WaitForSeconds(5f); // Wait 5 seconds
        SpawnBoss();
        hasFinishedSpawning = true; // mark as finished so gate will open on boss death
    }


    private void SpawnBoss()
    {
        audioSource.Play();
        audioSource.volume = 0.5f;
        if (bossPrefab != null && bossSpawner != null)
        {
            if (whichBoss == 0)
            {
                bossInstance = Instantiate(bossPrefab, bossSpawner.transform.position, Quaternion.identity);
            }
            if (whichBoss == 1)
            {
                bossInstance = Instantiate(boss2Prefab, bossSpawner.transform.position, Quaternion.identity);
            }
            if (whichBoss == 2)
            {
                bossInstance = Instantiate(boss3Prefab, bossSpawner.transform.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("Boss prefab or spawner not assigned.");
        }
    }
}
