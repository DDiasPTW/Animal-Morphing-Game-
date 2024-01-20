using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Def : MonoBehaviour
{
    [Header("Other")]
    private Player playerControls;
    private Rigidbody rb;
    private Transform cameraTransform;
    [Header("Movement")]
    public float moveSpeed = 6f;
    private Vector3 movement;
    [Header("Jumping")]
    public float jumpForce = 10.0f;
    public float gravityScale = 1.0f;
    public static float globalGravity = -9.81f;
    private float coyote_timer;
    [SerializeField] private float coyote_seconds = 0.1f; //coyote timer to allow the player to jump briefly after leaving a platform
    private float JumpBuffer_Timer;
    [SerializeField] private float JumpBuffer_Seconds = 0.1f; //jump buffer to allow the player to jump immediately after hitting the ground if they failed the timing while falling
    [SerializeField] private bool isGrounded; // To check if player is on the ground
    [SerializeField] private bool jumpRequested; // To store jump request
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool justLanded = false;

    [Header("Animation")]
    private Animator anim;
    private string moveAnim = "player_move";
    private string idleAnim = "player_idle";
    private string jumpUpAnim = "player_jump_up";
    private string landingAnim = "player_jump_land";

    private void Awake()
    {
        playerControls = new Player();

        // Subscribe to input action events
        playerControls.Gameplay.Movement.performed += ctx => movement = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Movement.canceled += ctx => movement = Vector2.zero;
        playerControls.Gameplay.Jump.performed += ctx => Jump();
        playerControls.Gameplay.Interact.performed += ctx => Merge();
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.useGravity = false;
        cameraTransform = Camera.main.transform; // Get the main camera's transform
        isGrounded = false;
        jumpRequested = false;
    }

    void FixedUpdate()
    {
        Move();

        ApplyGravity();
    }

    void Update()
    {
        CheckJump();
        HandleAnimations();
    }

    #region Movement
    private void Move()
    {
        // Calculate the movement direction relative to the camera's orientation
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; // Ensure that movement is only on the x-z plane
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveVector = (forward * movement.y + right * movement.x).normalized; // Direction of movement

        if (moveVector != Vector3.zero)
        {
            // Rotate the GameObject to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
        }

        // Calculate the velocity
        Vector3 velocity = moveVector * moveSpeed;
        // Apply the velocity to the Rigidbody
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // Maintain existing Y-axis velocity

    }

    private void ApplyGravity()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void CheckJump()
    {
        if (!isGrounded)
        {
            coyote_timer -= Time.deltaTime;
        }
        if (jumpRequested && JumpBuffer_Timer < 0)
        {
            jumpRequested = false;
        }
        if (jumpRequested && (isGrounded || coyote_timer > 0))
        {
            PerformJump();
            jumpRequested = false;
        }
        if (JumpBuffer_Timer > 0 && isGrounded)
        {
            PerformJump();
        }

        JumpBuffer_Timer -= Time.deltaTime;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            PerformJump();
        }
        else
        {
            jumpRequested = true;
            JumpBuffer_Timer = JumpBuffer_Seconds;
        }
    }

    private void PerformJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        isGrounded = false;
        isJumping = true;
    }



    //!MUDAR ESTE TIPO DE DETEÇÃO
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground")) // Assuming ground objects have "Ground" tag
        {
            isGrounded = true;
            isJumping = false;
            coyote_timer = coyote_seconds;
            if (!justLanded)
            {
                justLanded = true; // Player just landed
                Debug.Log("just landed");
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    #endregion

    #region Merge
    private void Merge()
    {
        // Add the logic to merge with animals, player must be in the "default" form to merge
        // When reaching an area where the player must merge, forcibly swap out its form to the default
    }
    #endregion

    #region Visuals
    private void HandleAnimations()
    {
        if (justLanded)
        {
            anim.Play(landingAnim);
        }
        else if (isJumping)
        {
            anim.Play(jumpUpAnim);
        }
        else if (movement != Vector3.zero && !isJumping)
        {
            anim.Play(moveAnim);
        }
        else
        {
            anim.Play(idleAnim);
        }
    }

    public void ResetLanding(){
        justLanded = false;
    }
    #endregion
}
