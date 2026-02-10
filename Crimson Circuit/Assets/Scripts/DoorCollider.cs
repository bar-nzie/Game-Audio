using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    public GameObject LeftDoor;
    public GameObject RightDoor;

    public float rotationDuration = 1.0f; // Duration of the rotation in seconds
    private bool isOpening = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpening)
        {
            StartCoroutine(RotateDoor(LeftDoor.transform, Vector3.up, -90f));
            StartCoroutine(RotateDoor(RightDoor.transform, Vector3.up, 90f));
            isOpening = true;
        }
    }

    private System.Collections.IEnumerator RotateDoor(Transform door, Vector3 axis, float angle)
    {
        Quaternion startRotation = door.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(axis * angle);
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            door.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.rotation = endRotation;
    }
}
