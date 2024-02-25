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


    private void Start()
    {
        StartCoroutine(ShootBallsAtFrequency(1f));
    }

    IEnumerator ShootBallsAtFrequency(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay); // Initial delay before starting the loop

        while (canSpawn)
        {
            ShootBall();
            yield return new WaitForSeconds(spawnFrequency);
        }
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

