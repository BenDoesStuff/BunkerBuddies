using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    [Header("Head Bob Settings")]
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;

    [Header("Landing Bob Settings")]
    public float landingBobAmount = 0.1f;
    public float landingBobSpeed = 5f;
    public float jumpKickAmount = 4f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Footstep Settings")]
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;

    [Header("Landing FX")]
    public GameObject landingParticlesPrefab;
    public Transform landingFXSpawnPoint;

    private CharacterController controller;
    private AudioSource audioSource;
    private float verticalVelocity;
    private float verticalRotation = 0f;
    private bool isGrounded;
    private bool wasGroundedLastFrame;

    private Vector3 originalCamLocalPos;
    private float bobTimer;

    private float landingBobOffset = 0f;
    private float jumpKickOffset = 0f;
    private float jumpKickVelocity = 0f;

    private float stepTimer = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTransform != null)
            originalCamLocalPos = cameraTransform.localPosition;
        else
            Debug.LogWarning("Camera Transform not assigned.");
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleHeadBob();
        HandleFootsteps();

        wasGroundedLastFrame = isGrounded;
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        float finalPitch = verticalRotation + jumpKickOffset;
        cameraTransform.localRotation = Quaternion.Euler(finalPitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;

            if (!wasGroundedLastFrame)
            {
                landingBobOffset = -landingBobAmount;

                if (landingParticlesPrefab && landingFXSpawnPoint)
                {
                    GameObject fx = Instantiate(landingParticlesPrefab, landingFXSpawnPoint.position, Quaternion.identity);
                    Destroy(fx, 2f);
                }
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpKickOffset = jumpKickAmount;
            jumpKickVelocity = -jumpKickAmount * 8f;
        }

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void HandleHeadBob()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float bobFrequency = bobSpeed * (currentSpeed / walkSpeed);

        if (isGrounded && isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmount;
            Vector3 localPos = originalCamLocalPos;
            localPos.y += bobOffset + landingBobOffset;
            cameraTransform.localPosition = localPos;
        }
        else
        {
            ResetHeadBob();
        }

        jumpKickOffset = Mathf.SmoothDamp(jumpKickOffset, 0f, ref jumpKickVelocity, 0.2f);
    }

    private void ResetHeadBob()
    {
        landingBobOffset = Mathf.Lerp(landingBobOffset, 0f, Time.deltaTime * landingBobSpeed);
        jumpKickOffset = Mathf.SmoothDamp(jumpKickOffset, 0f, ref jumpKickVelocity, 0.2f);

        Vector3 localPos = originalCamLocalPos;
        localPos.y += landingBobOffset;

        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            localPos,
            Time.deltaTime * bobSpeed
        );
    }

    private void HandleFootsteps()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f;

        if (isGrounded && isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayFootstepAudio();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = stepInterval;
        }
    }

    private void PlayFootstepAudio()
    {
        if (footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        audioSource.PlayOneShot(footstepClips[index]);
    }
}
