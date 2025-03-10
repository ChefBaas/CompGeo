using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[CustomEditor(typeof(FreePoints))]
public class FreePointsEditor : Editor
{
    private FreePoints freePoints;

    public override void OnInspectorGUI()
    {
        freePoints = (FreePoints)target;
        List<Vector3> points = freePoints.Points;

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical();
        {
            if (GUILayout.Button("^"))
            {
                freePoints.PushPoints(Vector3.forward * 0.1f);
                /*for (int i = 0; i < points.Count; i++)
                {
                    points[i] = new Vector3(points[i].x, points[i].y, points[i].z + 0.1f);
                }*/
            }
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("<"))
                {
                    freePoints.PushPoints(Vector3.left * 0.1f);
                    /*for (int i = 0; i < points.Count; i++)
                    {
                        points[i] = new Vector3(points[i].x - 0.1f, points[i].y, points[i].z);
                    }*/
                }
                if (GUILayout.Button(">"))
                {
                    freePoints.PushPoints(Vector3.right * 0.1f);
                    /*for (int i = 0; i < points.Count; i++)
                    {
                        points[i] = new Vector3(points[i].x + 0.1f, points[i].y, points[i].z);
                    }*/
                }
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("v"))
            {
                /*for (int i = 0; i < points.Count; i++)
                {
                    points[i] = new Vector3(points[i].x, points[i].y, points[i].z - 0.1f);
                }*/
                freePoints.PushPoints(Vector3.back * 0.1f);
            }
        }
        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            freePoints.SendDataChangedEvent();
            SceneView.RepaintAll();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
