using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public Transform playerObj;
    public Transform orientation;
    public float rotationSpeed;
    private PlayerControls controls; //Input map
    private Vector2 moveInput;
    private InputAction CombatMode;

    public Transform combatLook;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public bool combat = false;

    public GameObject weapon;

    public CameraStyle currentStyle;

    public GameObject recticle;

    public enum CameraStyle
    {
        Basic,
        Combat
    }

    void Awake()
    {
        controls = new PlayerControls();
        CombatMode = controls.Player.CombatMode;
        recticle.SetActive(false); // Disable the recticle by default
    }

    private void OnEnable()
    {
        // Enable the controls and set up the callback for movement input
        controls.Enable();
        controls.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();  // When move is performed, store the value
        controls.Player.Movement.canceled += ctx => moveInput = Vector2.zero;  // When move is canceled, stop movement (set to zero)
    }

    private void OnDisable()
    {
        // Disable the controls when the object is disabled
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CombatMode.triggered && combat == false)
        {
            combat = true;
            SwitchCameraStyle(CameraStyle.Combat);
            //Debug.Log(currentStyle);
        }

        else if (CombatMode.triggered && combat == true)
        {
            combat = false;
            SwitchCameraStyle(CameraStyle.Basic);
            //Debug.Log(currentStyle);
        }
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        if (currentStyle == CameraStyle.Basic)
        {
            float horizontalInput = moveInput.x;
            float verticalInput = moveInput.y;
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        }

        else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 lookDir = combatLook.position - new Vector3(transform.position.x, combatLook.position.y, transform.position.z);
            orientation.forward = lookDir.normalized;

            playerObj.forward = lookDir.normalized;
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);

        if (newStyle == CameraStyle.Basic)
        {
            Debug.Log("Basic");
            recticle.SetActive(false); // Enable the recticle for basic camera style
            thirdPersonCam.SetActive(true);
            weapon.SetActive(false);
        }
        if (newStyle == CameraStyle.Combat)
        {
            Debug.Log("Combat");
            recticle.SetActive(true); // Enable the recticle for basic camera style
            combatCam.SetActive(true);
            weapon.SetActive(true);
        }

        currentStyle = newStyle;
    }
}