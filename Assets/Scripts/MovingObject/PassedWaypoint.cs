using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassedWaypoint : MonoBehaviour
{
    [SerializeField] private Transform movingObject;
    [SerializeField] private WaypointManager waypointManager;

    // Update is called once per frame
    void Update()
    {
        Vector3 a = movingObject.transform.position - waypointManager.Current.position;
        Vector3 b = waypointManager.Next.position - waypointManager.Current.position;

        if (MathLibrary.ProjectVector(a, b) > 1f) 
        {
            waypointManager.PassedWaypoint();
        }
    }
}
