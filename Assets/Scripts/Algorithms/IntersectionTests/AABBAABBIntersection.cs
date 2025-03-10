using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AABBAABBIntersection
{
    public static bool Intersect(Vector2 bl1, Vector2 tr1, Vector2 bl2, Vector2 tr2)
    {
        if (bl1.x > tr2.x)
        {
            return false;
        }
        else if (bl2.x > tr1.x)
        {
            return false;
        }
        else if (bl1.y > tr2.y)
        {
            return false;
        }
        else if (bl2.y > tr1.y)
        {
            return false;
        }
        return true;
    }
}
