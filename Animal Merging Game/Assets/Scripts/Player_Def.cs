using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Def : MonoBehaviour
{
    [Header("Other")]
    public Player playerControls;
    public Rigidbody rb;
    private Transform cameraTransform;
    [SerializeField] private Vector3 groundCheckSize;
    [SerializeField] private Transform groundCheckPosition;
    public LayerMask groundLayer;
    [SerializeField] private Vector3 groundCheckOffset = Vector3.zero;


    [Header("Movement")]
    public float moveSpeed = 6f;
    [SerializeField] private float airControlFactor = 2f;
    private Vector3 movement;
    [Header("Jumping")]
    public float jumpForce = 10.0f;
    public float gravityScale = 1.0f;
    public static float globalGravity = -9.81f;
    private float coyote_timer;
    [SerializeField] private float coyote_seconds = 0.1f; //coyote timer to allow the player to jump briefly after leaving a platform
    private float JumpBuffer_Timer;
    [SerializeField] private float JumpBuffer_Seconds = 0.1f; //jump buffer to allow the player to jump immediately after hitting the ground if they failed the timing while falling
    public bool isGrounded; // To check if player is on the ground
    private bool jumpRequested; // To store jump request
    public bool justLanded = false;
    private int howManyJumps = 1;
    private int totalJumps = 0;

    [Header("Animation")]
    private Animator anim;
    private string moveAnim = "player_move";
    private string idleAnim = "player_idle";
    private string jumpUpAnim = "player_jump_up";
    private string landingAnim = "player_jump_land";

    [Header("Animals")]
    public GameObject playerModel;
    public Sprite playerSprite;
    public List<GameObject> animalModels = new List<GameObject>();
    private bool isBaseModelActive = true; // To track if the base model is active
    [SerializeField] private List<Animal> animals = new List<Animal>();
    public Animal currentlyActiveAnimal;
    public float abilityDuration = 5f; // Duration for the animal ability
    public float currentAbilityTimer;
    public bool isAbilityActive = false;
    [SerializeField] private int activeAnimalIndex = 0;

    private void Awake()
    {
        playerControls = new Player();

        // Subscribe to input action events
        playerControls.Gameplay.Movement.performed += ctx => movement = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Movement.canceled += ctx => movement = Vector2.zero;
        playerControls.Gameplay.Jump.performed += ctx => Jump();

        playerControls.Gameplay.Interact.performed += ctx => TryActivateAbility();

        playerControls.Gameplay.AnimalOne.performed += ctx => SwitchActiveAnimal(0);
        playerControls.Gameplay.AnimalTwo.performed += ctx => SwitchActiveAnimal(1);
        playerControls.Gameplay.AnimalThree.performed += ctx => SwitchActiveAnimal(2);
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
        UpdateAbilityTimer();
        CheckGroundStatus();
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

        if (!isGrounded)
        {
            // In air, smoothly interpolate towards the new velocity
            velocity.x = Mathf.Lerp(rb.velocity.x, velocity.x, Time.fixedDeltaTime * airControlFactor);
            velocity.z = Mathf.Lerp(rb.velocity.z, velocity.z, Time.fixedDeltaTime * airControlFactor);
        }


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

        if (jumpRequested && JumpBuffer_Timer > 0)
        {
            JumpBuffer_Timer -= Time.deltaTime;

            if (isGrounded || coyote_timer > 0)
            {
                PerformJump();
                jumpRequested = false; // Reset jumpRequested after the jump is performed
                JumpBuffer_Timer = 0; // Reset JumpBuffer_Timer to prevent double jumps
            }
        }else if(jumpRequested && coyote_timer <= 0){
            jumpRequested = false;
            JumpBuffer_Timer = 0;
        }
    }

    private void Jump()
    {
        // Check if the current animal allows for a normal jump
        if (currentlyActiveAnimal != null && !currentlyActiveAnimal.AllowNormalJump())
        {
            return; // If not allowed, exit the method and don't perform a normal jump
        }

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
        if(totalJumps < howManyJumps)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false; 
            totalJumps++;
        }
        
    }

    private void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;

        // Use the player's current rotation for the box check
        Quaternion groundCheckRotation = transform.rotation;

        // Offset the box position relative to the player (if needed)
        Vector3 groundCheckBoxPosition = groundCheckPosition.position + transform.TransformDirection(groundCheckOffset);

        isGrounded = Physics.CheckBox(groundCheckBoxPosition, groundCheckSize, groundCheckRotation, groundLayer);

        if (isGrounded)
        {
            if (!wasGrounded)
            {
                // Just landed
                justLanded = true;
                totalJumps = 0;
            }
            coyote_timer = coyote_seconds; // Reset coyote time when grounded
        }
        else
        {
            justLanded = false;
            coyote_timer -= Time.deltaTime; // Count down coyote time when in air
        }

        // Handle jump buffer
        if (jumpRequested && JumpBuffer_Timer > 0)
        {
            JumpBuffer_Timer -= Time.deltaTime;
            if (isGrounded)
            {
                PerformJump();
                jumpRequested = false; // Reset jumpRequested after the jump is performed
                JumpBuffer_Timer = 0; // Reset JumpBuffer_Timer to prevent double jumps
            }
        }
        else if (jumpRequested && JumpBuffer_Timer <= 0)
        {
            jumpRequested = false; // Reset jump request if buffer timer runs out
            JumpBuffer_Timer = 0;
        }
    }


    void OnDrawGizmos()
    {
        if(!isGrounded) Gizmos.color = Color.red;
        else if(isGrounded) Gizmos.color = Color.blue;

        // Assuming groundCheckPosition is a Transform representing the center of the ground check box
        Vector3 boxCenter = groundCheckPosition.position;

        // Apply the same offset (if any) as used in CheckGroundStatus
        boxCenter += transform.TransformDirection(groundCheckOffset);

        // Create a transformation matrix
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);

        // Apply the transformation matrix to the Gizmos
        Gizmos.matrix = rotationMatrix;

        // Draw the cube
        Gizmos.DrawCube(Vector3.zero, groundCheckSize);

        // Reset the Gizmos matrix
        Gizmos.matrix = Matrix4x4.identity;
    }





    #endregion


    #region Animals

    private void TryActivateAbility()
    {
        // Check if the player is in an animal form before activating any ability
        if (!isBaseModelActive && !isAbilityActive && animals.Count > activeAnimalIndex)
        {
            animals[activeAnimalIndex].Activate(this);
            isAbilityActive = true;
            currentAbilityTimer = abilityDuration;
        }
    }


    private void UpdateAbilityTimer()
    {
        if (isAbilityActive)
        {
            currentAbilityTimer -= Time.deltaTime;
            if (currentAbilityTimer <= 0)
            {
                ResetAbility();
            }
            else
            {
                // Let the currently active animal update its ability state.
                currentlyActiveAnimal?.UpdateAbilityState(this);
            }
        }
    }

    private void ResetAbility()
    {
        if (animals.Count > activeAnimalIndex)
        {
            animals[activeAnimalIndex].ResetAbility(this);
        }
        isAbilityActive = false;
    }

    // Method to switch active animal
    public void SwitchActiveAnimal(int index)
    {
       
        if (index >= 0 && index < animalModels.Count)
        {
            if (isBaseModelActive || activeAnimalIndex != index)
            {
                // Activate the selected animal model and deactivate the player model
                animalModels.ForEach(animal => animal.SetActive(false)); // Deactivate all animal models
                animalModels[index].SetActive(true); // Activate the selected animal model
                playerModel.SetActive(false);

                isBaseModelActive = false;
                activeAnimalIndex = index;
                currentlyActiveAnimal = animals[index];
            }
            else
            {
                // Deactivate the current animal model and revert to the player model
                animalModels[index].SetActive(false);
                playerModel.SetActive(true);

                isBaseModelActive = true;
                currentlyActiveAnimal = null;
                
            }
        }

        if(isAbilityActive) ResetAbility();
        
    }

    #endregion


    #region Visuals
    private void HandleAnimations()
    {
        if (justLanded)
        {
            anim.Play(landingAnim);
        }
        else if (rb.velocity.y > 0)
        {
            anim.Play(jumpUpAnim);
        }
        else if (movement != Vector3.zero && isGrounded)
        {
            anim.Play(moveAnim);
        }
        else
        {
            anim.Play(idleAnim);
        }
    }

    public void ResetLanding()
    {
        if (!jumpRequested)
        {

            justLanded = false;
        }
    }
    #endregion
}
