using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private Rigidbody rb;
    public Transform orientation;

    private float moveSpeed = 10f;
    public float jumpForce = 10f;
    private Vector3 moveDirection;
    public float playerHeight;
    public float groundDrag;
    RaycastHit slopeHit;
    RaycastHit groundHit;

    public float airControlMultiplier = 0.5f;
    public float fallMultiplier = 2.5f;

    float dashDistance = 10f;
    float dashStopBuffer = 0.2f;
    private bool isDashing = false;
    public float dashCooldown = 2f;
    private bool dashReady = true;

    public Animator animator;
    private bool CombatMode = false;

    public LayerMask whatIsGround;
    public bool grounded;

    public Transform combatObject; 
    public Vector3 raisedOffset = new Vector3(0f, 0.5f, 0f); 
    private Vector3 originalCombatObjectPos;

    public Image dashCooldownImage;
    private float sensitivity = 1f;

    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip shootSound2;

    public float sense;
    public CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook freeLookCam2;
    public GameObject PauseMenu;
    public bool paused = false;
    public GameObject resumeButton;
    public GameObject optionsMenu;

    public GameObject dashEffect;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        controls.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Movement.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => OnJump();
        controls.Player.Dash.performed += ctx => OnDash();
        controls.Player.Pause.performed += ctx => OnPause();
        controls.Player.CombatMode.performed += ctx => OnCombat();
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        originalCombatObjectPos = combatObject.localPosition;
        dashCooldownImage.fillAmount = 0f;
        float value = PlayerPrefs.GetFloat("CamSensitivity", 2.0f);
        UpdateSense(value);
    }


    void FixedUpdate()
    {
        float value = PlayerPrefs.GetFloat("CamSensitivity", 2.0f);
        UpdateSense(value);
        animator.SetBool("IsJumping", false);
        if (isDashing) return;

        //Convert 2D input (x, y) to 3D (x, z)
        Vector3 movement = new Vector3(moveInput.x, 0.0f, moveInput.y);

        //Move the rigidbody
        float multiplier = grounded ? 1f : airControlMultiplier;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * multiplier, ForceMode.Force);

        SpeedControl();

        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (OnSlope())
        {
            // Project the movement onto the slope
            moveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
        }

        if (moveDirection == Vector3.zero)
        {
            animator.SetBool("IsWalking", false);
        }
        else
        {
            animator.SetBool("IsWalking", true);
        }

        if(rb.velocity.y < 0f)
        {
            animator.SetBool("IsFalling", true);
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }

        grounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out groundHit, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (!grounded && !isDashing && rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }

        if (grounded && rb.velocity.y < 0f)
        {
            animator.SetBool("IsLanding", true);
        }
        else
        {
            animator.SetBool("IsLanding", false);
        }

        if(CombatMode == true)
        {
            animator.SetBool("IsCombat", true);
            combatObject.localPosition = originalCombatObjectPos + raisedOffset;
        }
        else
        {
            animator.SetBool("IsCombat", false);
            combatObject.localPosition = originalCombatObjectPos;
        }

        animator.SetFloat("Horizintal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    public void SpeedUpgrade(float speed)
    {
        moveSpeed = speed;
    }

    public void CooldownUpgrade(float time)
    {
        dashCooldown = time;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
        }
    }

    private void OnJump()
    {
        if (grounded && CombatMode == false)
        {
            audioSource.PlayOneShot(shootSound);
            animator.SetBool("IsJumping", true);
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDash()
    {
        if (!isDashing && dashReady && CombatMode == false)
        {
            animator.SetBool("IsDashing", true);
            audioSource.PlayOneShot(shootSound2);
            StartCoroutine(DashCoroutine());
            StartCoroutine(DashCooldownCoroutine()); 
        }
        //rb.drag = 0f;
        //Debug.Log("Dash");
        //rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

        //Vector3 dashDir = transform.forward;

        //// If grounded and on a slope, project onto slope
        //if (OnSlope())
        //{
        //    dashDir = Vector3.ProjectOnPlane(dashDir, slopeHit.normal).normalized;
        //}

        //Vector3 targetPos = transform.position + dashDir * dashDistance;

        //// Raycast to stop before hitting something
        //if (Physics.Raycast(transform.position, dashDir, out RaycastHit hit, dashDistance))
        //{
        //    targetPos = hit.point - dashDir * dashStopBuffer; // Stop slightly before hit
        //}

        //rb.AddForce(transform.forward * 100f * 5f, ForceMode.Impulse);
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;

        dashEffect.SetActive(true);

        rb.drag = 0f;
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f); // Reset horizontal velocity

        Vector3 dashDir = transform.forward;
        if (OnSlope())
        {
            dashDir = Vector3.ProjectOnPlane(dashDir, slopeHit.normal).normalized;
        }

        Vector3 targetPos = transform.position + dashDir * dashDistance;

        // Raycast to stop dash before hitting obstacle
        if (Physics.Raycast(transform.position, dashDir, out RaycastHit hit, dashDistance))
        {
            targetPos = hit.point - dashDir * dashStopBuffer; // Stop slightly before obstacle
        }

        float dashSpeed = 50f; // How fast to move during dash
        float distance = Vector3.Distance(transform.position, targetPos);
        float dashDuration = distance / dashSpeed;

        float elapsed = 0f;
        Vector3 startPos = transform.position;

        // Disable gravity during dash for better control
        rb.useGravity = false;

        while (elapsed < dashDuration)
        {
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, elapsed / dashDuration);
            rb.MovePosition(newPos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);

        rb.useGravity = true;
        isDashing = false;
        animator.SetBool("IsDashing", false);
    }

    private IEnumerator DashCooldownCoroutine()
    {
        dashReady = false;
        float elapsed = 0f;

        while (elapsed < dashCooldown)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(1f - (elapsed / dashCooldown));
            dashCooldownImage.fillAmount = fill;
            yield return null;
        }
        dashEffect.SetActive(false);
        dashCooldownImage.fillAmount = 0f;
        dashReady = true;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            return slopeHit.normal != Vector3.up;
        }
        return false;
    }

    private void OnCombat()
    {
        CombatMode = !CombatMode;
    }

    public void UpdateSense(float value)
    {
        sense = value;
        freeLookCam.m_XAxis.m_MaxSpeed = sense * 100f;
        freeLookCam.m_YAxis.m_MaxSpeed = sense * 0.5f; 
        freeLookCam2.m_XAxis.m_MaxSpeed = sense * 100f;
        freeLookCam2.m_YAxis.m_MaxSpeed = sense * 0.5f;
    }

    public void SetPause()
    {
        paused = true;
        OnPause();
    }

    public void OnPause()
    {
        if (!paused)
        {
            paused = true;
            PauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            EventSystem.current.SetSelectedGameObject(null); // Clear current selection
            EventSystem.current.SetSelectedGameObject(resumeButton);
            return;
        }
        if (paused)
        {
            Debug.Log("Resume");
            Time.timeScale = 1f;
            paused = false;
            PauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }
    }
}
