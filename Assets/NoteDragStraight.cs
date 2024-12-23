using System.Collections.Generic;
using UnityEngine;
using Variables;

public class NoteDragStraight : NoteDrag
{
    public Vector3 endPosition;

    public override void Initialize(NoteType noteType, float spawnTime, Vector3 position, float speed, List<Vector3> dragPath = null)
    {
        // Lấy endPosition từ dragPath (đã được truyền vào từ NoteSpawner)
        this.endPosition = dragPath[1];
        // Không cần tạo lại List<Vector3> path nữa
        base.Initialize(noteType, spawnTime, position, speed, dragPath);
    }
}