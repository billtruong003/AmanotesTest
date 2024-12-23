using UnityEngine;
using Variables;
using System.Collections.Generic;

public class NoteSpecial : NoteBase
{
    public string specialEffect;

    public override void Initialize(NoteType noteType, float spawnTime, Vector3 position, float speed, List<Vector3> dragPath = null)
    {
        base.Initialize(noteType, spawnTime, position, speed, null); // Truyền null cho dragPath
    }

    public override void Hit()
    {
        base.Hit();
        Debug.Log("Special Effect: " + specialEffect);
    }
}