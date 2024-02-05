using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraLimits : MonoBehaviour
{
    public float minX = -3f;
    public float maxX = 3f;

    private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer transposer;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update()
    {
        var currentPosition = vcam.transform.position;
        var limitedXPosition = Mathf.Clamp(currentPosition.x, minX, maxX);
        vcam.transform.position = new Vector3(limitedXPosition, currentPosition.y, currentPosition.z);
    }
}
