using System.Collections.Generic;
using UnityEngine;
using Variables;

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner Instance;
    public SongData songData; // Class này bạn cần tự định nghĩa, chứa thông tin bài hát
    public GameObject noteTapPrefab;
    public GameObject noteDragStraightPrefab;
    public GameObject noteDragCurvePrefab;
    public GameObject noteSpecialPrefab;
    public float currentTime = 0f;

    private List<NoteData> notesToSpawn; // Danh sách các note cần được spawn
    private int nextNoteIndex = 0;

    void Start()
    {
        // Load dữ liệu bài hát từ file hoặc nguồn nào đó
        // songData = LoadSongData("your_song_data_file");
        // Giả sử songData đã được load:
        notesToSpawn = songData.notes;
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        // Kiểm tra xem có note nào cần được spawn không
        while (nextNoteIndex < notesToSpawn.Count && notesToSpawn[nextNoteIndex].spawnTime <= currentTime)
        {
            SpawnNote(notesToSpawn[nextNoteIndex]);
            nextNoteIndex++;
        }
    }

    void SpawnNote(NoteData noteData)
    {
        GameObject notePrefab = GetNotePrefab(noteData.noteType);
        GameObject noteObject = Instantiate(notePrefab);
        NoteBase note = noteObject.GetComponent<NoteBase>();

        if (noteData.noteType == NoteType.DragStraight)
        {
            NoteDragStraightData dragData = (NoteDragStraightData)noteData;
            note.Initialize(noteData.noteType, noteData.spawnTime, noteData.position, noteData.speed, new List<Vector3>() { noteData.position, dragData.endPosition });
        }
        else if (noteData.noteType == NoteType.DragCurve)
        {
            NoteDragCurveData curveData = (NoteDragCurveData)noteData;
            note.Initialize(noteData.noteType, noteData.spawnTime, noteData.position, noteData.speed, curveData.controlPoints);
        }
        else
        {
            note.Initialize(noteData.noteType, noteData.spawnTime, noteData.position, noteData.speed);
        }
    }

    GameObject GetNotePrefab(NoteType type)
    {
        switch (type)
        {
            case NoteType.Tap: return noteTapPrefab;
            case NoteType.DragStraight: return noteDragStraightPrefab;
            case NoteType.DragCurve: return noteDragCurvePrefab;
            case NoteType.Special: return noteSpecialPrefab;
            default: return null;
        }
    }
}

