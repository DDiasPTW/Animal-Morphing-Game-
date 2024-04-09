using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    [Header("Values")]
    public Vector3 shootDirection = Vector3.forward;
    public float maxDistance = 5f;
    public float spawnInterval = 3f;
    [SerializeField] private float startDelay = 0f;
    public float lineDuration = 1f;
    public float chargingDuration = 1f;
    private LineRenderer lineRenderer;

    [Header("Visuals")]
    [SerializeField] private GameObject chargingParticles;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private float cameraShakeIntensity = 2f;
    [Header("Audio")]
    private AudioSource aS;
    [SerializeField] private AudioClip chargingAudio;
    [SerializeField] private float chargingVolume;
    [SerializeField] private AudioClip hitAudio;
    [SerializeField] private float hitVolume;
    [SerializeField] private float hitPitch = 0.9f;
    [SerializeField] private AudioClip shootingAudio;
    [SerializeField] private float shootingVolume;

    [Header("Kill Player")]
    public bool canAttack = true;
    private BoxCollider col;
    private GameManager gM;
    [SerializeField] private float laserWidth = 2f;
    private Transform playerTransform;
    [SerializeField] private LayerMask playerLayer;


    void Awake()
    {
        canAttack = true;
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        lineRenderer = GetComponent<LineRenderer>();
        aS = GetComponent<AudioSource>();
        col = GetComponent<BoxCollider>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        SetCollider();
    }

    private void SetCollider()
    {
        col.size = new Vector3(laserWidth / 1.3f, laserWidth / 1.3f, maxDistance);
        if (shootDirection.z != 0) //(0,0,1) or (0,0,-1)
        {
            col.center = new Vector3(0, 0, shootDirection.z * (maxDistance / 2));
        }
        else if (shootDirection.x > 0) //(1,0,0)
        {
            col.center = new Vector3(0, 0, -shootDirection.x * (maxDistance / 2));
        }
        else if (shootDirection.x < 0) //(-1,0,0)
        {
            col.center = new Vector3(0, 0, shootDirection.x * (maxDistance / 2));
        }
        else if (shootDirection.y > 0) //(0,1,0)
        {
            col.center = new Vector3(0, 0, -shootDirection.y * (maxDistance / 2));
        }

        col.enabled = false;
    }

    void Start()
    {
        chargingDuration = chargingAudio.length;
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component not found!");
            return;
        }
        CameraShake.Instance.ShakeCamera(0, 0);
        // Start the coroutine to spawn lines at intervals
        StartCoroutine(DelayedStartCoroutine(startDelay));

    }

    IEnumerator DelayedStartCoroutine(float delayTime)
{
    yield return new WaitForSeconds(delayTime);
    // Start the coroutine to spawn lines at intervals
    StartCoroutine(SpawnLaser());
}

    private void SpawnChargingParticles()
    {
        aS.volume = chargingVolume;
        aS.PlayOneShot(chargingAudio);
        GameObject pS = Instantiate(chargingParticles, transform);
        pS.transform.SetParent(null);
        pS.transform.position += shootDirection * .5f;
        StartCoroutine(DestroyChargingParticles(pS));
    }

    IEnumerator DestroyChargingParticles(GameObject particles)
    {
        yield return new WaitForSeconds(chargingAudio.length);
        Destroy(particles);
    }


    private void SpawnHitParticles(Vector3 hitPoint, Vector3 normal)
    {
        // Instantiate hit particles slightly offset from the hit point along the normal
        Vector3 offsetPosition = hitPoint + normal * 0.25f; // Adjust offset distance as needed
        GameObject pS = Instantiate(hitParticles, offsetPosition, Quaternion.identity);

        // Rotate the hit particles to align with the surface normal
        Quaternion rotation = Quaternion.LookRotation(normal);
        pS.transform.rotation = rotation;

        StartCoroutine(StopHitParticles(pS));
    }

    IEnumerator StopHitParticles(GameObject particles)
    {
        yield return new WaitForSeconds(lineDuration);
        particles.GetComponent<ParticleSystem>().Stop();
        StartCoroutine(DestroyChargingParticles(particles));
    }



    private IEnumerator SpawnLaser()
    {
        while (canAttack && !gM.levelFinished)
        {
            // Spawn charging particles
            SpawnChargingParticles();

            // Wait for charging duration before firing
            yield return new WaitForSeconds(chargingDuration);

            //Play the shooting Sound Effect
            aS.volume = shootingVolume;
            aS.PlayOneShot(shootingAudio);
            // Enable LineRenderer
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
            lineRenderer.enabled = true;

            // Draw first point
            lineRenderer.SetPosition(0, transform.position);


            RaycastHit hit;

            if (Physics.Raycast(transform.position, shootDirection, out hit, maxDistance, ~playerLayer))
            {
                lineRenderer.SetPosition(1, hit.point);

                SpawnHitParticles(hit.point, hit.normal);

                // Calculate distance between player and hit point
                float distanceToPlayer = Vector3.Distance(hit.point, playerTransform.position);

                // Adjust camera shake intensity based on distance
                float adjustedIntensity = cameraShakeIntensity / distanceToPlayer;

                // Clamp the intensity to prevent it from becoming too large
                adjustedIntensity = Mathf.Clamp(adjustedIntensity, 1f, cameraShakeIntensity); // Adjust the range as needed
                CameraShake.Instance.ShakeCamera(adjustedIntensity, lineDuration);
            }
            else
            {
                lineRenderer.SetPosition(1, transform.position + shootDirection * maxDistance);
            }
            col.enabled = true;
            yield return null; // Wait for the next frame before performing the next raycast check

            // Wait for line duration
            yield return new WaitForSeconds(lineDuration);

            //Disable the collider
            col.enabled = false;
            // Disable LineRenderer
            lineRenderer.enabled = false;

            // Fade out the audio gradually
            float fadeDuration = 0.3f;
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);
                aS.volume = Mathf.Lerp(shootingVolume, 0f, t);
                yield return null;
            }

            // Stop the audio once faded out
            aS.Stop();

            // Wait for next spawn interval
            yield return new WaitForSeconds(spawnInterval);

        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player_Def>().moveSpeed = 2f;
            other.GetComponent<Player_Def>().jumpForce = 3f;

            aS.volume = hitVolume;
            aS.pitch = hitPitch;
            aS.PlayOneShot(hitAudio);
            Narrator.Instance.TriggerLaserLines();
            StartCoroutine(RestartLevel());
        }
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(.3f);
        if (!gM.levelFinished)
        {
            gM.ResetLevel();
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + shootDirection * maxDistance);
    }
}
