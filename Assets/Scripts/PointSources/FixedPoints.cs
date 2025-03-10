using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPoints : PointSource
{
    private List<Vector3> points = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();

        points.Add(new Vector3(min.x * 0.8f, 0f, min.y * 0.8f));
        points.Add(new Vector3(min.x * 0.8f, 0f, max.y * 0.8f));
        points.Add(new Vector3(max.x * 0.8f, 0f, min.y * 0.8f));
        points.Add(new Vector3(max.x * 0.8f, 0f, max.y * 0.8f));

        points.Add(new Vector3(min.x * 0.25f, 0f, min.y * 0.35f));
        points.Add(new Vector3(min.x * 0.35f, 0f, max.y * 0.25f));
        points.Add(new Vector3(max.x * 0.25f, 0f, min.y * 0.35f));
        points.Add(new Vector3(max.x * 0.35f, 0f, max.y * 0.25f));
    }

    public override List<Vector3> GetPoints()
    {
        return new List<Vector3>(points);
    }

    public override List<Vector3> GetNewPoints()
    {
        return GetPoints();
    }
}
