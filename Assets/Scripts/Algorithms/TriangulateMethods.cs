using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriangulateMethods
{
    #region ConvexMethods
    public static List<Triangle> TriangulateConvex(DrawablePolygon p)
    {
        List<Triangle> triangles = new List<Triangle>();
        List<Vector3> points = p.Outline;
        List<Vertex> vertices = TransformRepresentation.Vector3sToVertexList(points);

        for (int i = 2; i < points.Count; i++)
        {
            Vertex a = new Vertex(points[0]);
            Vertex b = new Vertex(points[i - 1]);
            Vertex c = new Vertex(points[i]);

            triangles.Add(new Triangle(a, b, c));
        }

        return triangles;
    }

    /// <summary>
    /// checks whether any vertex from a list is contained in a triangle
    /// if so, it replaces that triangle with three new ones, to include the vertex in the web of triangles
    /// </summary>
    /// <param name="triangles"></param>
    /// <param name="vertices"></param>
    public static void SplitTriangles(List<Triangle> triangles, List<Vector3> points)
    {
        List<Vertex> vertices = TransformRepresentation.Vector3sToVertexList(points);
        for (int i = 0; i < vertices.Count; i++)
        {
            Vertex v = vertices[i];
            Vector2 p = v.GetPos2D_XZ();

            for (int j = 0; j < triangles.Count; j++)
            {
                Triangle t = triangles[j];
                Vertex v1 = triangles[j].v1;
                Vertex v2 = triangles[j].v2;
                Vertex v3 = triangles[j].v3;

                if (!(v == v1 || v == v2 || v == v3))
                {
                    if (MathLibrary.PointIsInTriangle(p, v1.GetPos2D_XZ(), v2.GetPos2D_XZ(), v3.GetPos2D_XZ(), true))
                    {
                        Triangle t1 = new Triangle(v1, v2, v);
                        Triangle t2 = new Triangle(v2, v3, v);
                        Triangle t3 = new Triangle(v3, v1, v);

                        triangles.Remove(t);
                        triangles.Add(t1);
                        triangles.Add(t2);
                        triangles.Add(t3);

                        break;
                    }
                }
            }
        }
    }

    public static void TriangulateByFlippingEdges(List<HalfEdge> halfEdges)
    {
        int safety = 0;

        int flippedEdges = 0;

        while (true)
        {
            safety += 1;

            if (safety > 100000)
            {
                Debug.Log("Stuck in endless loop");

                break;
            }

            bool hasFlippedEdge = false;

            //Search through all edges to see if we can flip an edge
            for (int i = 0; i < halfEdges.Count; i++)
            {
                HalfEdge thisEdge = halfEdges[i];

                //Is this edge sharing an edge, otherwise its a border, and then we cant flip the edge
                if (thisEdge.oppositeEdge == null)
                {
                    continue;
                }

                //The vertices belonging to the two triangles, c-a forms the shared edge, b belongs to this triangle
                Vertex a = thisEdge.v;
                Vertex b = thisEdge.nextEdge.v;
                Vertex c = thisEdge.prevEdge.v;
                Vertex d = thisEdge.oppositeEdge.nextEdge.v;

                Vector2 aPos = a.GetPos2D_XZ();
                Vector2 bPos = b.GetPos2D_XZ();
                Vector2 cPos = c.GetPos2D_XZ();
                Vector2 dPos = d.GetPos2D_XZ();

                //Use the circle test to test if we need to flip this edge
                if (MathLibrary.PointIsInCircle(aPos, bPos, cPos, dPos) > 0f)
                {
                    // Debug.Log("Check 1");
                    //Are these the two triangles that share this edge forming a convex quadrilateral?
                    //Otherwise the edge cant be flipped
                    if (MathLibrary.IsQuadrilateralConvex(aPos, bPos, cPos, dPos))
                    {
                        // Debug.Log("Check 2");
                        //If the new triangle after a flip is not better, then dont flip
                        //This will also stop the algoritm from ending up in an endless loop
                        if (MathLibrary.PointIsInCircle(bPos, cPos, dPos, aPos) > 0f)
                        {
                            continue;
                        }

                        //Flip the edge
                        flippedEdges += 1;

                        hasFlippedEdge = true;

                        FlipEdge(thisEdge);
                    }
                }
            }

            //We have searched through all edges and havent found an edge to flip, so we have a Delaunay triangulation!
            if (!hasFlippedEdge)
            {
                break;
            }
        }
    }

    private static void FlipEdge(HalfEdge one)
    {
        //The data we need
        //This edge's triangle
        HalfEdge two = one.nextEdge;
        HalfEdge three = one.prevEdge;
        //The opposite edge's triangle
        HalfEdge four = one.oppositeEdge;
        HalfEdge five = one.oppositeEdge.nextEdge;
        HalfEdge six = one.oppositeEdge.prevEdge;
        //The vertices
        Vertex a = one.v;
        Vertex b = one.nextEdge.v;
        Vertex c = one.prevEdge.v;
        Vertex d = one.oppositeEdge.nextEdge.v;



        //Flip

        //Change vertex
        a.HalfEdge = one.nextEdge;
        c.HalfEdge = one.oppositeEdge.nextEdge;

        //Change half-edge
        //Half-edge - half-edge connections
        one.nextEdge = three;
        one.prevEdge = five;

        two.nextEdge = four;
        two.prevEdge = six;

        three.nextEdge = five;
        three.prevEdge = one;

        four.nextEdge = six;
        four.prevEdge = two;

        five.nextEdge = one;
        five.prevEdge = three;

        six.nextEdge = two;
        six.prevEdge = four;

        //Half-edge - vertex connection
        one.v = b;
        two.v = b;
        three.v = c;
        four.v = d;
        five.v = d;
        six.v = a;

        //Half-edge - triangle connection
        Triangle t1 = one.t;
        Triangle t2 = four.t;

        one.t = t1;
        three.t = t1;
        five.t = t1;

        two.t = t2;
        four.t = t2;
        six.t = t2;

        //Opposite-edges are not changing!

        //Triangle connection
        t1.v1 = b;
        t1.v2 = c;
        t1.v3 = d;

        t2.v1 = b;
        t2.v2 = d;
        t2.v3 = a;

        t1.halfEdge = three;
        t2.halfEdge = four;
    }

    #endregion

    #region ConcaveMethods

    // DOESN'T YET WORK FOR SELF-INTERSECTING SHAPES
    public static List<Triangle> TriangulateConcave(DrawablePolygon p)
    {
        List<Triangle> triangles = new List<Triangle>();
        List<Vector3> points = p.Outline;
        List<Vertex> hull = TransformRepresentation.Vector3sToVertexList(points);

        if (hull.Count == 3)
        {
            triangles.Add(new Triangle(hull[0], hull[1], hull[2]));
            return triangles;
        }

        for (int i = 0; i < hull.Count; i++)
        {
            CheckVertexConcave(hull[i]);
        }

        List<Vertex> ears = new List<Vertex>();
        for (int i = 0; i < hull.Count; i++)
        {
            CheckVertexEar(hull[i], hull, ears);
        }

        while (hull.Count > 3)
        {
            Vertex ear = ears[0];
            Vertex prev = ear.prevVertex;
            Vertex next = ear.nextVertex;

            triangles.Add(new Triangle(prev, ear, next));

            hull.Remove(ear);
            ears.Remove(ear);

            prev.nextVertex = next;
            next.prevVertex = prev;

            ears.Remove(prev);
            ears.Remove(next);

            CheckVertexConcave(prev);
            CheckVertexConcave(next);
            CheckVertexEar(prev, hull, ears);
            CheckVertexEar(next, hull, ears);
        }
        triangles.Add(new Triangle(hull[0], hull[1], hull[2]));

        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle triangle = triangles[i];
            // Debug.Log(MathLibrary.IsTriangleOrientedClockwise(triangle.v1.GetPos2D_XZ(), triangle.v2.GetPos2D_XZ(), triangle.v3.GetPos2D_XZ()));
        }

        return triangles;
    }

    private static void CheckVertexConcave(Vertex v)
    {
        v.isConvex = false;
        v.isConcave = false;

        if (MathLibrary.IsTriangleOrientedClockwise(v.prevVertex.GetPos2D_XZ(), v.GetPos2D_XZ(), v.nextVertex.GetPos2D_XZ()))
        {
            v.isConcave = true;
        }
        else
        {
            v.isConvex = true;
        }
    }

    private static void CheckVertexEar(Vertex v, List<Vertex> vertices, List<Vertex> ears)
    {
        if (v.isConcave)
        {
            // Debug.LogFormat("Vertex {0} is concave", v.debugText);
            return;
        }
        // Debug.LogFormat("Vertex {0} is convex", v.debugText);

        Vector2 a = v.prevVertex.GetPos2D_XZ();
        Vector2 b = v.GetPos2D_XZ();
        Vector2 c = v.nextVertex.GetPos2D_XZ();

        bool hasPointInside = false;
        for (int i = 0; i < vertices.Count; i++)
        {
            if (vertices[i].isConcave)
            {
                Vector2 p = vertices[i].GetPos2D_XZ();

                if (MathLibrary.PointIsInTriangle(p, a, b, c, false))
                {
                    // Debug.LogFormat("Vertex {0} is not an ear. It contains {1}", v.debugText, vertices[i].debugText);
                    hasPointInside = true;
                }
            }
        }
        if (!hasPointInside)
        {
            ears.Add(v);
        }
    }
    #endregion
}
