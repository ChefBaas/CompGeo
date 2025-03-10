using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Greiner-Hormann calculates various clipped areas between two concave polygons
/// We can ask it for various boolean operations bla bad text lol
/// </summary>
public static class GreinerHormann
{
    public enum BooleanOperation
    {
        Intersection,
        Difference,
        ExclusiveOr,
        Union
    }

    #region StepByStep

    public static List<Vector2> polyVector, clipPolyVector;
    public static List<ClipVertex> poly, clipPoly, intersectionVertices;
    public static List<List<Vector2>> result = new List<List<Vector2>>();

    // stap 1 = data converteren
    // stap 2 = intersections vinden
    // stap 3 = entry/exit marken
    // stap 4 = polygons tracen

    public static void SetData(List<Vector2> poly1, List<Vector2> poly2)
    {
        polyVector = poly1;
        clipPolyVector = poly2;

        poly = CreateClipVertexList(polyVector);
        clipPoly = CreateClipVertexList(clipPolyVector);

        intersectionVertices = poly;
    }

    public static bool FindIntersections()
    {
        bool polygonsIntersect = false;
        for (int i = 0; i < poly.Count; i++)
        {
            ClipVertex current = poly[i];
            int iPlusOne = MathLibrary.FixListIndex(i + 1, poly.Count);
            Vector2 a = current.coordinate;
            Vector2 b = poly[iPlusOne].coordinate;

            for (int j = 0; j < clipPoly.Count; j++)
            {
                int jPlusOne = MathLibrary.FixListIndex(j + 1, clipPoly.Count);
                Vector2 c = clipPoly[j].coordinate;
                Vector2 d = clipPoly[jPlusOne].coordinate;

                Vector2 intersection;
                if (MathLibrary.LinepiecesIntersect(a, b, c, d, out intersection) == MathLibrary.bool3.True)
                {
                    polygonsIntersect = true;

                    // here we insert a vertex into both linked vertex structures (two different vertices)
                    ClipVertex polyVertex = InsertIntersectionVertex(a, b, intersection, current);
                    ClipVertex clipVertex = InsertIntersectionVertex(c, d, intersection, clipPoly[j]);

                    // and link them together
                    polyVertex.neighbor = clipVertex;
                    clipVertex.neighbor = polyVertex;
                }
            }
        }

        return polygonsIntersect;
    }

    public static void MarkIntersectionsAsEntryExit()
    {
        MarkEntryExit(poly, clipPolyVector);
        MarkEntryExit(clipPoly, polyVector);
    }

    public static void TracePolygon()
    {
        // for now we only calculate the intersection of both polygons
        // later we can add other boolean operations, such as Difference, XOr and Union
        intersectionVertices = GetClippedPolygon(poly, true);
        AddPolygonToList(intersectionVertices, result, false);
    }

    public static void Reset()
    {
        polyVector = null;
        poly = null;
        clipPolyVector = null;
        clipPoly = null;
        intersectionVertices = null;
        result.Clear();
    }

    #endregion

