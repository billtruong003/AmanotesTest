using System.Collections.Generic;
using UnityEngine;
using Variables;

public class NoteDragCurve : NoteDrag
{
    public List<Vector3> controlPoints;

    public override void Initialize(NoteType noteType, float spawnTime, Vector3 position, float speed, List<Vector3> dragPath = null)
    {

        this.controlPoints = dragPath;
        dragPath = CalculateBezierPath(controlPoints);
        base.Initialize(noteType, spawnTime, position, speed, dragPath);
    }


    private List<Vector3> CalculateBezierPath(List<Vector3> points)
    {
        List<Vector3> path = new List<Vector3>();
        for (float t = 0; t <= 1; t += 0.05f)
        {
            Vector3 p = CalculateBezierPoint(t, points[0], points[1], points[2], points[3]);
            path.Add(p);
        }
        return path;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}