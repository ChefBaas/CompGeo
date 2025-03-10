using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPoints : PointSource
{
    [SerializeField] private int numberOfPoints;

    private List<Vector3> points = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();

        RandomizePointList();
    }

    public override List<Vector3> GetPoints()
    {
        return new List<Vector3>(points);
    }

    public override List<Vector3> GetNewPoints()
    {
        RandomizePointList();
        return GetPoints();
    }

    private void RandomizePointList()
    {
        points.Clear();
        for (int i = 0; i < numberOfPoints; i++)
        {
            float xRand = Random.Range(min.x, max.x);
            float zRand = Random.Range(min.z, max.z);
            points.Add(new Vector3(xRand, 0f, zRand));
        }
    }
}
