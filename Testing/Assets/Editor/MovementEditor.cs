using UnityEngine;
using UnityEditor;
using Enemies;

[CustomEditor(typeof(Movement))]

public class MovementEditor : Editor {

    private void OnSceneGUI() {
        Movement fov = (Movement)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.sightRange);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.viewAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.viewAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.sightRange);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.sightRange);

        if (fov.canSeePlayer) {
            Handles.color = Color.red;
            Handles.DrawLine(fov.transform.position, fov.thePlayer.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees) {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));

    }
    
}
