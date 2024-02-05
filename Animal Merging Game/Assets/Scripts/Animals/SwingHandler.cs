using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingHandler : MonoBehaviour
{
    public Player_Def player; // Reference to the Player_Def script
    public Transform grappleStartPosition; // The start position of the grappling line

    private LineRenderer lineRenderer;
    [SerializeField] private Material renderMaterial;
    [SerializeField] private float startWidth = 0.1f;
    [SerializeField] private float endWidth = 0.1f;

    public Color startColor;
    public Color endColor;

    void LateUpdate()
    {
        // Check if the currently active animal is a Spider
        if (player.currentlyActiveAnimal is Spider spider && player.CurrentState == Player_Def.PlayerState.Swinging)
        {
            if(lineRenderer == null) {lineRenderer = gameObject.AddComponent<LineRenderer>();}
            
            Vector3 grapplePoint = spider.grapplePoint; // Access the grapple point from the Spider script
            SetLR(grapplePoint);
            if (!lineRenderer.enabled) lineRenderer.enabled = true;
        }
        else if (player.CurrentState != Player_Def.PlayerState.Swinging)
        {
            Destroy(lineRenderer);
        }
    }

    private void SetLR(Vector3 grapplePoint)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.positionCount = 2; // Two points (start and end of the line)
        lineRenderer.material = renderMaterial;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        lineRenderer.SetPosition(0, grappleStartPosition.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}

