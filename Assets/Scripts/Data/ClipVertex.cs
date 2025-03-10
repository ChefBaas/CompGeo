using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Necessary data structure for Greiner-Hormannn algorithm
/// This algorithm allows us to do boolean operations (intersection, union, exclusive or, difference) on two overlapping polygons
/// </summary>
public class ClipVertex
{
    public Vector2 coordinate;

    //Next and previous vertex in the chain that will form a polygon if we walk around it
    public ClipVertex next;
    public ClipVertex prev;

    //We may end up with more than one polygon, and this means we jump to that polygon from this polygon
    public ClipVertex nextPoly;

    //True if this is an intersection vertex
    public bool isIntersection = false;

    //Is an intersect an entry to a neighbor polygon, otherwise its an exit from a polygon
    public bool isEntry;

    //If this is an intersection vertex, then this is the same intersection vertex but on the other polygon
    public ClipVertex neighbor;

    //If this is an intersection vertex, this is how far is is between two vertices that are not intersecting
    public float alpha = 0f;

    //Is this vertex taken by the final polygon, which is more efficient than removing from a list
    //when we create the final polygon
    public bool isTakenByFinalPolygon;

    public ClipVertex(Vector2 coordinate)
    {
        this.coordinate = coordinate;
    }
}