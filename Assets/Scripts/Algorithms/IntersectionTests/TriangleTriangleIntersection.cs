using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriangleTriangleIntersection
{
    public static bool Intersect(Triangle t1, Triangle t2)
    {
        // get an AABB around both triangles, test intersection there first
        Vector2 bl1, tr1, bl2, tr2;
        t1.GetAABB(out bl1, out tr1);
        t2.GetAABB(out bl2, out tr2);
        if (!AABBAABBIntersection.Intersect(bl1, tr1, bl2, tr2))
        {
            return false;
        }

        List<Vector2> linesT1 = t1.GetLines(), linesT2 = t2.GetLines();
        for (int i = 0; i < linesT1.Count; i += 2)
        {
            for (int j = 0; j < linesT2.Count; j += 2)
            {
                Vector2 intersection = Vector2.zero;
                if (MathLibrary.LinepiecesIntersect(linesT1[i], linesT1[i + 1], linesT2[j], linesT2[j + 1], out intersection) == MathLibrary.bool3.True)
                {
                    return true;
                }
            }
        }

        return MathLibrary.PointIsInTriangle(linesT1[0], t2.v1.GetPos2D_XZ(), t2.v2.GetPos2D_XZ(), t2.v3.GetPos2D_XZ(), true) || MathLibrary.PointIsInTriangle(linesT2[0], t1.v1.GetPos2D_XZ(), t1.v2.GetPos2D_XZ(), t1.v3.GetPos2D_XZ(), true);
    }
}