    public static List<List<Vector2>> ClipPolygons(List<Vector2> polyVector, List<Vector2> clipPolyVector)
    {
        List<List<Vector2>> output = new List<List<Vector2>>();

        // first create the correct data structure
        // greiner-hormann uses 'enhanced' vertices (we track/need more info)
        List<ClipVertex> poly = CreateClipVertexList(polyVector);
        List<ClipVertex> clipPoly = CreateClipVertexList(clipPolyVector);

        // next we find the intersection points between both polygons and register them as neighbours for their neighbouring vertices
        // we don't add them to the poly lists, we keep those as they are
        // we also track whether there is any intersection; there is a special case if there aren't. either one polygon completely envelops the other (special case) or they simply don't intersect
        bool polygonsIntersect = false;
        for (int i = 0; i < poly.Count; i++)
        {
            ClipVertex current = poly[i];
            int iPlusOne = MathLibrary.FixListIndex(i + 1, poly.Count);
            Vector2 a = current.coordinate;
            Vector2 b = poly[iPlusOne].coordinate;

            for (int j = 0; j < clipPoly.Count; j++)
            {
                int jPlusOne = MathLibrary.FixListIndex(j + 1, clipPoly.Count);
                Vector2 c = clipPoly[j].coordinate;
                Vector2 d = clipPoly[jPlusOne].coordinate;

                Vector2 intersection;
                if (MathLibrary.LinepiecesIntersect(a, b, c, d, out intersection) == MathLibrary.bool3.True)
                {
                    polygonsIntersect = true;

                    // here we insert a vertex into both linked vertex structures (two different vertices)
                    ClipVertex polyVertex = InsertIntersectionVertex(a, b, intersection, current);
                    ClipVertex clipVertex = InsertIntersectionVertex(c, d, intersection, clipPoly[j]);

                    // and link them together
                    polyVertex.neighbor = clipVertex;
                    clipVertex.neighbor = polyVertex;
                }
            }
        }

        if (polygonsIntersect)
        {
            // we check all clip vertices and mark them as entry/exit points
            // a vertex is an entry/exit point if it lies on an edge of the other polygon
            MarkEntryExit(poly, clipPolyVector);
            MarkEntryExit(clipPoly, polyVector);

            // for now we only calculate the intersection of both polygons
            // later we can add other boolean operations, such as Difference, XOr and Union
            List<ClipVertex> intersectionVertices = GetClippedPolygon(poly, true);
            AddPolygonToList(intersectionVertices, output, false);
            
        }
        // check whether a polygon is inside the other, otherwise the polygons don't intersect
        else
        {
            if (MathLibrary.PointIsInPolygon(polyVector, clipPolyVector[0]))
            {
                Debug.Log("ClipPoly is inside poly");
            }
            else if (MathLibrary.PointIsInPolygon(clipPolyVector, polyVector[0]))
            {
                Debug.Log("Poly is insicde clipPoly");
            }
            else
            {
                Debug.Log("Polys dont overlap");
            }
        }

        return output;
    }

    //Create the data structure needed
    private static List<ClipVertex> CreateClipVertexList(List<Vector2> polyVector)
    {
        List<ClipVertex> poly = new List<ClipVertex>();

        // create all clip vertices
        for (int i = 0; i < polyVector.Count; i++)
        {
            poly.Add(new ClipVertex(polyVector[i]));
        }

        //Connect the clip vertices to each other
        for (int i = 0; i < poly.Count; i++)
        {
            int iPlusOne = MathLibrary.FixListIndex(i + 1, poly.Count);
            int iMinusOne = MathLibrary.FixListIndex(i - 1, poly.Count);

            poly[i].next = poly[iPlusOne];
            poly[i].prev = poly[iMinusOne];
        }

        return poly;
    }

    /// <summary>
    /// creates a new vertex and registers it with its neighbours in the polygon structure
    /// does NOT add the vertex to the polygon list
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="intersection"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private static ClipVertex InsertIntersectionVertex(Vector2 a, Vector2 b, Vector2 intersection, ClipVertex current)
    {
        // alpha represents the new vertex position between a and b; 0 is a's position, 1 is b's
        // its position is a + (a - b) * alpha
        float alpha = (a - intersection).sqrMagnitude / (a - b).sqrMagnitude;

        ClipVertex intersectionVertex = new ClipVertex(intersection);
        intersectionVertex.alpha = alpha;
        intersectionVertex.isIntersection = true;

        // the new vertex lies between the current vertex and the next
        // however, we could have multiple intersection vertices between current and the next, so we need to make sure we insert this vertex in the correct position
        ClipVertex insertAfterVertex = current;
        int safety = 0;
        while (true)
        {
            // so we find the next vertex that is either:
            //      not an intersection (in this case we've come across the final vertex on this edge)
            //      or whose alpha is bigger than that of the new vertex (in this case we know the new vertex needs to come before this one)
            if (insertAfterVertex.next.alpha > alpha || !insertAfterVertex.next.isIntersection)
            {
                break;
            }

            // if neither is true, we need to move along the edge to the next vertex
            // note that this can never be a non-intersection point
            insertAfterVertex = insertAfterVertex.next;

            safety++;
            if (safety > 10000)
            {
                Debug.LogError("STUCK IN LOOP!!!");
                break;
            }
        }

        // finally we insert the new vertex into the linked vertices data structure
        intersectionVertex.next = insertAfterVertex.next;
        intersectionVertex.prev = insertAfterVertex;
        insertAfterVertex.next.prev = intersectionVertex;
        insertAfterVertex.next = intersectionVertex;

        return intersectionVertex;
    }

