using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    //Corners
    public Vertex v1;
    public Vertex v2;
    public Vertex v3;

    //If we are using the half edge mesh structure, we just need one half edge
    public HalfEdge halfEdge;

    public Triangle(Vertex v1, Vertex v2, Vertex v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        this.v1 = new Vertex(v1);
        this.v2 = new Vertex(v2);
        this.v3 = new Vertex(v3);
    }

    public Triangle(HalfEdge halfEdge)
    {
        this.halfEdge = halfEdge;
    }

    //Change orientation of triangle from cw -> ccw or ccw -> cw
    public void ChangeOrientation()
    {
        Vertex temp = v1;

        v1 = v2;

        v2 = temp;
    }

    public List<Vector2> GetLines()
    {
        return new List<Vector2>() { v1.GetPos2D_XZ(), v2.GetPos2D_XZ(), v2.GetPos2D_XZ(), v3.GetPos2D_XZ(), v3.GetPos2D_XZ(), v1.GetPos2D_XZ()};
    }

    public List<Vector2> GetPoints()
    {
        return new List<Vector2>() { v1.GetPos2D_XZ(), v2.GetPos2D_XZ(), v3.GetPos2D_XZ() };
    }

    public void GetAABB(out Vector2 bl, out Vector2 tr)
    {
        float blX = Mathf.Infinity;
        float blY = Mathf.Infinity;
        float trX = Mathf.NegativeInfinity;
        float trY = Mathf.NegativeInfinity;

        List<Vector2> points = GetPoints();
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p = points[i];
            if (p.x < blX) blX = p.x;
            if (p.x > trX) trX = p.x;
            if (p.y < blY) blY = p.y;
            if (p.y > trY) trY = p.y;
        }

        bl = new Vector2(blX, blY);
        tr = new Vector2(trX, trY);
    }

    public void Print()
    {
        Debug.LogFormat("Triangle has points {0}, {1} and {2}", v1.position, v2.position, v3.position);
    }
}