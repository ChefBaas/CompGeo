using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

public class FreePoints : PointSource
{
    [SerializeField] private List<Vector3> points;
    public List<Vector3> Points
    {
        get => points;
    }

    public override List<Vector3> GetPoints()
    {
        return points;
    }

    public override List<Vector3> GetNewPoints()
    {
        return GetPoints();
    }
}