    /// <summary>
    /// Mark every ClipVertex that is an intersection as an entry or exit point
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="vectorsOther"></param>
    private static void MarkEntryExit(List<ClipVertex> vertices, List<Vector2> vectorsOther)
    {
        // note that the list doesn't contain any intersection vertices yet
        // so unless the first vertex happens to lie exactly on the edge of the other polygon, it is never ambiguous whether it lies in- or outside the other polygon
        ClipVertex currentVertex = vertices[0];
        ClipVertex firstVertex = currentVertex;
        bool isInside = MathLibrary.PointIsInPolygon(vectorsOther, currentVertex.coordinate);
        // Debug.LogFormat("Point at {0} is inside? {1}", currentVertex.coordinate, isInside);
        int safety = 0;

        while (true)
        {
            if (currentVertex.isIntersection)
            {
                // if we are currently inside, this vertex marks an exit point and vice versa
                currentVertex.isEntry = isInside ? false : true;
                // we know that at every intersection whether we are in- or outside changes
                isInside = !isInside;
            }

            currentVertex = currentVertex.next;
            if (currentVertex == firstVertex)
            {
                break;
            }

            safety++;
            if (safety > 10000)
            {
                Debug.LogFormat("STUCK IN LOOP!!!");
            }
        }
    }

    private static List<ClipVertex> GetClippedPolygon(List<ClipVertex> poly, bool getIntersectionPolygon)
    {
        List<ClipVertex> output = new List<ClipVertex>();

        // why it this necessary? explain pls
        // ResetVertices(poly);

        // find an entry point; regardless of the boolean operation, we always start at some intersection
        ClipVertex thisVertex = FindFirstEntryVertex(poly);
        // save the start vertex so we know when to stop the algorithm
        ClipVertex firstVertex = thisVertex;

        output.Add(firstVertex);

        // instead of removing these vertices from their lists we mark them as 'used'
        // this is more efficient than removing them from their lists
        thisVertex.isTakenByFinalPolygon = true;
        thisVertex.neighbor.isTakenByFinalPolygon = true;

        // depending on the boolean operation we move forward or backward through our polygons
        // not sure why, would be nice to know
        bool isMovingForward = getIntersectionPolygon ? true : false;

        thisVertex = isMovingForward ? thisVertex.next : thisVertex.prev;

        int safety = 0;
        while (true)
        {
            // if this is true we're back where we started
            if (thisVertex.Equals(firstVertex) || (thisVertex.neighbor != null && thisVertex.neighbor.Equals(firstVertex)))
            {
                // if there are any unused entry vertices there must be another polygon we need to add
                ClipVertex nextVertex = FindFirstEntryVertex(poly);

                // if there aren't we stop the algorithm
                if (nextVertex == null)
                {
                    break;
                }
                // else we start tracing again from a new point
                else
                {
                    // separate output polygons are connected to each other via nextPoly
                    // the last vertex of our last polygon references the first vertex of the polygon we are currently tracing
                    output[output.Count - 1].nextPoly = nextVertex;

                    thisVertex = nextVertex;
                    firstVertex = nextVertex;

                    output.Add(thisVertex);
                    thisVertex.isTakenByFinalPolygon = true;
                    thisVertex.neighbor.isTakenByFinalPolygon = true;

                    // depending on the boolean operation isMovingForward might change
                    // if it has, we need to reset it here
                    isMovingForward = getIntersectionPolygon ? true : false;
                    thisVertex = isMovingForward ? thisVertex.next : thisVertex.prev;
                }
            }

            // if the vertex is not an intersection, we can simply keep going
            // remember we are always tracing one polygon or another, so rn we are simply walking along the edge of some polygon
            // until we encounter an intersection, nothing exciting needs to happen
            if (!thisVertex.isIntersection)
            {
                output.Add(thisVertex);
                thisVertex = isMovingForward ? thisVertex.next : thisVertex.prev;
            }
            // if we do find an intersection we have to continue tracing the other polygon
            else
            {
                // mark this intersection as used, so that it doesn't get picked up as an unused entry point later
                thisVertex.isTakenByFinalPolygon = true;
                thisVertex.neighbor.isTakenByFinalPolygon = true;

                // switch to the other polygon
                thisVertex = thisVertex.neighbor;
                output.Add(thisVertex);

                // what happens here?
                // depending on the boolean operation we determine whether we need to move forward or backward
                if (getIntersectionPolygon)
                {
                    // if the vertex we switch at is an entry vertex, we need to move further into the shape
                    // if it is an exit vertex, we need to move back into the shape
                    isMovingForward = thisVertex.isEntry ? true : false;
                }
                else
                {
                    isMovingForward = !isMovingForward;
                }
                thisVertex = isMovingForward ? thisVertex.next : thisVertex.prev;
            }

            safety++;
            if (safety > 10000)
            {
                Debug.LogError("STUCK IN LOOP!!!");
                break;
            }
        }

        return output;
    }

