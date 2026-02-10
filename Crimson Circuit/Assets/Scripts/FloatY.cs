using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatY : MonoBehaviour
{
    private GameObject targetObject; // Reference to the object you want to float
    public float minY = 0.4f;
    public float maxY = 0.6f;
    public float speed = 1f; // Speed of the floating motion

    private float originalY;
    private float amplitude;
    private float midY;

    private void Start()
    {
        if (targetObject == null)
        {
            targetObject = this.gameObject; // Default to the object this script is attached to
        }

        originalY = targetObject.transform.position.y;
        midY = (minY + maxY) / 2f;
        amplitude = (maxY - minY) / 2f;
    }

    private void Update()
    {
        float newY = midY + Mathf.Sin(Time.time * speed) * amplitude;
        Vector3 position = targetObject.transform.position;
        position.y = newY;
        targetObject.transform.position = position;
    }
}
