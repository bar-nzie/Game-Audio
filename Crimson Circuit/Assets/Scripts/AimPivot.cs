using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimPivot : MonoBehaviour
{
    public Transform aimPivot; // This is your AimPivot GameObject
    public Transform cameraTransform; // The main camera (for aiming direction)
    public float rotationSpeed = 5f;

    void LateUpdate()
    {
        aimPivot.rotation = cameraTransform.rotation;
    }
}
