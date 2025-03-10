using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SutherlandHodgman
{
    /// <summary>
    /// p2 must be a convex polygon, p1 may be concave
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static List<Vector3> ClipPolygon(List<Vector3> p1, List<Vector3> p2)
    {
        List<Plane> clippingPlanes = GetClippingPlanes(p2);

        List<Vector3> vertices = ClipPolygon(p1, clippingPlanes);

        return vertices;
    }

    public static List<Vector3> ClipPolygon(List<Vector3> polygon, List<Plane> planes)
    {
        // clone the polygon list so we can remove safely from this copy, leaving the original polygon untouched
        List<Vector3> vertices = new List<Vector3>(polygon);
        // we save all new vertices in this list
        List<Vector3> temp = new List<Vector3>();

        for (int i = 0; i < planes.Count; i++)
        {
            Plane p = planes[i];

            for (int j = 0; j < vertices.Count; j++)
            {
                int jPlusOne = MathLibrary.FixListIndex(j + 1, vertices.Count);

                Vector3 v1 = vertices[j];
                Vector3 v2 = vertices[jPlusOne];

                // we check where both vertices lie in relation to the plane
                // if the dot if positive, it lies 'in front' of the plane, if its negative its behind, if its 0 its on the plane
                // note this is true regardless of the position we chose earlier, as long as the plane's position is somewhere on the plane
                float relativePositionV1 = Vector3.Dot(p.normal, v1 - p.pos);
                float relativePositionV2 = Vector3.Dot(p.normal, v2 - p.pos);

                // we check four different cases now:

                // case 1: v1 and v2 lie behind the plane. in this case we should ditch both, as they both lay outside our clipping polygon
                // this means we don't have to do anything here
                if (relativePositionV1 < 0f && relativePositionV2 < 0f)
                {
                    // 
                }

                // case 2: v1 and v2 lie in front of the plane. now we should keep the v2 (v1 will added when j has a different value and it is considered to be v2)
                else if (relativePositionV1 > 0f && relativePositionV2 > 0f)
                {
                    temp.Add(v2);
                }

                // case 3: v1 lies behind the plane, v2 in front. v2 should be kept, so we add it, but we also add the intersection point between the plane and the edge formed by v1-v2
                // we actually save the intersection point first to make sure the vertices stay in order
                else if (relativePositionV1 < 0f && relativePositionV2 > 0f)
                {
                    Vector3 direction = (v2 - v1).normalized;
                    Vector3 intersection = MathLibrary.GetRayPlaneIntersectionCoordinate(p.pos, p.normal, v1, direction);
                    temp.Add(intersection);
                    temp.Add(v2);
                }

                // case 4: v1 lies in front of the plane, v2 behind it. only keep the intersection point
                // the same reasoning as with case 2 applies: the vertex represented by v1 will be added when it is represented by v2 sooner or later
                else if (relativePositionV1 > 0f && relativePositionV2 < 0f)
                {
                    Vector3 direction = (v2 - v1).normalized;
                    Vector3 intersection = MathLibrary.GetRayPlaneIntersectionCoordinate(p.pos, p.normal, v1, direction);
                    temp.Add(intersection);
                }
            }

            //Add the new vertices to the list of vertices
            vertices.Clear();

            vertices.AddRange(temp);

            temp.Clear();
        }
        return vertices;
    }

    public static List<Plane> GetClippingPlanes(List<Vector3> polygon)
    {
        // For every edge of the clipping polygon (p2), we define a plane
        // We use each plane to compare to each vertex from p1; any vertex which lies on its inside (left side) should be kept
        List<Plane> clippingPlanes = new List<Plane>();

        for (int i = 0; i < polygon.Count; i++)
        {
            int iPlusOne = MathLibrary.FixListIndex(i + 1, polygon.Count);

            Vector3 v1 = polygon[i];
            Vector3 v2 = polygon[iPlusOne];

            // position doesn't really matter, can be nice to use this for debugging purposes though
            Vector3 planePos = (v1 + v2) * 0.5f;
            // parallel to an edge described by both vertices
            Vector3 planeDir = v2 - v1;
            // normal of the plane, facing inward
            Vector3 planeNormal = new Vector3(-planeDir.z, 0f, planeDir.x).normalized;

            clippingPlanes.Add(new Plane(planePos, planeNormal));
        }

        return clippingPlanes;
    }
}
