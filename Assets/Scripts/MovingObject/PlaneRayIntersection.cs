using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneRayIntersection : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform movingObject, plane;


    // Update is called once per frame
    void Update()
    {
        Vector3 planePosition = plane.position;
        Vector3 planeNormal = -plane.up;

        Vector3 objectPosition = movingObject.position;
        Vector3 objectForward = movingObject.forward;

        float denominator = MathLibrary.DotProduct(planeNormal, objectForward);
        if (denominator > 0.0000001f)
        {
            float t = MathLibrary.DotProduct(planePosition - objectPosition, planeNormal) / denominator;
            Vector3 rayIntersection = objectPosition + t * objectForward;

            lineRenderer.SetPosition(0, objectPosition);
            lineRenderer.SetPosition(1, rayIntersection);
        }
        else
        {
            Debug.Log("NO RAY");
        }
    }
}
