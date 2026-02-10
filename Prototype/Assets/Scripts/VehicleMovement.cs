using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 120f;
    public float acceleration = 4f;
    public float deceleration = 6f;

    private float currentSpeed;

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");   // W/S
        float turnInput = Input.GetAxis("Horizontal"); // A/D

        // Smooth acceleration / deceleration
        if (Mathf.Abs(moveInput) > 0.1f)
            currentSpeed = Mathf.Lerp(currentSpeed, moveInput * moveSpeed, acceleration * Time.deltaTime);
        else
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime);

        // Move forward/back
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Turn (only if moving)
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turnDirection = Mathf.Sign(currentSpeed); // reverse steering when backing up
            transform.Rotate(Vector3.up * turnInput * turnSpeed * turnDirection * Time.deltaTime);
        }
    }
}
