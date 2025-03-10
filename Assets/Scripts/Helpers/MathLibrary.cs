using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class MathLibrary
{
    public enum bool3
    {
        True = 1,
        False = 0,
        Unknown = -1
    }

    public static bool LinesIntersect(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out Vector2 intersection)
    {
        Vector2 dir1 = (end1 - start1).normalized;
        Vector2 dir2 = (end2 - start2).normalized;

        Vector2 normal1 = new Vector2(-dir1.y, dir1.x);
        Vector2 normal2 = new Vector2(-dir2.y, dir2.x);

        // write both lines in the general form ax + by = d
        float a1 = normal1.x;
        float b1 = normal1.y;
        float a2 = normal2.x;
        float b2 = normal2.y;
        float d1 = a1 * start1.x + b1 * start1.y; // value of d should be the same as long as pick any point on the line
        float d2 = a2 * start2.x + b2 * start2.y; // d should also be the shortest distance from the line to the origin point (0,0), assuming normalized normals

        // check if lines are parallel, if so no solution
        if (Vector2.Angle(normal1, normal2) == 0f || Vector2.Angle(normal1, normal2) == 180f)
        {
            intersection = Vector2.zero;
            return false;
        }

        // check if line contains both start points
        // if so return false?
        // tutorial says this checks whether lines are the same, but i dont believe that's true
        /*if (Mathf.Abs(Vector2.Dot(start1 - start2, normal1)) < 0.000001f)
        {
            Debug.Log("HALLO");
            intersection = Vector2.zero;
            return false;
        }*/

        // what is happening here?
        float x = (normal2.y * d1 - normal1.y * d2) / (normal1.x * normal2.y - normal2.x * normal1.y);
        float y = (normal1.x * d2 - normal2.x * d1) / (normal1.x * normal2.y - normal2.x * normal1.y);

        // could check whether the lines overlap, but we don't bother
        intersection = new Vector2(x, y);
        return true;
    }

    public static bool3 LinepiecesIntersect(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out Vector2 intersection)
    {
        if (LinesIntersect(start1, end1, start2, end2, out intersection))
        {
            Vector2 ac1 = intersection - start1;
            Vector2 ac2 = intersection - start2;

            if (IsBetween(intersection.x, start1.x, end1.x, false) && IsBetween(intersection.y, start1.y, end1.y, false) && IsBetween(intersection.x, start2.x, end2.x, false) && IsBetween(intersection.y, start2.y, end2.y, false))
            {
                return bool3.True;
            }

            if (Mathf.Approximately(ac1.sqrMagnitude, 0f) || Mathf.Approximately(ac2.sqrMagnitude, 0f))
            {
                return bool3.Unknown;
            }
        }
        intersection = Vector2.zero;
        return bool3.False;
    }

    // just replace this with actualy Vector3.Dot?
    public static float DotProduct(Vector3 v1, Vector3 v2)
    {
        return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
    }

    public static float ProjectVector(Vector3 v1, Vector3 v2)
    {
        return DotProduct(v1, v2) / v2.sqrMagnitude;
    }

    public static float Min(List<float> values)
    {
        float min = values[0];
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] < min)
            {
                min = values[i];
            }
        }
        return min;
    }

    public static float Max(List<float> values)
    {
        float max = values[0];
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] > max)
            {
                max = values[i];
            }
        }
        return max;
    }

    public static bool IsBetween(float value, float n1, float n2, bool equalReturnsTrue)
    {
        float min, max;
        if (n2 < n1)
        {
            min = n2;
            max = n1;
        }
        else
        {
            min = n1;
            max = n2;
        }

        if (equalReturnsTrue)
        {
            return min <= value && value <= max;
        }
        else
        {
            return min < value && value < max;
        }
    }

    public static Vector2 GetNormal(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        return new Vector2(-direction.y, direction.x);
    }

    //Where is point in relation to start-end
    // < 0 -> to the right
    // = 0 -> on the line
    // > 0 -> to the left
    public static float PointRelativeToLine(Vector2 start, Vector2 end, Vector2 point)
    {
        return (start.x - point.x) * (end.y - point.y) - (start.y - point.y) * (end.x - point.x);
    }
    
    // We convert the triangle to a barycentric coordinate system (http://totologic.blogspot.com/2014/01/accurate-point-in-triangle-test.html)
    // This results in three scalar 'weights' a, b and c
    // We can use these weights to change the representation of p from (x, y) into a * (x1, y1) + b (x2, y2) + c (x3, y3)
    // Now, if a, b and c are all between 0 and 1, p lies inside the triangle
    public static bool PointIsInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c, bool equalsReturnsTrue)
    {
        float weightA = ((b.y - c.y) * (p.x - c.x) + (c.x - b.x) * (p.y - c.y)) / ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
        float weightB = ((c.y - a.y) * (p.x - c.x) + (a.x - c.x) * (p.y - c.y)) / ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
        float weightC = 1 - weightA - weightB;

        return IsBetween(weightA, 0f, 1f, equalsReturnsTrue) && IsBetween(weightB, 0f, 1f, equalsReturnsTrue) && IsBetween(weightC, 0f, 1f, equalsReturnsTrue);
    }

    // We check whether d is inside, on, or outside a circle on which a, b and c lay
    // Returns positive if inside, negative if outside and 0 if on the circle
    public static float PointIsInCircle(Vector2 aVec, Vector2 bVec, Vector2 cVec, Vector2 dVec)
    {
        float a = aVec.x - dVec.x;
        float d = bVec.x - dVec.x;
        float g = cVec.x - dVec.x;

        float b = aVec.y - dVec.y;
        float e = bVec.y - dVec.y;
        float h = cVec.y - dVec.y;

        float c = a * a + b * b;
        float f = d * d + e * e;
        float i = g * g + h * h;

        return (a * e * i) + (b * f * g) + (c * d * h) - (g * e * c) - (h * f * a) - (i * d * b);
    }

    public static bool PointIsInPolygon(List<Vector2> points, Vector2 p)
    {
        // create a point that lies to the right of the polygon (so we know it is always outside of it)
        Vector2 maxXPosVertex = points[0];
        for (int i = 1; i < points.Count; i++)
        {
            if (points[i].x > maxXPosVertex.x)
            {
                maxXPosVertex = points[i];
            }
        }
        Vector2 pointOutside = maxXPosVertex + Vector2.right * 10f;

        // check how many intersections we get with the polygon when drawing a line from p to the point that is always outside our polygon
        // if this amount is even, the point lies outside the polygon
        int nIntersections = 0;
        for (int i = 0; i < points.Count; i++)
        {
            int iPlusOne = FixListIndex(i + 1, points.Count);
            Vector2 p1 = points[i];
            Vector2 p2 = points[iPlusOne];
            Vector2 intersection;

            if (LinepiecesIntersect(p, pointOutside, p1, p2, out intersection) == bool3.True)
            {
                nIntersections++;
            }
            else if (LinepiecesIntersect(p, pointOutside, p1, p2, out intersection) == bool3.Unknown)
            {
                Debug.Log("Ambiguous shit going on");
                nIntersections = 0;
                i = -1;
                pointOutside += Vector2.up;
            }
        }

        return !(nIntersections % 2 == 0);
    }

    /// <summary>
    /// for each point, we check whether it would be inside a triangle formed by the other three
    /// if any point is, then the form is concave
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool IsQuadrilateralConvex(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        /*if (!PointIsInTriangle(d, a, b, c, false))
        {
            if (!PointIsInTriangle(c, a, b, d, false))
            {
                if (!PointIsInTriangle(b, a, c, d, false))
                {
                    if (!PointIsInTriangle(a, b, c, d, false))
                    {
                        return true;
                    }
                }
            }
        }
        return false;*/

        bool isConvex = false;

        bool abc = IsTriangleOrientedClockwise(a, b, c);
        bool abd = IsTriangleOrientedClockwise(a, b, d);
        bool bcd = IsTriangleOrientedClockwise(b, c, d);
        bool cad = IsTriangleOrientedClockwise(c, a, d);

        if (abc && abd && bcd & !cad)
        {
            isConvex = true;
        }
        else if (abc && abd && !bcd & cad)
        {
            isConvex = true;
        }
        else if (abc && !abd && bcd & cad)
        {
            isConvex = true;
        }
        //The opposite sign, which makes everything inverted
        else if (!abc && !abd && !bcd & cad)
        {
            isConvex = true;
        }
        else if (!abc && !abd && bcd & !cad)
        {
            isConvex = true;
        }
        else if (!abc && abd && !bcd & !cad)
        {
            isConvex = true;
        }


        return isConvex;
    }

    public static bool IsTriangleOrientedClockwise(Triangle t)
    {
        return IsTriangleOrientedClockwise(t.v1.GetPos2D_XZ(), t.v2.GetPos2D_XZ(), t.v3.GetPos2D_XZ());
    }

    //Is a triangle in 2d space oriented clockwise or counter-clockwise
    //https://math.stackexchange.com/questions/1324179/how-to-tell-if-3-connected-points-are-connected-clockwise-or-counter-clockwise
    //https://en.wikipedia.org/wiki/Curve_orientation
    public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y < 0f;
    }

    public static bool IsPolygonOrientedClockwise(List<Vector2> points)
    {
        float total = 0;
        for (int i = 0; i < points.Count; i++)
        {
            int iPlusOne = FixListIndex(i + 1, points.Count);
            total += (points[iPlusOne].x - points[i].x) * (points[iPlusOne].y + points[i].y);
        }

        return total > 0;
    }

    public static bool IsPolygonOrientedClockwise(List<Vector3> points)
    {
        List<Vector2> vector2s = TransformRepresentation.Vector3sToVector2s(points);
        return IsPolygonOrientedClockwise(vector2s);
    }

    public static int FixListIndex(int current, int listLength)
    {
        if (current >= listLength)
        {
            return 0;
        }
        if (current < 0)
        {
            return listLength - 1;
        }
        return current;
    }

    // checks whether points c and d are on opposite sides of a line from a to b
    // see here: https://www.habrador.com/tutorials/math/5-line-line-intersection/
    // we use this to check whether two line pieces intersect at some point:
    //      in the line intersection check we get the hypothetical point where they intersect were they infinite lines
    //      then we check for both linepieces if their start and end point are on opposite sides of the other line
    private static bool PointsOnDifferentSide(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        Vector2 line = b - a;
        Vector2 normal = new Vector2(-line.y, line.x);

        float dot1 = Vector2.Dot(normal, c - a);
        float dot2 = Vector2.Dot(normal, d - a);

        if (dot1 * dot2 < 0f)
        {
            return true;
        }
        return false;
    }

    //Get the coordinate if we know a ray-plane is intersecting
    public static Vector3 GetRayPlaneIntersectionCoordinate(Vector3 planePos, Vector3 planeNormal, Vector3 rayStart, Vector3 rayDir)
    {
        float denominator = Vector3.Dot(-planeNormal, rayDir);

        Vector3 vecBetween = planePos - rayStart;

        float t = Vector3.Dot(vecBetween, -planeNormal) / denominator;

        Vector3 intersectionPoint = rayStart + rayDir * t;

        return intersectionPoint;
    }
}