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

    private float verticalRotation;

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

        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

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
