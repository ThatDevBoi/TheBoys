using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A cloass that edits the FieldOfView class which edits angles / radius
/// This class makes the angle and raduis visual in the scene
/// Dont attach to objects
/// </summary>
[CustomEditor (typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        // Radius
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        // Angle
        Vector3 viewAngleA = fow.directionFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.directionFromAngle(fow.viewAngle / 2, false);
        // Draw Angle
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        // Visable Targets
        // SHOOTS LINE AT PLAYER
        Handles.color = Color.red;
        foreach(Transform visableTarget in fow.visableTargets)
        {
            Handles.DrawLine(fow.transform.position, visableTarget.position);
        }
    }
}
