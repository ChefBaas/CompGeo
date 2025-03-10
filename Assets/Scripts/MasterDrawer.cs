using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class MasterDrawer : MonoBehaviour
{
    // public UnityEvent requestDrawInfo;

    private List<IDrawable> drawables = new List<IDrawable>();

    public void AddDrawable(IDrawable drawable)
    {
        if (!drawables.Contains(drawable)) 
        {
            drawables.Add(drawable); 
        }
    }

    public void RemoveDrawable(IDrawable drawable)
    {
        drawables.Remove(drawable);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < drawables.Count; i++)
        {
            drawables[i].Draw(this);
        }
    }
    
    public void SetDrawInfo(List<IDrawable> drawables)
    {
        this.drawables = drawables;
    }

    public void DrawPolygon(List<Vector3> outline, Color c, bool drawPoints = true, bool drawLines = true)
    {
        Gizmos.color = c;
        for (int i = 0; i < outline.Count; i++)
        {
            int iPlusOne = MathLibrary.FixListIndex(i + 1, outline.Count);
            if (drawPoints) 
            {
                Gizmos.DrawSphere(outline[i], 0.2f);
                // Handles.color = c;
                // Handles.Label(outline[i] + Vector3.up * 0.2f + Vector3.right * 0.2f, i.ToString());
            }
            if (drawLines) Gizmos.DrawLine(outline[i], outline[iPlusOne]);
        }
    }

    public void DrawPointCloud(List<Vector3> points, Color c)
    {
        Gizmos.color = c;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i], 0.2f);
        }
    }

    public void DrawTriangles(List<Triangle> triangles, Color c, bool drawSpecial = false)
    {
        Gizmos.color = c;
        for (int i = 0; i < triangles.Count; i++)
        {
            Vector3 v1 = triangles[i].v1.position;
            Vector3 v2 = triangles[i].v2.position;
            Vector3 v3 = triangles[i].v3.position;

            if (drawSpecial)
            {
                Vector3 d1 = (v2 - v1).normalized;
                Vector3 d2 = (v3 - v2).normalized;
                Vector3 d3 = (v1 - v3).normalized;

                Gizmos.DrawLine(v1 + d1 * 0.1f, v2 - d1 * 0.1f);
                Gizmos.DrawLine(v2 + d2 * 0.1f, v3 - d2 * 0.1f);
                Gizmos.DrawLine(v3 + d3 * 0.1f, v1 - d3 * 0.1f);
            }
            else
            {
                Gizmos.DrawLine(v1, v2);
                Gizmos.DrawLine(v2, v3);
                Gizmos.DrawLine(v3, v1);
            }
        }
    }

    public void DrawLine(Vector3 start, Vector3 end, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawLine(start, end);
    }
}