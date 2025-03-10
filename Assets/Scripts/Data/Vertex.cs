using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public Vector3 position;

    //The outgoing halfedge (a halfedge that starts at this vertex)
    //Doesnt matter which edge we connect to it
    private HalfEdge halfEdge;
    public HalfEdge HalfEdge
    {
        get => halfEdge;
        set { halfEdge = value; }
    }

    //Which triangle is this vertex a part of?
    public Triangle triangle;

    //The previous and next vertex this vertex is attached to
    public Vertex prevVertex;
    public Vertex nextVertex;

    //Properties this vertex may have
    public bool isConcave;
    public bool isConvex;
    public bool isEar;

    public string debugText = "";

    public Vertex(Vector3 position)
    {
        this.position = position;
    }

    //Get 2d pos of this vertex
    public Vector2 GetPos2D_XZ()
    {
        Vector2 pos_2d_xz = new Vector2(position.x, position.z);

        return pos_2d_xz;
    }
}