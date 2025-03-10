using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawableLine : IDrawable
{
    private Vector3 start;
    public Vector3 Start
    {
        get => start;
    }
    private Vector3 end;
    public Vector3 End
    {
        get => end;
    }

    private Color c;

    public DrawableLine(Vector3 start, Vector3 end, Color c)
    {
        this.start = start;
        this.end = end;
        this.c = c;
    }

    public void Draw(MasterDrawer drawer)
    {
        drawer.DrawLine(start, end, c);
    }
}
