using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 1.0f;
    private float startTime;
    private float journeyLength;

    void Start()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(pointA.position, pointB.position);
    }

    void Update()
    {
        // Calculate the current duration of the journey
        float distCovered = (Time.time - startTime) * speed;
        
        // Calculate the fraction of the journey completed
        float fractionOfJourney = distCovered / journeyLength;
        
        // Use PingPong to oscillate the fraction between 0 and 1
        float pingPong = Mathf.PingPong(fractionOfJourney, 1);
        
        // Interpolate position between pointA and pointB based on pingPong value
        transform.position = Vector3.Lerp(pointA.position, pointB.position, pingPong);
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player")){
            other.gameObject.transform.parent = gameObject.transform;
        }
    }

    void OnCollisionExit(Collision other)
    {
         if(other.gameObject.CompareTag("Player")){
            other.gameObject.transform.parent = null;
        }
    }
}
