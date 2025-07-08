using UnityEngine;
using UnityEngine.UI;

public class FPSController : MonoBehaviour
{
    private Camera playerCamera;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float runSpeed = 10f;
    private bool canDoubleJump = false;
    [SerializeField] float jumpForce = 1.5f;
    [SerializeField] float gravity = 6f;

    public float mouseSensitivity = 2f;
    float keyRotationSpeed = 100f;
    [SerializeField] float lookXLimit = 70f;

    float rotationX = 0f;
    Vector3 moveDirection = Vector3.zero;
    CharacterController characterController;

    public float stamina = 0.5f;
    public float stillRegenRate = 0.1f;
    public float walkRegenRate = 0.05f;
    public float drainRate = 0.3f;
    public Slider staminaBar;

    void Start()
    {
        playerCamera = Camera.main;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Horizontal rotation via Q and E keys
        float rotationInput = Input.GetAxis("KeyRotate");
        transform.Rotate(Vector3.up, rotationInput * keyRotationSpeed * Time.deltaTime);

        // Horizontal rotation based on mouse X movement
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);

        // Vertical rotation based on mouse Y movement
        rotationX += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.Rotate(Vector3.up, rotationInput * keyRotationSpeed * Time.deltaTime);

        // Control Movement and Jumping
        bool isIdle = false;
        if (characterController.isGrounded)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            // Captured for later use
            isIdle = verticalInput == 0;

            float moveDirectionY = moveDirection.y;
            moveDirection = (horizontalInput * transform.right) + (verticalInput * transform.forward);

            canDoubleJump = true;

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
            else
            {
                moveDirection.y = moveDirectionY;
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && canDoubleJump)
            {
                moveDirection.y = jumpForce;
                canDoubleJump = false;
            }

            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Handle running and stamina
        float stamina = staminaBar.value;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && stamina > 0;
        float moveSpeed = isRunning ? runSpeed : walkSpeed;

        characterController.Move(moveSpeed * Time.deltaTime * moveDirection);

        if (!isIdle && isRunning) stamina -= drainRate * Time.deltaTime;
        if (!isIdle && !isRunning) stamina += walkRegenRate * Time.deltaTime;
        if (isIdle) stamina += stillRegenRate * Time.deltaTime;

        staminaBar.value = Mathf.Clamp(stamina, 0, staminaBar.maxValue);
    }
}
