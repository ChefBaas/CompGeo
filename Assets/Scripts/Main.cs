using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.U2D.Path;
using UnityEngine;

[ExecuteAlways]
public class Main : MonoBehaviour
{
    public enum CompGeoOperation
    {
        Nothing,
        Triangulate,
        CalculateOverlap,
        CheckIntersection
    }

    [SerializeField] private MasterDrawer masterDrawer;
    [SerializeField] private List<PointSource> pSources;
    [SerializeField] private PolygonBuilder polygonBuilder;

    [HideInInspector] public bool makeConvexPolygons = false;
    [HideInInspector] public bool includeInsidePoints = false;
    [HideInInspector] public bool avoidUglyTriangles = false;
    [SerializeField] public CompGeoOperation operation = CompGeoOperation.Nothing;

    List<IDrawable> drawables = new List<IDrawable>();

    [HideInInspector] public bool polygonsAreConvex = false;
    [HideInInspector] public Vector3 smallestDistanceToResolveCollision = Vector3.zero;

    private void OnEnable()
    {
        for (int i = 0; i < pSources.Count; i++)
        {
            pSources[i].OnDataChanged.AddListener(DataChanged);
        }
        PerformOperation();
    }

    public void DataChanged()
    {
        PerformOperation();
    }

    private void PerformOperation()
    {
        drawables.Clear();

        polygonsAreConvex = true;
        smallestDistanceToResolveCollision = Vector3.zero;

        List<DrawablePolygon> polygons = new List<DrawablePolygon>();
        for (int i = 0; i < pSources.Count; i++)
        {
            DrawablePolygon p = polygonBuilder.BuildPolygon(pSources[i].GetPoints(), pSources[i].GetColor(), makeConvexPolygons);
            polygons.Add(p);
            drawables.Add(p);
            if (!p.IsConvex())
            {
                polygonsAreConvex = false;
            }
        }

        switch (operation)
        {
            case CompGeoOperation.Triangulate:
                Triangulate(polygons);
                break;

            case CompGeoOperation.CheckIntersection:
                drawables.AddRange(CheckIntersection(polygons));
                break;

            case CompGeoOperation.CalculateOverlap:
                drawables.AddRange(CalculateOverlap(polygons));
                break;
        }

        SendDrawInfo();
    }

    private void Triangulate(List<DrawablePolygon> polygons)
    {
        for (int i = 0; i < polygons.Count; i++)
        {
            if (polygons[i].IsConvex())
            {
                List<Triangle> triangles = TriangulateMethods.TriangulateConvex(polygons[i]);
                if (includeInsidePoints)
                {
                    TriangulateMethods.SplitTriangles(triangles, polygons[i].OtherPoints);
                }
                if (avoidUglyTriangles)
                {
                    Debug.LogFormat("STARTING SHAPE {0}", i);
                    triangles = CreateNicerTriangles(triangles);
                }
                polygons[i].Triangles = triangles;
            }
            else
            {
                List<Triangle> triangles = TriangulateMethods.TriangulateConcave(polygons[i]);
                polygons[i].Triangles = triangles;
                if (avoidUglyTriangles)
                {
                    triangles = CreateNicerTriangles(triangles);
                }
            }
        }
    }

    private List<IDrawable> CheckIntersection(List<DrawablePolygon> polygons)
    {
        List<IDrawable> output = new List<IDrawable>();

        bool useSAT = true;
        for (int i = 0; i < polygons.Count; i++)
        {
            if (!polygons[i].IsConvex())
            {
                useSAT = false;
                break;
            }
        }

        if (useSAT)
        {
            Vector2 smallestOverlap;
            if (SATIntersection.Intersect(polygons[0], polygons[1], out smallestOverlap))
            {
                Vector3 avgPoint = (polygons[0].GetAveragePoint() + polygons[1].GetAveragePoint()) * 0.5f;

                smallestDistanceToResolveCollision = new Vector3(smallestOverlap.x, 0f, smallestOverlap.y);
                DrawableLine line1 = new DrawableLine(avgPoint, avgPoint + smallestDistanceToResolveCollision * 0.5f, Color.magenta);
                DrawableLine line2 = new DrawableLine(avgPoint, avgPoint - smallestDistanceToResolveCollision * 0.5f, Color.green);
                output.Add(line1);
                output.Add(line2);
            }
        }
        else
        {
            Triangulate(polygons);
            List<Triangle> t1 = polygons[0].Triangles, t2 = polygons[1].Triangles;
            for (int i = 0; i < t1.Count; i++)
            {
                for (int j = 0; j < t2.Count; j++)
                {
                    if (TriangleTriangleIntersection.Intersect(t1[i], t2[j]))
                    {
                        if (MathLibrary.IsTriangleOrientedClockwise(t1[i]))
                        {
                            t1[i].ChangeOrientation();
                        }
                        if (MathLibrary.IsTriangleOrientedClockwise(t2[j]))
                        {
                            t2[j].ChangeOrientation();
                        }
                        List<Vector3> pointsT1 = TransformRepresentation.Vector2sToVector3s(t1[i].GetPoints());
                        List<Vector3> pointsT2 = TransformRepresentation.Vector2sToVector3s(t2[j].GetPoints());
                        output.AddRange(CalculateOverlap(new List<DrawablePolygon>() { new DrawablePolygon(pointsT1, null, Color.black), new DrawablePolygon(pointsT2, null, Color.black) }));
                    }
                }
            }
        }
        return output;
    }

    private List<IDrawable> CalculateOverlap(List<DrawablePolygon> polygons)
    {
        List<IDrawable> output = new List<IDrawable>();

        bool useSH = true;
        for (int i = 0; i < polygons.Count; i++)
        {
            if (!polygons[i].IsConvex())
            {
                useSH = false;
                break;
            }
        }

        if (useSH)
        {
            List<Vector3> overlap = SutherlandHodgman.ClipPolygon(polygons[0].Outline, polygons[1].Outline);
            DrawablePolygon overlapPoly = new DrawablePolygon(overlap, null, Color.blue);
            output.Add(overlapPoly);
        }
        else
        {
            List<List<Vector2>> overlapVectors = GreinerHormann.ClipPolygons(polygons[0].GetV2Outline(), polygons[1].GetV2Outline());
            for (int j = 0; j < overlapVectors.Count; j++)
            {
                List<Vector3> overlap = TransformRepresentation.Vector2sToVector3s(overlapVectors[j]);
                DrawablePolygon p = new DrawablePolygon(overlap, new List<Vector3>(), Color.cyan);
                output.Add(p);
            }
        }

        return output;
    }

    public void ResolveIntersection()
    {
        pSources[0].PushPoints(smallestDistanceToResolveCollision * -0.5f);
        pSources[1].PushPoints(smallestDistanceToResolveCollision * 0.5f);
    }

    private List<Triangle> CreateNicerTriangles(List<Triangle> triangles)
    {
        List<HalfEdge> edges = TransformRepresentation.TrianglesToHalfEdges(triangles);
        TriangulateMethods.TriangulateByFlippingEdges(edges);
        return TransformRepresentation.HalfEdgesToTriangles(edges);
    }

    private void SendDrawInfo()
    {
        masterDrawer.SetDrawInfo(drawables);
    }
}
