using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    private CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;
    private Vector3 verticalVelocity;
    private Vector2 moveInput;

    [Header("Look")]
    public float lookSensitivity = 100.0f; 
    private float xRotation = 0f;
    private Vector2 lookInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleCursor();
    }

    void HandleMovement()
    {
        float currentSpeed = moveSpeed;
        if(SkillManager.instance!=null && SkillManager.instance.isFasterMovementUnlocked)
        {
            currentSpeed *= 1.25f;
        }
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        //Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCursor()
    {
        if (GameManager.instance.IsUiMenuOpen || GameManager.instance.currentState == GameManager.GameState.Paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnMove(InputValue value)
    {
        if (GameManager.instance.currentState == GameManager.GameState.Playing && !GameManager.instance.IsUiMenuOpen)
        {
            moveInput = value.Get<Vector2>();
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    public void OnLook(InputValue value)
    {
        if (GameManager.instance.currentState == GameManager.GameState.Playing && !GameManager.instance.IsUiMenuOpen)
        {
            lookInput = value.Get<Vector2>();
        }
        else
        {
            lookInput = Vector2.zero; 
        }
    }
}