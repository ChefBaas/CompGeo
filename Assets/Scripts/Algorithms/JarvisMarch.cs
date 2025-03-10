using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JarvisMarch calculates a convex hull from a list of points
/// </summary>
public static class JarvisMarch
{
    public static List<Vector3> GetConvexHull(List<Vector3> points)
    {
        if (points.Count == 3)
        {
            return points;
        }
        if (points.Count < 3)
        {
            return null;
        }

        List<Vector3> convexHull = new List<Vector3>();

        // find the most bottom-left vertex in the XZ-plane, we start from there
        Vector3 startPoint = points[0];
        for (int i = 1; i < points.Count; i++)
        {
            if (points[i].x < startPoint.x || Mathf.Approximately(points[i].x, startPoint.x) && points[i].z < startPoint.z) 
            {
                startPoint = points[i]; 
            }
        }
        convexHull.Add(startPoint);

        // from the perspective of the start vertex, find the right-most next vertex
        List<Vector3> pointsCopy = new List<Vector3>(points);
        pointsCopy.Remove(startPoint);
        Vector3 nextPoint = GetRightMostPoint(startPoint, pointsCopy);
        convexHull.Add(nextPoint);

        // continue this process while the next vertex is not the vertex we started from
        while (nextPoint != startPoint)
        {
            pointsCopy = new List<Vector3>(points);
            pointsCopy.Remove(nextPoint);
            nextPoint = GetRightMostPoint(nextPoint, pointsCopy);
            if (nextPoint == startPoint)
            {
                break;
            }
            convexHull.Add(nextPoint);
        }

        // DONE :)
        return convexHull;
    }

    // find the point from the points list that is farthest to the left from the perspective of startVertex (when it would look at all points)
    private static Vector3 GetRightMostPoint(Vector3 startVertex, List<Vector3> points)
    {
        int candidateIndex = 0;
        for (int i = 0; i < points.Count; i++)
        {
            if (candidateIndex != i)
            {
                Vector3 candidateVertex = points[candidateIndex];
                float relation = MathLibrary.PointRelativeToLine(new Vector2(startVertex.x, startVertex.z), new Vector2(candidateVertex.x, candidateVertex.z), new Vector2(points[i].x, points[i].z));
                if (Mathf.Approximately(relation, 0f))
                {
                    // colinear stuff
                }
                if (relation < 0f)
                {
                    candidateIndex = i;
                }
            }
        }
        return points[candidateIndex];
    }
}