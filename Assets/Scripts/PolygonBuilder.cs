using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonBuilder : MonoBehaviour
{
    public DrawablePolygon BuildPolygon(List<Vector3> points, Color displayColor, bool guaranteeConvex = false)
    {
        if (!guaranteeConvex)
        {
            return new DrawablePolygon(points, new List<Vector3>(), displayColor);
        }

        List<Vector3> convexHull = JarvisMarch.GetConvexHull(points);
        List<Vector3> otherPoints = points.Where(x => !convexHull.Contains(x)).ToList();
        if (otherPoints.Equals(null))
        {
            Debug.LogError("SHOULD NEVER BE NULL");
        }
        return new DrawablePolygon(convexHull, otherPoints, displayColor);
    }
}
