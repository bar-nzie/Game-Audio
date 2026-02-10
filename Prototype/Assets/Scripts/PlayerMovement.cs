using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Playermovement : MonoBehaviour
{
    [Header("Movement")]
    public float gravity = -9.8f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraHolder;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Sprint")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float stamina = 5f;
    public float maxStamina = 5f;
    public float staminaDrain = 1f;
    public float staminaRegen = 0.8f;
    public Image staminaFill;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Attack")]
    public Transform aimPivot;
    public GameObject bulletPrefab;
    public Camera playerCam;

    private bool isSprinting;

    private bool isGrounded;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    bool isBenching;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentHealth = maxHealth;
    }

    public void IsBenching(bool benching)
    {
        isBenching = benching;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBenching) return;
        cameraHolder.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 1);
        Move();
        MouseLook();
        UpdateUI();
        if (currentHealth <= 0f)
        {
            Destroy(this.gameObject);
        }
        if (inputActions.Player.Attack.WasPressedThisFrame())
        {
            //Shoot();
        }
    }

    void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        float currentSpeed = walkSpeed;

        if (inputActions.Player.Sprint.IsPressed() && stamina > 0f && input.y > 0)
        {
            currentSpeed = sprintSpeed;
            stamina -= staminaDrain * Time.deltaTime;
        }
        else if (stamina < maxStamina)
        {
            stamina += staminaRegen * Time.deltaTime;
        }

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (inputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void MouseLook()
    {
        Vector2 mouseDelta = inputActions.Player.Look.ReadValue<Vector2>();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    void UpdateUI()
    {
        //astaminaFill.fillAmount = stamina / maxStamina;
    }

    private void Shoot()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector3 targetPoint;
        targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - aimPivot.position;

        GameObject currentBullet = Instantiate(bulletPrefab, aimPivot.position, Quaternion.LookRotation(directionWithoutSpread));
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
    }
}
