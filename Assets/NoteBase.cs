using UnityEngine;
using System.Collections.Generic;
using Variables;

public abstract class NoteBase : MonoBehaviour
{
    public NoteType noteType;
    public float spawnTime;
    public Vector3 position;
    public float speed;
    public bool isHit;

    public virtual void Initialize(NoteType noteType, float spawnTime, Vector3 position, float speed, List<Vector3> dragPath = null)
    {
        this.noteType = noteType;
        this.spawnTime = spawnTime;
        this.position = position;
        this.speed = speed;
        this.isHit = false;

        transform.position = position;
    }

    public virtual void UpdateNote(float deltaTime)
    {
        position.y -= speed * deltaTime;
        transform.position = position;

        if (position.y < -5f)
        {
            Miss();
        }
    }

    public virtual void Hit()
    {
        isHit = true;
        Debug.Log(noteType + " Note Hit!");
        DestroyNote();
    }

    public virtual void Miss()
    {
        if (!isHit)
        {
            Debug.Log(noteType + " Note Missed!");
        }
        DestroyNote();
    }

    public virtual void DestroyNote()
    {
        Destroy(gameObject);
    }
}