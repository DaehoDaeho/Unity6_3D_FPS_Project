using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private float moveSpeed = 5.0f;

    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float minVerticalAngle = -80.0f;
    [SerializeField] private float maxVerticalAngle = 80.0f;

    [SerializeField] private float gravity = -20.0f;
    [SerializeField] private float jumpHeight = 1.5f;

    private float verticalRotation;
    private float verticalVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        CheckRequiredReferences();

        ConfigureCursor();
    }

    void ConfigureCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        LookAround();
    }

    void CheckRequiredReferences()
    {
        if(characterController == null)
        {
            Debug.LogWarning("CharacterController가 연결되지 않았습니다.", this);
        }

        if (cameraPivot == null)
        {
            Debug.LogWarning("CameraPivot이 연결되지 않았습니다.", this);
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerCamera가 연결되지 않았습니다.", this);
        }
    }

    void MovePlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.forward * verticalInput) + (transform.right * horizontalInput);

        moveDirection.Normalize();

        Vector3 horizontalMovement = moveDirection * moveSpeed;

        if(characterController.isGrounded == true && verticalVelocity < 0.0f)
        {
            verticalVelocity = -2.0f;
        }

        if(characterController.isGrounded == true && Input.GetKeyDown(KeyCode.Space) == true)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 verticalMovement = Vector3.up * verticalVelocity;

        Vector3 movement = (horizontalMovement + verticalMovement) * Time.deltaTime;

        characterController.Move(movement);
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        if(cameraPivot == null)
        {
            return;
        }

        cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0.0f, 0.0f);
    }
}