    private static void ResetVertices(List<ClipVertex> vertices)
    {
        ClipVertex current = vertices[0];

        int safety = 0;
        while (true)
        {
            current.isTakenByFinalPolygon = false;
            current.nextPoly = null;
            if (current.isIntersection)
            {
                current.neighbor.isTakenByFinalPolygon = false;
            }

            current = current.next;
            if (current.Equals(vertices[0]))
            {
                break;
            }

            safety++;
            if (safety > 10000)
            {
                Debug.LogError("STUCK IN LOOP!!!");
            }
        }
    }

    private static ClipVertex FindFirstEntryVertex(List<ClipVertex> polygon)
    {
        ClipVertex current = polygon[0], firstVertex = current;

        int safety = 0;
        while (true)
        {
            // do we need to check for intersection here?
            // if isEntry is true, then it should always be an intersection vertex right?
            if (current.isIntersection && current.isEntry && !current.isTakenByFinalPolygon)
            {
                break;
            }

            current = current.next;

            // we didn't find a vertex that satisfies our requirements :(
            if (current.Equals(firstVertex))
            {
                current = null;
                break;
            }

            safety++;
            if (safety > 10000)
            {
                Debug.LogError("STUCK IN LOOP!!!");
            }
        }

        return current;
    }

    private static void AddPolygonToList(List<ClipVertex> verticesToAdd, List<List<Vector2>> output, bool shouldReverse)
    {
        List<Vector2> vector2s = new List<Vector2>();
        output.Add(vector2s);

        for (int i = 0; i < verticesToAdd.Count; i++)
        {
            ClipVertex v = verticesToAdd[i];
            vector2s.Add(v.coordinate);

            if (v.nextPoly != null)
            {
                if (shouldReverse)
                {
                    vector2s.Reverse();
                }

                vector2s = new List<Vector2>();
                output.Add(vector2s);
            }
        }

        if (shouldReverse)
        {
            output[output.Count - 1].Reverse();
        }
    }
}