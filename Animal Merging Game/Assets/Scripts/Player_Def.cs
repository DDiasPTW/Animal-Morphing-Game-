using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player_Def : MonoBehaviour
{
    [Header("Other")]
    public Player playerControls;
    [HideInInspector] public Rigidbody rb;
    private Transform cameraTransform;
    [SerializeField] private Vector3 groundCheckSize;
    [SerializeField] private float castDistance;
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private float groundCheckDelayAfterJump = 0.05f; // 50ms delay before allowing ground check
    private float groundCheckTimer = 0f;
    public LayerMask groundLayer;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float accelerationTime = 1f; // Time it takes to reach full speed
    [HideInInspector] public float startMoveSpeed;
    public float airControlFactor = 6f;
    public Vector3 movement;
    private float currentAcceleration = 0f;
    private int howManyJumps = 1;
    private int totalJumps = 0;


    [Header("Jumping")]
    public float jumpForce = 10.0f;
    [HideInInspector] public float peakJumpHeight;
    public static float globalGravity = -9.81f;
    [SerializeField]private float coyote_timer;
    [SerializeField] private float coyote_seconds = 0.1f; //coyote timer to allow the player to jump briefly after leaving a platform
    private float JumpBuffer_Timer;
    [SerializeField] private float JumpBuffer_Seconds = 0.1f; //jump buffer to allow the player to jump immediately after hitting the ground if they failed the timing while falling
    public bool isGrounded; // To check if player is on the ground
    private bool jumpRequested; // To store jump request
    public bool justLanded = false;
    private bool wasFalling = false;

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
    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        Falling,
        Landing,
        Swinging
    }

    [Header("Animals")]
    [SerializeField] private float switchCooldown = 1.0f; // Cooldown duration in seconds
    private float lastSwitchTime = -1.0f; // Timestamp of the last switch
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

    #region Normal methods
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
            for (int i = 0; i < animals.Count && i < animalSprites.Count; i++)
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

        if (currentlyActiveAnimal is FlyingSquirrel fSquirrel)
        {
            fSquirrel.HandleJumpRelease(this);
        }
    };

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
        if (groundCheckTimer > 0)
        {
            groundCheckTimer -= Time.deltaTime;
        }

        CheckGroundStatus(); // Only check ground status if the timer has elapsed

        HandleJumpInput();
        HandleAnimations();

        //------- ANIMALS -------//
        CheckJumpPeak();
        // Handle continuous jump button press for Flying Squirrel gliding
        if (!isGrounded && playerControls.Gameplay.Jump.ReadValue<float>() > 0 && rb.velocity.y < 0)
        {
            if (currentlyActiveAnimal is FlyingSquirrel fSquirrel)
            {
                fSquirrel.HandleJump(this);
            }
        }
    }


    #endregion

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
        if (moveVector != Vector3.zero)
        {
            // Rotate the GameObject to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
        }

        if (currentlyActiveAnimal is Spider spider && spider.isSwinging)
        {
            return;
        }

        // Gradually increase the speed based on acceleration time
        float targetSpeed = moveVector.magnitude * moveSpeed;
        if (moveVector.magnitude > 0)
        {
            currentAcceleration += Time.deltaTime / accelerationTime;
        }
        else
        {
            currentAcceleration = 0f;
        }
        currentAcceleration = Mathf.Clamp01(currentAcceleration); // Ensure it stays within 0 to 1

        float smoothedSpeed = Mathf.Lerp(0, targetSpeed, currentAcceleration);

        // Apply smoothed speed to the velocity vector
        Vector3 desiredVelocity = new Vector3(moveVector.x * smoothedSpeed, rb.velocity.y, moveVector.z * smoothedSpeed);

        if (!isGrounded)
        {
            // Apply air control factor
            desiredVelocity.x = Mathf.Lerp(rb.velocity.x, moveVector.x * smoothedSpeed, Time.fixedDeltaTime * airControlFactor);
            desiredVelocity.z = Mathf.Lerp(rb.velocity.z, moveVector.z * smoothedSpeed, Time.fixedDeltaTime * airControlFactor);
        }

        // Apply the calculated velocity to the Rigidbody
        rb.velocity = desiredVelocity;
    }

    private void ApplyGravity()
    {
        float currentGravityScale = (endedJumpEarly && rb.velocity.y > 0 && !isBouncing) ? increasedGravityScale : normalGravityScale;
        Vector3 gravity = globalGravity * currentGravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;

        // CheckGroundStatus logic remains largely unchanged but streamlined for clarity
        Vector3 castOrigin = groundCheckPosition.position + Vector3.up * groundCheckSize.y / 2;
        Vector3 boxHalfExtents = groundCheckSize * 0.5f;
        isGrounded = Physics.BoxCast(castOrigin, boxHalfExtents, Vector3.down, out _, Quaternion.identity, castDistance + groundCheckSize.y / 2, groundLayer) && groundCheckTimer <= 0;


        // Check if the player is currently falling (moving downwards and not grounded)
        if (rb.velocity.y < 0 && !isGrounded)
        {
            wasFalling = true;
        }
        
        if (isGrounded)
        {   
            ResetJumpState();
            if(!wasGrounded && wasFalling)
            {
                HandleLanding();
            }
            
        }
        else
        {
            justLanded = false;
            coyote_timer -= Time.deltaTime;
        }

    }

    private void HandleJumpInput()
    {
        JumpBuffer_Timer -= Time.deltaTime;

        if (jumpRequested)
        {
            // Check for jump conditions within the buffered time or coyote time
            if ((isGrounded || coyote_timer > 0) && totalJumps < howManyJumps)
            {
                PerformJump();
                // Reset jumpRequested inside PerformJump or here after a successful jump
                jumpRequested = false;
            }
            else if (JumpBuffer_Timer <= 0)
            {
                // Reset jumpRequested if the jump buffer timer has expired without fulfilling jump conditions
                jumpRequested = false;
            }
            
            // Player is in the air and not already grappling
            if (currentlyActiveAnimal is Spider spider && !spider.isSwinging)
            {
                spider.HandleJump(this);
            }
        }

    }

    public void Jump()
    {
        // Request a jump, consider animal-specific conditions here
        if (currentlyActiveAnimal == null || currentlyActiveAnimal.AllowNormalJump())
        {
            jumpRequested = true;
            JumpBuffer_Timer = JumpBuffer_Seconds;
        }
    }

    private void PerformJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        totalJumps++;
        endedJumpEarly = false;
        groundCheckTimer = groundCheckDelayAfterJump;
       
        //Debug.Log($"Jump performed. isGrounded= {isGrounded}, totalJumps= {totalJumps}, coyoteTimer= {coyote_timer}, jumpBufferTimer= {JumpBuffer_Timer}, playerState= {currentState}");
    }

    private void ResetJumpState()
    {
        if (isGrounded)
        {
            totalJumps = 0;
            coyote_timer = coyote_seconds; // Refresh coyote time on landing
        }
    }

    private void HandleLanding()
    {
        // Handle landing logic, including landing particles and animal-specific actions
        currentlyActiveAnimal?.UpdateAbilityState(this);
        justLanded = true;
        StartCoroutine(ResetJustLanded());
        wasFalling = false;

    }

    IEnumerator ResetJustLanded()
    {
        yield return new WaitForSeconds(0.25f); // Short delay with the duration of the landing animation
        justLanded = false;
        landingParticlesSpawned = false;
    }
    #endregion

    #region Animals
    public void SwitchActiveAnimal(int index)
    {
        // Check if we are within the cooldown period
        float currentTime = Time.time;
        if (currentTime - lastSwitchTime < switchCooldown) return;


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
        animalGameObjects[index].GetComponent<AnimationsHandler>().NotifyFormSwapped();

        activeAnimalIndex = index;
        currentlyActiveAnimal = animals[index];
        currentlyActiveAnimal.Activate(this);

        lastSwitchTime = currentTime; // Update the timestamp of the last switch
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


    #endregion

    #region Visuals
    private void HandleAnimations()
    {
        //swinging animation (spider)
        if (currentlyActiveAnimal is Spider spider && spider.isSwinging)
        {
            currentState = PlayerState.Swinging;
        }
        //jump up animation
        else if (rb.velocity.y > 0 && !isGrounded && !justLanded)
        {
            currentState = PlayerState.Jumping;
            SpawnJumpingParticles();
        }
        //jump down animation
        else if (rb.velocity.y < 0 && !isGrounded)
        {
            currentState = PlayerState.Falling;
            jumpingParticlesSpawned = false;
        }
        //landing animation
        else if (justLanded)
        {
            currentState = PlayerState.Landing;
            SpawnLandingParticles();
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

    private void SpawnJumpingParticles()
    {
        if (!jumpingParticlesSpawned)
        {
            jumpingParticlesSpawned = true;
            GameObject pS = Instantiate(jumpingParticles, groundCheckPosition);
            pS.transform.SetParent(null);
            StartCoroutine(DestroyParticles(pS));
        }
    }

    private void SpawnLandingParticles()
    {
        if (!landingParticlesSpawned)
        {
            //Debug.Log("Spawned landing particles");
            landingParticlesSpawned = true;
            GameObject pS = Instantiate(landingParticles, groundCheckPosition);
            pS.transform.SetParent(null);
            StartCoroutine(DestroyParticles(pS));
        }
    }

    IEnumerator DestroyParticles(GameObject particles)
    {
        yield return new WaitForSeconds(1f);
        Destroy(particles);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // Adjust the cast origin for BoxCast visualization
        Vector3 castOrigin = groundCheckPosition.position + Vector3.up * groundCheckSize.y / 2;
        Vector3 boxHalfExtents = new Vector3(groundCheckSize.x / 2, groundCheckSize.y / 2, groundCheckSize.z / 2);
        Quaternion rotation = Quaternion.identity; // Assuming the box is not rotated

        // Calculate the full distance to move the box for visualization
        Vector3 endPosition = castOrigin + Vector3.down * (castDistance + groundCheckSize.y);

        // Draw the box at the start of the cast
        Gizmos.matrix = Matrix4x4.TRS(castOrigin, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2); // Gizmos are drawn relative to the Gizmos.matrix

        // Draw the box at the end of the cast
        Gizmos.matrix = Matrix4x4.TRS(endPosition, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2); // Multiply by 2 to get full size since DrawWireCube expects full size

        // Reset Gizmos.matrix to default
        Gizmos.matrix = Matrix4x4.identity;


        //See the range of the spider swing mechanic
        if (currentlyActiveAnimal is Spider spider)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spider.grapplingRange);
        }
    }
    #endregion
}