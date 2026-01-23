using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 0.2f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;

    [Header("Other")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody rigidBody;

    private InputAction moveAction;
    private InputAction lookAction;

    private float rotationX;
    private float rotationY;

    void Awake()
    {
        rigidBody.freezeRotation = true;

        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        lookAction = new InputAction("Look", InputActionType.Value);
        lookAction.AddBinding("<Mouse>/delta");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    void Update()
    {
        HandleCamera();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * input.y + right * input.x;

        rigidBody.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rigidBody.linearVelocity.y,
            moveDirection.z * moveSpeed
        );
    }

    private void HandleCamera()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        rotationY += lookInput.x * mouseSensitivity;
        rotationX -= lookInput.y * mouseSensitivity;

        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        cameraTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        bodyTransform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}