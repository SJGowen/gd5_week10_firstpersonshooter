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
    public Image fillImage;
    Color minColor = Color.red;
    Color maxColor = Color.green;

    // Store isIdle and isRunning as fields so they can be shared between methods
    bool isIdle = false;
    bool isRunning = false;

    void Start()
    {
        playerCamera = Camera.main;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleMovementAndJumping();
        HandleStaminaAndRunning();
    }

    void HandleRotation()
    {
        float rotationInput = Input.GetAxis("KeyRotate");
        transform.Rotate(Vector3.up, rotationInput * keyRotationSpeed * Time.deltaTime);

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);

        rotationX += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.Rotate(Vector3.up, rotationInput * keyRotationSpeed * Time.deltaTime);
    }

    void HandleMovementAndJumping()
    {
        if (characterController.isGrounded)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            // Make either right mouse button or middle mouse button make the player move forward
            if (verticalInput == 0 && (Input.GetMouseButton(1) || Input.GetMouseButton(2)))
            {
                verticalInput += 1;
            }

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
    }

    void HandleStaminaAndRunning()
    {
        float stamina = staminaBar.value;

        // Make Left Shift or middle mouse button make the player run
        isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(2)) && stamina > 0;
        float moveSpeed = isRunning ? runSpeed : walkSpeed;

        characterController.Move(moveSpeed * Time.deltaTime * moveDirection);

        if (!isIdle && isRunning) stamina -= drainRate * Time.deltaTime;
        if (!isIdle && !isRunning) stamina += walkRegenRate * Time.deltaTime;
        if (isIdle) stamina += stillRegenRate * Time.deltaTime;

        staminaBar.value = Mathf.Clamp(stamina, 0, staminaBar.maxValue);
        float valuePercent = staminaBar.value / staminaBar.maxValue;
        fillImage.color = Color.Lerp(Color.red, Color.green, valuePercent);
    }
}
