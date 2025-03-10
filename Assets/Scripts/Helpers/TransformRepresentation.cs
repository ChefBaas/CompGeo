using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformRepresentation
{
    public static List<Triangle> HalfEdgesToTriangles(List<HalfEdge> edges)
    {
        List<Triangle> triangles = new List<Triangle>();

        for (int i = 0; i < edges.Count; i += 3)
        {
            triangles.Add(edges[i].t);
        }

        return triangles;
    }

    public static List<HalfEdge> TrianglesToHalfEdges(List<Triangle> triangles)
    {
        // triangles may be facing the wrong direction. not sure when this happens or why it is a problem...
        CheckTriangleOrientation(triangles);

        List<HalfEdge> halfEdges = new List<HalfEdge>();
        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle t = triangles[i];

            // create a new half edge, using the vertex it points to
            HalfEdge he1 = new HalfEdge(t.v1);
            HalfEdge he2 = new HalfEdge(t.v2);
            HalfEdge he3 = new HalfEdge(t.v3);

            he1.nextEdge = he2;
            he2.nextEdge = he3;
            he3.nextEdge = he1;
            he1.prevEdge = he3;
            he2.prevEdge = he1;
            he3.prevEdge = he2;

            // each v contains the half edge originating from it
            he1.v.HalfEdge = he2;
            he2.v.HalfEdge = he3;
            he3.v.HalfEdge = he1;

            t.halfEdge = he1;
            he1.t = t;
            he2.t = t;
            he3.t = t;

            halfEdges.Add(he1);
            halfEdges.Add(he2);
            halfEdges.Add(he3);
        }

        // assign every half edge its opposite
        for (int i = 0; i < halfEdges.Count; i++)
        {
            HalfEdge he = halfEdges[i];
            Vertex from = he.prevEdge.v;
            Vertex to = he.v;

            for (int j = 0; j < halfEdges.Count; j++)
            {
                if (i != j)
                {
                    HalfEdge opposite = halfEdges[j];
                    if (from.position == opposite.v.position && to.position == opposite.prevEdge.v.position)
                    {
                        he.oppositeEdge = opposite;
                        break;
                    }
                }
            }
        }

        return halfEdges;
    }

    public static List<Vector2> Vector3sToVector2s(List<Vector3> v3s)
    {
        List<Vector2> output = new List<Vector2>();
        for (int i = 0; i < v3s.Count; i++)
        {
            output.Add(new Vector2(v3s[i].x, v3s[i].z));
        }
        return output;
    }

    public static List<Vector3> Vector2sToVector3s(List<Vector2> v2s)
    {
        List<Vector3> output = new List<Vector3>();
        for (int i = 0; i < v2s.Count; i++)
        {
            output.Add(new Vector3(v2s[i].x, 0f, v2s[i].y));
        }
        return output;
    }

    public static List<Vertex> Vector3sToVertexList(List<Vector3> v3s)
    {
        List<Vertex> output = new List<Vertex>();

        for (int i = 0; i < v3s.Count; i++)
        {
            Vertex v = new Vertex(v3s[i]);
            output.Add(v);
        }
        
        for (int i = 0; i < output.Count; i++)
        {
            int iMinusOne = MathLibrary.FixListIndex(i - 1, output.Count);
            int iPlusOne = MathLibrary.FixListIndex(i + 1, output.Count);
            output[i].prevVertex = output[iMinusOne];
            output[i].nextVertex = output[iPlusOne];
        }

        return output;
    }

    private static void CheckTriangleOrientation(List<Triangle> triangles)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle t = triangles[i];
            if (MathLibrary.IsTriangleOrientedClockwise(t.v1.position, t.v2.position, t.v3.position))
            {
                t.ChangeOrientation();
            }
        }
    }
}
