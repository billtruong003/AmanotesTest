using System.Collections.Generic;
using UnityEngine;
using Variables;

public abstract class NoteDrag : NoteBase
{
    public List<Vector3> dragPath;
    public int currentDragIndex = 0;
    public float dragStartTime;
    public bool isDragging;

    public override void Initialize(NoteType noteType, float spawnTime, Vector3 position, float speed, List<Vector3> dragPath = null)
    {
        base.Initialize(noteType, spawnTime, position, speed, dragPath);
        this.dragPath = dragPath;
        isDragging = false;
    }

    public override void UpdateNote(float deltaTime)
    {
        if (isDragging)
        {
            if (currentDragIndex < dragPath.Count)
            {
                position = Vector3.MoveTowards(position, dragPath[currentDragIndex], speed * deltaTime);
                transform.position = position;

                if (Vector3.Distance(position, dragPath[currentDragIndex]) < 0.01f)
                {
                    currentDragIndex++;
                }
            }
            else
            {
                EndDrag();
            }
        }
        else
        {
            base.UpdateNote(deltaTime);
        }
    }

    public void StartDrag()
    {
        isDragging = true;
        dragStartTime = Time.time;
        currentDragIndex = 0;
    }

    public void EndDrag()
    {
        isDragging = false;
        Hit();
    }

    public override void Hit()
    {
        base.Hit();
    }
}