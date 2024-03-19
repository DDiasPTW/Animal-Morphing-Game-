using UnityEngine;

public class FloatingHead : MonoBehaviour
{
    private Transform player;
    public float orbitSpeed;
    public float orbitMinRadius = 2f; // Distance from the player
    public float orbitMaxRadius = 2f; // Distance from the player
    public float orbitMinHeight = 2f;
    public float orbitMaxHeight = 2f;
    public float orbitFrequency = 1f; // Frequency of orbit changes
    public float orbitMinRange = 0f; // Minimum range for orbit angle
    public float orbitMaxRange = 60f; // Max angle range for orbit
    public float orbitChangeSpeed = 30f; // Speed of angle change

    private float orbitAngle = 0f;
    private float targetOrbitAngle = 0f;
    private float targetOrbitHeight = 0f;
    private float orbitRadius;
    private float timeSinceLastOrbit = 0f;
    [Header("Emotions")]
    [SerializeField] private Mesh NeutralMesh;
    [SerializeField] private Mesh HappyMesh;
    [SerializeField] private Mesh SadMesh;
    [SerializeField] private Mesh AngryMesh;
    private enum Emotion
    {
        Neutral,
        Happy,
        Sad,
        Angry
    }

    

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PlayerFollow").transform;
    }

    void Start()
    {
        // Initialize orbit radius randomly within the given range
        orbitRadius = Random.Range(orbitMinRadius, orbitMaxRadius);

        // Start with a random orbit angle within the specified range
        orbitAngle = Random.Range(-orbitMaxRange, orbitMaxRange);
        float orbitHeight = Random.Range(orbitMinHeight, orbitMaxHeight);
        targetOrbitAngle = orbitAngle;
        targetOrbitHeight = orbitHeight;
    }

    void Update()
    {
        OrbitAroundPlayer();
        UpdateOrbitAngle();
    }

    void OrbitAroundPlayer()
    {
        // Calculate orbit position relative to the world's forward direction (0, 0, 1)
        Quaternion orbitRotation = Quaternion.Euler(0f, orbitAngle, 0f);
        Vector3 orbitDirection = orbitRotation * Vector3.forward;

        Vector3 orbitPosition = (player.position + new Vector3(0, targetOrbitHeight, 0)) + orbitDirection * orbitRadius;

        // Smoothly move towards the orbit position
        transform.position = Vector3.Lerp(transform.position, orbitPosition, orbitSpeed * Time.deltaTime);

        // Rotate the FloatingHead to face the player
        transform.LookAt(player);
    }

    void UpdateOrbitAngle()
    {
        // Update time since last orbit change
        timeSinceLastOrbit += Time.deltaTime;

        // If enough time has passed, choose a new target orbit angle
        if (timeSinceLastOrbit >= orbitFrequency)
        {

            // If enough time has passed, choose a new target orbit angle
            if (timeSinceLastOrbit >= orbitFrequency)
            {
                // Choose a random value within the positive effective range or negative effective range
                float randomAngle = Random.Range(0f, 1f) < 0.5f ? Random.Range(orbitMinRange, orbitMaxRange) : Random.Range(-orbitMaxRange, -orbitMinRange);

                // Set the target orbit angle
                targetOrbitAngle = randomAngle;
                //Debug.Log(targetOrbitAngle);

                targetOrbitHeight = Random.Range(orbitMinHeight, orbitMaxHeight);
                orbitRadius = Random.Range(orbitMinRadius, orbitMaxRadius);
                timeSinceLastOrbit = 0f;
            }
        }

        // Smoothly transition towards the target orbit angle
        orbitAngle = Mathf.MoveTowardsAngle(orbitAngle, targetOrbitAngle, orbitChangeSpeed * Time.deltaTime);
    }

}
