using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Main))]
public class MainEditor : Editor
{
    private Main main;

    public override void OnInspectorGUI()
    {
        main = (Main)target;

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical();
        {
            DrawStandardOptions();
            switch (main.operation)
            {
                case Main.CompGeoOperation.Triangulate:
                    DrawTriangulateOptions();
                    break;

                case Main.CompGeoOperation.CalculateOverlap:
                    DrawOverlapOptions();
                    break;

                case Main.CompGeoOperation.CheckIntersection:
                    DrawIntersectionOptions();
                    break;
            }
        }
        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            main.DataChanged();
            SceneView.RepaintAll();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }

    private void DrawStandardOptions()
    {
        bool b = main.makeConvexPolygons;
        b = EditorGUILayout.Toggle("MakeConvexPolygons", b);
        main.makeConvexPolygons = b;
    }

    private void DrawTriangulateOptions()
    {
        if (main.makeConvexPolygons || main.polygonsAreConvex)
        {
            bool c = main.includeInsidePoints;
            c = EditorGUILayout.Toggle("IncludeInsidePoints", c);
            main.includeInsidePoints = c;
        }

        bool d = main.avoidUglyTriangles;
        d = EditorGUILayout.Toggle("AvoidUglyTriangles", d);
        main.avoidUglyTriangles = d;
    }

    private void DrawOverlapOptions()
    {
    }

    private void DrawIntersectionOptions()
    {
        if ((main.makeConvexPolygons || main.polygonsAreConvex) && main.smallestDistanceToResolveCollision != Vector3.zero)
        {
            if (GUILayout.Button("Resolve collision"))
            {
                main.ResolveIntersection();
            }
        }
    }
}
