using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player_Def : MonoBehaviour
{
    [Header("Other")]
    [HideInInspector] public Player playerControls;
    [HideInInspector] public Rigidbody rb;
    private Transform cameraTransform;
    [SerializeField] private Vector3 groundCheckSize;
    [SerializeField] private float castDistance;
    [SerializeField] private Transform groundCheckPosition;
    public LayerMask groundLayer;

    [Header("Movement")]
    public float moveSpeed = 6f;
    [HideInInspector] public float startMoveSpeed;
    public float airControlFactor = 6f;
    private Vector3 movement;

    [Header("Jumping")]
    public float jumpForce = 10.0f;
    public float peakJumpHeight;
    public static float globalGravity = -9.81f;
    [SerializeField] private float coyote_timer;
    [SerializeField] private float coyote_seconds = 0.1f; //coyote timer to allow the player to jump briefly after leaving a platform
    [SerializeField] private float JumpBuffer_Timer;
    [SerializeField] private float JumpBuffer_Seconds = 0.1f; //jump buffer to allow the player to jump immediately after hitting the ground if they failed the timing while falling
    public bool isGrounded; // To check if player is on the ground
    private bool jumpRequested; // To store jump request
    public bool justLanded = false;
    private bool wasFalling = false;
    private int howManyJumps = 1;
    [SerializeField] private int totalJumps = 0;

    [Header("Variable Jump")]
    public float normalGravityScale = 1.0f; // Normal gravity
    public float increasedGravityScale = 2.0f; // Gravity when jump is released early
    private bool endedJumpEarly = false; // Flag to check if jump was released early


    [Header("Animation")]
    [SerializeField] private PlayerState currentState;
    public PlayerState CurrentState
    {
        get { return currentState; }
        private set { currentState = value; }
    }
    public enum PlayerState{
        Idle,
        Moving,
        Jumping,
        Falling,
        Landing,
        Swinging
    }
    
    [Header("Animals")]
    [SerializeField] private List<GameObject> animalGameObjects = new List<GameObject>();
    [SerializeField] private List<Animal> animals = new List<Animal>();
    public Animal currentlyActiveAnimal;
    [SerializeField] private GameObject defaultPlayerGameObject;
    [SerializeField] private int activeAnimalIndex = 0;
    public bool isBouncing = false; // Flag to indicate if the player is currently bouncing
    public List<Image> animalSprites = new List<Image>();

    [Header("Visuals")]
    public GameObject landingParticles;
    private bool landingParticlesSpawned = false;
    public GameObject jumpingParticles;
    private bool jumpingParticlesSpawned = false;
    public GameObject swapParticles;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SubscribeToInputs();
        GetUi();
        defaultPlayerGameObject.SetActive(true);
        defaultPlayerGameObject.GetComponent<AnimationsHandler>().player = this;
    }

    private void GetUi()
    {
        if (animalSprites.Count > 0)
        {
            // Set the rest of the elements to the animal sprites
            for (int i = 0; i < animals.Count && i + 1 < animalSprites.Count; i++)
            {
                animalSprites[i].sprite = animals[i].animalSprite;
            }
        }
    }

    private void SubscribeToInputs()
    {
        playerControls = new Player();

        playerControls.Gameplay.Movement.performed += ctx => movement = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Movement.canceled += ctx => movement = Vector2.zero;

        playerControls.Gameplay.Jump.performed += ctx => Jump();

        playerControls.Gameplay.Jump.canceled += ctx =>
    {
        endedJumpEarly = true;
        if (currentlyActiveAnimal is Spider spider)
        {
            spider.HandleJumpRelease(this);
        }
        
        if(currentlyActiveAnimal is FlyingSquirrel fSquirrel){
            fSquirrel.HandleJumpRelease(this);
        }
    };
        playerControls.Gameplay.Interact.performed += ctx => TryActivateAbility();

        playerControls.Gameplay.AnimalOne.performed += ctx => SwitchActiveAnimal(0);
        playerControls.Gameplay.AnimalTwo.performed += ctx => SwitchActiveAnimal(1);
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
        rb.useGravity = false;
        cameraTransform = Camera.main.transform; // Get the main camera's transform
        isGrounded = false;
        jumpRequested = false;
        startMoveSpeed = moveSpeed;
    }

    void FixedUpdate()
    {
        Move();
        ApplyGravity();
    }

    void Update()
    {
        CheckJump();
        CheckJumpPeak();
        HandleAnimations();
        CheckGroundStatus();

        // Handle continuous jump button press for Flying Squirrel gliding
        if (!isGrounded && playerControls.Gameplay.Jump.ReadValue<float>() > 0 && rb.velocity.y < 0)
        {
            if (currentlyActiveAnimal is FlyingSquirrel fSquirrel)
            {
                fSquirrel.HandleJump(this);
            }
        }
    }

    private void CheckJumpPeak()
    {
        if (!isGrounded)
        {
            peakJumpHeight = Mathf.Max(peakJumpHeight, transform.position.y);
        }
        else if (justLanded)
        {
            // When the player lands, check if they should bounce
            currentlyActiveAnimal?.UpdateAbilityState(this);
        }
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

        Vector3 moveVector = (forward * movement.y) + (right * movement.x); // Direction of movement
        Vector3 finalMoveVector = moveVector.normalized;
        if (moveVector != Vector3.zero)
        {
            // Rotate the GameObject to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(finalMoveVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
        }

        // Calculate the velocity
        if(currentlyActiveAnimal is Spider spider && spider.isSwinging)
        {
            return;
        }

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
        float currentGravityScale = (endedJumpEarly && rb.velocity.y > 0 && !isBouncing) ? increasedGravityScale : normalGravityScale;
        Vector3 gravity = globalGravity * currentGravityScale * Vector3.up;
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
        }
        else if (jumpRequested && coyote_timer <= 0)
        {
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

        if (isGrounded || (coyote_timer > 0 && !jumpRequested))
        {
            // Player is grounded or within coyote time and hasn't already requested a jump
            PerformJump();
            // Reset coyote timer as jump has been made
            coyote_timer = 0;
        }
        else if (!isGrounded)
        {
            // Player is in the air and not already grappling
            if (currentlyActiveAnimal is Spider spider && !spider.isSwinging)
            {
                spider.HandleJump(this);
            }

            // Set up jump buffer
            jumpRequested = true;
            JumpBuffer_Timer = JumpBuffer_Seconds;
        }

        // Listen for jump release
        playerControls.Gameplay.Jump.canceled += ctx => endedJumpEarly = true;
    }

    private void PerformJump()
    {
        if (totalJumps < howManyJumps)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            totalJumps++;
            //isGrounded = false;
            endedJumpEarly = false;
        }

    }

    private void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;

        Vector3 castOrigin = groundCheckPosition.position + Vector3.up * groundCheckSize.y;
        float castRadius = groundCheckSize.x / 2;
        float cD = groundCheckSize.y + castDistance; // Small distance below the capsule

        RaycastHit hit;
        isGrounded = Physics.SphereCast(castOrigin, castRadius, Vector3.down, out hit, castDistance, groundLayer);

        // Check if the player is currently falling (moving downwards and not grounded)
        if (rb.velocity.y < 0 && !isGrounded)
        {
            wasFalling = true;
        }

        if (isGrounded)
        {
            totalJumps = 0;
            endedJumpEarly = false;
            if (!wasGrounded && wasFalling)
            {
                justLanded = true;
                //totalJumps = 0;
                wasFalling = false;

                if (currentlyActiveAnimal is Sheep sheep)
                {
                    sheep.UpdateAbilityState(this);
                }
                if(currentlyActiveAnimal is FlyingSquirrel fS){
                    fS.HandleJumpRelease(this);
                }

                peakJumpHeight = transform.position.y; // Update last grounded height
                StartCoroutine(ResetJustLanded());
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
                justLanded = false;
                jumpRequested = false; // Reset jumpRequested after the jump is performed
                JumpBuffer_Timer = 0; // Reset JumpBuffer_Timer to prevent double jumps
            }
        }
        else if (jumpRequested && JumpBuffer_Timer <= 0)
        {
            jumpRequested = false; // Reset jump request if buffer timer runs out
            JumpBuffer_Timer = 0;
        }

        // Reset the falling state if the player is grounded
        if (isGrounded)
        {
            wasFalling = false;
        }
    }

    IEnumerator ResetJustLanded()
    {
        yield return new WaitForSeconds(0.1f); // Short delay
        justLanded = false;
    }
    #endregion

    #region Animals
    private void TryActivateAbility()
    {
        // Check if the player is in an animal form before activating any ability
        if (animals.Count != activeAnimalIndex)
        {
            Debug.Log("Activated ability");
            animals[activeAnimalIndex].Activate(this);
        }
    }

    public void SwitchActiveAnimal(int index)
    {
        // Check if the index is within the bounds of the animals list
        if (index < 0 || index >= animals.Count) return;

        // Check if the new animal is the same as the currently active one
        if (index == activeAnimalIndex && currentlyActiveAnimal != null) return;

        // First, reset any currently active ability
        if (currentlyActiveAnimal != null)
        {
            currentlyActiveAnimal.ResetAbility(this);
            animalGameObjects[activeAnimalIndex].SetActive(false); // Deactivate the current animal GameObject
        }

        GameObject sP = Instantiate(swapParticles, transform);
        StartCoroutine(DestroyParticles(sP));

        // Only reset lastGroundedHeight if the player is grounded
        if (isGrounded)
        {
            peakJumpHeight = transform.position.y;
        }

        // Activate the new animal GameObject
        defaultPlayerGameObject.SetActive(false);
        animalGameObjects[index].SetActive(true);
        animalGameObjects[index].GetComponent<AnimationsHandler>().player = this;

        activeAnimalIndex = index;
        currentlyActiveAnimal = animals[index];
        currentlyActiveAnimal.Activate(this);
    }

    #endregion

    #region Visuals
    private void HandleAnimations()
    {
        //swinging animation (spider)
        if(currentlyActiveAnimal is Spider spider && spider.isSwinging){
            currentState = PlayerState.Swinging;
        }
        //jump up animation
        else if (rb.velocity.y > 0 && !isGrounded && !justLanded)
        {
            currentState = PlayerState.Jumping;
            landingParticlesSpawned = false;
            if (!jumpingParticlesSpawned)
            {
                GameObject pS = Instantiate(jumpingParticles, groundCheckPosition);
                pS.transform.SetParent(null);
                StartCoroutine(DestroyParticles(pS));
                jumpingParticlesSpawned = true;
            }

        }
        //jump down animation
        else if (rb.velocity.y < 0 && !isGrounded)
        {
            currentState = PlayerState.Falling;
            jumpingParticlesSpawned = false;
            landingParticlesSpawned = false;
        }
        //landing animation
        else if (justLanded)
        {
            jumpingParticlesSpawned = false;
            if (!landingParticlesSpawned)
            {
                GameObject pS = Instantiate(landingParticles, groundCheckPosition);
                pS.transform.SetParent(null);
                landingParticlesSpawned = true;
                StartCoroutine(DestroyParticles(pS));
            }

            currentState = PlayerState.Landing;
        }
        //running animation
        else if (movement != Vector3.zero && isGrounded)
        {
            currentState = PlayerState.Moving;
        }
        //idle animation
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    public void ResetLanding()
    {
        if (!jumpRequested)
        {
            justLanded = false;
            landingParticlesSpawned = false;
        }
    }

    IEnumerator DestroyParticles(GameObject particles)
    {
        yield return new WaitForSeconds(1f);
        Destroy(particles);
        landingParticlesSpawned = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.blue : Color.red;

        Vector3 castOrigin = groundCheckPosition.position + Vector3.up * groundCheckSize.y;
        float castRadius = groundCheckSize.x / 2;
        float cD = groundCheckSize.y + castDistance;

        // Draw the sphere at the start of the cast
        Gizmos.DrawWireSphere(castOrigin, castRadius);

        // Draw the sphere at the end of the cast
        Gizmos.DrawWireSphere(castOrigin + Vector3.down * cD, castRadius);


        if (currentlyActiveAnimal is Spider spider)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spider.grapplingRange);
        }

    }

    #endregion
}
