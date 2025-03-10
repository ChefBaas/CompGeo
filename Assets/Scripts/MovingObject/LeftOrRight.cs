using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftOrRight : MonoBehaviour
{
    [SerializeField] private Transform movingObject;
    [SerializeField] private WaypointManager waypointManager;

    private void Update()
    {
        Vector3 objectDirection = movingObject.forward;
        Vector3 waypointDirection = waypointManager.Next.position - movingObject.position;
        
        Vector3 cross = Vector3.Cross(objectDirection, waypointDirection);
        float dot = MathLibrary.DotProduct(cross, movingObject.up);

        if (dot > 0)
        {
            Debug.Log("Turn right");
        }
        else
        {
            Debug.Log("Turn left");
        }
    }
}
