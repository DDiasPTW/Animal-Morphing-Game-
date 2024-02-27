using System.Collections;
using UnityEngine;

public class MovingWallManager : MonoBehaviour
{
    public GameObject ballPrefab; // Assign the ball prefab with a Rigidbody
    public Vector3 shootDirection = Vector3.forward; // Direction to shoot the ball
    public float shootForce = 500f; // Force applied to shoot the ball
    public float ballLifetime = 5f; // Time before the ball gets destroyed
    public float spawnFrequency = 2f; // Frequency of shooting balls
    public bool canSpawn = true;
    public float initialSpawnDelay = 1f;

    //---Visuals
    [SerializeField] private GameObject shootParticles;
    [SerializeField] private GameObject readyParticles;
    [SerializeField] private bool spawnedReadyParticles = false;


    private void Start()
    {
        StartCoroutine(ShootBallsAtFrequency(initialSpawnDelay));
    }


    // IEnumerator ShootBallsAtFrequency(float initialDelay)
    // {
    //     yield return new WaitForSeconds(initialDelay); // Initial delay before starting the loop

    //     while (canSpawn)
    //     {
    //         ShootBall();
    //         SpawnShootParticles();
    //         yield return new WaitForSeconds(spawnFrequency);
    //     }
    // }

    IEnumerator ShootBallsAtFrequency(float initialDelay)
{
    yield return new WaitForSeconds(initialDelay); // Initial delay before starting the loop

    while (canSpawn)
    {
        float timeUntilNextSpawn = spawnFrequency; // Reset the timer every loop

        ShootBall();
        SpawnShootParticles();

        while (timeUntilNextSpawn > 0)
        {
            timeUntilNextSpawn -= Time.deltaTime; // Decrement the timer

            // Check if it's time to spawn ready particles and if they haven't been spawned yet
            if (timeUntilNextSpawn <= 1f && !spawnedReadyParticles)
            {
                SpawnReadyParticles();
            }

            yield return null; // Wait until the next frame to continue the loop
        }

        
    }
}



    private void SpawnShootParticles(){
        GameObject pS = Instantiate(shootParticles, transform);
            pS.transform.SetParent(null);
            StartCoroutine(DestroyParticles(pS));
    }

    private void SpawnReadyParticles()
    {
        spawnedReadyParticles = true;
        GameObject pS = Instantiate(readyParticles, transform);
        pS.transform.SetParent(null);
        StartCoroutine(DestroyParticles(pS));
    }

    IEnumerator DestroyParticles(GameObject particles)
    {
        yield return new WaitForSeconds(1f);
        Destroy(particles);
        spawnedReadyParticles = false;
    }

    void ShootBall()
    {
        // Instantiate the ball at the position and rotation of the shooter
        GameObject ballInstance = Instantiate(ballPrefab, transform.position, transform.rotation);

        // Apply force to shoot the ball
        Rigidbody rb = ballInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(shootDirection.normalized * shootForce);
        }
        // Destroy the ball after a set time
        Destroy(ballInstance, ballLifetime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;   
        Gizmos.DrawLine(transform.position,transform.position + shootDirection * 10);
    }
}

