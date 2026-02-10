using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public Transform Transform;

    public NavMeshSurface surface;

    public LevelActivator levelActivator;

    public Bullet Zap;
    public Bullet Big;
    public Bullet Bullet;

    public Score level;

    public Forcefield forcefield;
    public RuneAbillity rune;



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Zap.InitialDamage(20f);
        Big.InitialDamage(20f);
        Bullet.InitialDamage(5f);
    }

    public GameObject[] roomPrefabs;
    public GameObject bossRoomPrefab; // <-- assign in Inspector
    public float spawnDistance = 50f;
    public int maxActiveRooms = 3;

    private List<GameObject> activeRooms = new List<GameObject>();
    private int roomsPassed = 0;
    public int RoomsPassed => roomsPassed;
    public float damage = 1f;
    private int count = 0;
    private int whichBoss = -1;

    public bool allowNewEnemies = false;

    public void TrySpawnRoom(Transform spawnerTransform)
    {
        Vector3 roomPos = spawnerTransform.position + spawnerTransform.forward * spawnDistance;
        roomPos.y = 0;

        GameObject room;

        if (roomsPassed >= 9)
        {
            // Spawn boss room
            room = Instantiate(bossRoomPrefab, roomPos, spawnerTransform.rotation, Transform);
            levelActivator = GameObject.FindObjectOfType<LevelActivator>();
            roomsPassed = 0; // Reset after boss room
            damage += 0.1f;
            levelActivator.SetDifficulty(damage);
            level.LevelUpdate(1);
            count++;
            if (count == 2)
            {
                forcefield.unlock();
            }
            if(count == 3)
            {
                rune.unlock();
            }
            allowNewEnemies = true;
            whichBoss++;
            if (whichBoss == 3)
            {
                whichBoss = 0;
            }
        }
        else
        {
            // Spawn a random normal room
            int index = Random.Range(0, roomPrefabs.Length);
            room = Instantiate(roomPrefabs[index], roomPos, spawnerTransform.rotation, Transform);
            levelActivator = GameObject.FindObjectOfType<LevelActivator>();
            roomsPassed++;
            levelActivator.SetDifficulty(damage);
            level.LevelUpdate(1);
        }

        activeRooms.Add(room);

        // Remove oldest room if over limit
        if (activeRooms.Count > maxActiveRooms)
        {
            Destroy(activeRooms[0]);
            activeRooms.RemoveAt(0);
        }


    }

    public bool fetchEnemyStatus()
    {
        return allowNewEnemies;
    }

    public int fetchBossStatus()
    {
        return whichBoss;
    }

    public void RegenerateNavMesh()
    {
        Debug.Log("Rebuilding NavMesh...");
        surface.BuildNavMesh();
    }
}