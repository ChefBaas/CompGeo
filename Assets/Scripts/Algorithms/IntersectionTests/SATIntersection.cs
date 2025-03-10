using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// To check collision between two convex shapes we use the Separating Axis Theorem
/// First we collect all (unique) normals for each edge of both shapes
/// Then we project both shapes on each normal
/// If there is at least one normal for which the projections don't overlap, the shapes don't overlap
/// In that case we can always draw a line parallel to the edge belonging to that normal (the Separating Axis) that separates both shapes
/// </summary>
public static class SATIntersection
{
    public static bool Intersect(DrawablePolygon p1, DrawablePolygon p2, out Vector2 smallestOverlap)
    {
        List<Vector2> normals = new List<Vector2>();
        normals.AddRange(p1.GetNormals(true));
        normals.AddRange(p2.GetNormals(true));

        float smallestOverlapDistance = Mathf.Infinity;
        Vector2 smallestOverlapVector;
        smallestOverlap = Vector2.zero;

        for (int i = 0; i < normals.Count; i++)
        {
            float overlapDistance;
            if (IsOverlapping(normals[i], p1, p2, out overlapDistance))
            {
                if (overlapDistance < smallestOverlapDistance)
                {
                    smallestOverlapDistance = overlapDistance;
                    smallestOverlapVector = normals[i];
                    smallestOverlap = smallestOverlapVector * smallestOverlapDistance;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsOverlapping(Vector2 normal, DrawablePolygon p1, DrawablePolygon p2, out float overlapDistance)
    {
        float min1, max1, min2, max2;
        GetMinMaxDot(normal, p1, out min1, out max1);
        GetMinMaxDot(normal, p2, out min2, out max2);

        overlapDistance = Mathf.Min(Mathf.Abs(max1 - min2), Mathf.Abs(max1 - min2));
        if (min1 <= max2 && min2 <= max1)
        {
            return true;
        }
        return false;
    }

    private static void GetMinMaxDot(Vector2 normal, DrawablePolygon p, out float min, out float max)
    {
        List<Vector2> corners = p.GetV2Outline();
        min = Mathf.Infinity;
        max = Mathf.NegativeInfinity;

        for (int i = 0; i < corners.Count; i++)
        {
            float dot = MathLibrary.DotProduct(normal, corners[i]);
            if (dot < min)
            {
                min = dot;
            }
            if (dot > max)
            {
                max = dot;
            }
        }
    }
}
