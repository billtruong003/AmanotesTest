using System;
using System.Collections.Generic;
using UnityEngine;
using Variables;

[Serializable]
public class NoteData
{
    public NoteType noteType;
    public float spawnTime;
    public Vector3 position;
    public float speed;
}

[Serializable]
public class NoteDragStraightData : NoteData
{
    public Vector3 endPosition;
}

[Serializable]
public class NoteDragCurveData : NoteData
{
    public List<Vector3> controlPoints;
}