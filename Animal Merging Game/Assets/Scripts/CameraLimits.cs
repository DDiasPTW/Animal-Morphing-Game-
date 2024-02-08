using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraLimits : MonoBehaviour
{
    public Vector3 targetRotation;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float rotationDuration = 1f;


    void Awake()
    {
        vcam = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(RotateCameraToTarget());
        }
    }

     IEnumerator RotateCameraToTarget()
    {
        Quaternion startRotation = vcam.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            // Interpolate rotation over time
            vcam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the target rotation is set after interpolation
        vcam.transform.rotation = endRotation;
    }
}
