using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPointRef : MonoBehaviour
{
    public Transform player;
    public float yaxis;
    private Vector3 transformation;

    private void Update()
    {
        if (player == null) return;
        transformation = new Vector3(player.position.x, player.position.y + yaxis, player.position.z);
        transform.position = transformation;
    }
}
