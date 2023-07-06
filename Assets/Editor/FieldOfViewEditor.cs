using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov._viewRadius);
        Vector3 viewAngleA = fov.DirFromAngle(-fov._viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov._viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov._viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov._viewRadius);

        Handles.color = Color.red;

        foreach(Transform visibleTarget in fov._visibleTargets)
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }

    }
}
