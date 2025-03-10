using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;

    private Transform current, next;
    public Transform Current
    {
        get => current;
    }
    public Transform Next
    {
        get => next;
    }
    private int currentWaypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentWaypoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PassedWaypoint()
    {
        Debug.Log("SUCCESS!");
        currentWaypointIndex++;
        if (currentWaypointIndex == waypoints.Count)
        {
            currentWaypointIndex = 0;
        }
        SetCurrentWaypoints();
    }

    private void SetCurrentWaypoints()
    {
        current = waypoints[currentWaypointIndex];
        next = GetNextWaypoint();
    }

    private Transform GetNextWaypoint()
    {
        if (currentWaypointIndex == waypoints.Count - 1) 
        {
            return waypoints[0];
        }
        else
        {
            return waypoints[currentWaypointIndex + 1];
        }
    }
}
