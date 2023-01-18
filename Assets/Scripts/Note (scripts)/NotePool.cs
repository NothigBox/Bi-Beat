using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    [SerializeField] private GameObject lightNotesPrefab;
    [SerializeField] private GameObject darkNotesPrefab;

    private Queue<Note> lightNotesAvailable;
    private Queue<Note> darkNotesAvailable;

    private void Awake()
    {
        lightNotesAvailable = new Queue<Note>();
        darkNotesAvailable = new Queue<Note>();
    }

    public Note SpawnLightNote(Vector3 position)
    {
        if (lightNotesAvailable.Count <= 0)
        {
            Note note = Instantiate(
                lightNotesPrefab, 
                position, 
                Quaternion.identity).GetComponent<Note>();
            
            note.gameObject.SetActive(false);
            
            lightNotesAvailable.Enqueue(note);
        }

        Note _note = lightNotesAvailable.Dequeue();
        _note.transform.position = position;
        _note.gameObject.SetActive(true);
        _note.OnDeath += lightNotesAvailable.Enqueue;
        
        return _note;
    }

    public Note SpawnDarkNote(Vector3 position)
    {
        if (darkNotesAvailable.Count <= 0)
        {
            Note note = Instantiate(
                darkNotesPrefab, 
                Vector3.up * 100f, 
                Quaternion.identity).GetComponent<Note>();
            
            note.gameObject.SetActive(false);
            
            darkNotesAvailable.Enqueue(note);
        }
        
        Note _note = darkNotesAvailable.Dequeue();
        _note.transform.position = position;
        _note.gameObject.SetActive(true);
        _note.OnDeath += darkNotesAvailable.Enqueue;

        return _note;
    }
}
