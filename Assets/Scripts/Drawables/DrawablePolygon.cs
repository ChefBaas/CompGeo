using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawablePolygon : IDrawable
{
    private List<Vector3> outline;
    public List<Vector3> Outline
    {
        get => outline;
    }
    private List<Vector3> otherPoints;
    public List<Vector3> OtherPoints
    {
        get => otherPoints;
    }
    private List<Triangle> triangles;
    public List<Triangle> Triangles
    {
        get => triangles;
        set => triangles = value;
    }

    private Color displayColor;

    public DrawablePolygon(List<Vector3> outline, List<Vector3> otherPoints, Color displayColor)
    {
        this.outline = outline;
        this.otherPoints = otherPoints;
        this.displayColor = displayColor;
    }

    // DOESN'T YET WORK FOR SELF-INTERSECTING SHAPES!!!
    public bool IsConvex()
    {
        int positiveResults = 0;
        int negativeResults = 0;
        for (int i = 0; i < outline.Count; i++)
        {
            int iMinusOne = MathLibrary.FixListIndex(i - 1, outline.Count);
            int iPlusOne = MathLibrary.FixListIndex(i + 1, outline.Count);

            float dx1 = outline[i].x  - outline[iMinusOne].x;
            float dy1 = outline[i].z - outline[iMinusOne].z;
            float dx2 = outline[iPlusOne].x - outline[i].x;
            float dy2 = outline[iPlusOne].z - outline[i].z;
            
            float cross = dx1 * dy2 - dy1 * dx2;

            if (cross > 0)
            {
                positiveResults++;
            }
            else
            {
                negativeResults++;
            }
        }

        return positiveResults == outline.Count || negativeResults == outline.Count;
    }

    public void Draw(MasterDrawer masterDrawer)
    {
        if (outline != null)
        {
            masterDrawer.DrawPolygon(outline, displayColor);
        }
        if (otherPoints != null)
        {
            masterDrawer.DrawPointCloud(otherPoints, new Color(displayColor.r, displayColor.g, displayColor.b, 0.5f));
        }
        if (triangles != null)
        {
            masterDrawer.DrawTriangles(triangles, displayColor, true);
        }
    }

    public List<Vector2> GetV2Outline()
    {
        return TransformRepresentation.Vector3sToVector2s(outline);
    }

    public List<Vector2> GetNormals(bool normalize = false)
    {
        List<Vector2> normals = new List<Vector2>();

        for (int i = 0; i < outline.Count; i++)
        {
            int iPlusOne = MathLibrary.FixListIndex(i + 1, outline.Count);
            Vector3 direction = outline[iPlusOne] - outline[i];
            Vector2 normal = new Vector2(-direction.z, direction.x);
            if (normalize)
            {
                normal.Normalize();
            }
            normals.Add(normal);
        }

        return normals;
    }

    public Vector3 GetAveragePoint()
    {
        Vector3 average = outline[0];
        for (int i = 1; i < outline.Count; i++)
        {
            average += outline[i];
        }
        return average / (float)outline.Count;
    }
}
