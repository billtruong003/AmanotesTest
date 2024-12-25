using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    public static NotePool Instance;
    public Notes notePrefab;
    public int poolSize = 50;
    public Transform noteContainer;

    private Queue<Notes> availableNotes = new Queue<Notes>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateAndEnqueueNote();
        }
    }

    private Notes CreateAndEnqueueNote()
    {
        Notes note = Instantiate(notePrefab, noteContainer);
        note.gameObject.SetActive(false);
        availableNotes.Enqueue(note);
        return note;
    }

    public Notes GetNote()
    {
        if (availableNotes.Count == 0)
        {
            Debug.LogWarning("Note pool exhausted, instantiating new note.");
            return CreateAndEnqueueNote(); // Still allow growth if needed
        }

        Notes spawnedNote = availableNotes.Dequeue();
        spawnedNote.gameObject.SetActive(true);
        return spawnedNote;
    }

    public void ReturnNote(Notes note)
    {
        note.gameObject.SetActive(false);
        availableNotes.Enqueue(note);
    }
}