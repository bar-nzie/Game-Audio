using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    private Vector3 SpawnPosition = new Vector3(0f, 0f, 0f);
    private float MovementAcross = 150f;
    public GameObject StartRoomPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnPosition = new Vector3(transform.position.x + MovementAcross, 0f, 0f );
            Vector3 PlayerPosition = new Vector3(transform.position.x + MovementAcross, 4f, 0f);

            other.transform.position = PlayerPosition;
            Instantiate(StartRoomPrefab, SpawnPosition, Quaternion.identity);

        }
    }
}
