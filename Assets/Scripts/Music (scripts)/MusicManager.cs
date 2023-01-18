using System;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(NotePool))]
[RequireComponent(typeof(ParticlesPool))]
[RequireComponent(typeof(MusicScoreReader))]
public class MusicManager : MonoBehaviour
{    
    [SerializeField] private float beatsPerMinute;
    [SerializeField] private float playDelay;
    
    [SerializeField] private Transform lightNotesSpawnPointsParent;
    [SerializeField] private Transform darkNotesSpawnPointsParent;
    
    [SerializeField] private ParticleSystem sparkles;
    
    [SerializeField] private AudioClip[] lightNotesClips;
    [SerializeField] private AudioClip[] darkNotesClips;
    [SerializeField] private AudioClip triangleClip;

    [SerializeField] private Instrument darkNotesInstrument;
    [SerializeField] private Instrument lightNotesInstrument;
    
    private float beatTimer;
    private int noteCount;
    private int percussionCounter;
    private int wavesPassed;
    private bool isPlaying;
    
    private AudioSource[] sources;
    private NotePool notePool;
    private ParticlesPool particlesPool;
    private MusicScoreReader reader;
    
    public Action OnDarkNoteSpawned;
    public Action OnLightNoteSpawned;

    public MusicScoreReader Reader => reader;

    private void Awake()
    {
        beatTimer = 0f;
        noteCount = 0;
        wavesPassed = 0;
        percussionCounter = 0;
        isPlaying = false;

        sources = new AudioSource[0];
        
        notePool = GetComponent<NotePool>();
        reader = GetComponent<MusicScoreReader>();
        particlesPool = GetComponent<ParticlesPool>();
    }

    private void FixedUpdate()
    {
        if(!isPlaying) return;
        
        if (beatTimer < 60f / beatsPerMinute)
        {
            beatTimer += Time.fixedDeltaTime;
        }
        else
        {
            beatTimer = 0f;
            noteCount++;

            if (noteCount >= reader.NotesCount)
            {
                isPlaying = false;
                noteCount = reader.NotesCount;
            }
            
            SpawnNotes();
        }
    }

    public void PlayDelay()
    {
        Invoke(nameof(Play), playDelay);
    }

    void Play()
    {
        beatTimer = 0f;
        SetIsPlaying(true);
    }

    private int eso = 4;
    private float probability = 60f;
    
    void SpawnNotes()
    {
        OnDarkNoteSpawned?.Invoke();
        OnLightNoteSpawned?.Invoke();

        string[] notes = reader.GetNotes(noteCount);
        int noteID = int.Parse(notes[0]);
        int spawmID = UnityEngine.Random.Range(0, lightNotesSpawnPointsParent.childCount);

        Note note = default; 

        if (noteID >= 0)
        {
            note = notePool.SpawnLightNote(lightNotesSpawnPointsParent
                .GetChild(spawmID).position);
            note.AutoSetSpeed();
            note.ID = noteID;
            note.OnDeath += PlayLightNote;
        }
        
        noteID = int.Parse(notes[1]);
        if (noteID >= 0)
        {
            note = notePool.SpawnDarkNote(darkNotesSpawnPointsParent
                .GetChild(spawmID).position);
            note.AutoSetSpeed();
            note.ID = noteID;
            note.OnDeath += PlayDarkNote;
        }
    }

    private void PlayDarkNote(Note note)
    {        
        sparkles.Play();
        particlesPool.SpawnDarkWaveAtPosition(note.transform.position);

        if(!note.DoSound) return;

        darkNotesInstrument.PlayAnimation();

        AudioSource darkSource = GetAudioSource();
        darkSource.loop = false;
        darkSource.Stop();
        darkSource.clip = darkNotesClips[note.ID];
        darkSource.Play();

        /*
        float r = UnityEngine.Random.Range(0f, 100f);

        if (r <= probability)
        */
        
        percussionCounter++;
        
        if(percussionCounter >= eso)
        {
            percussionCounter = 0;
            
            AudioSource source = GetAudioSource();
            source.loop = false;
            source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            source.clip = triangleClip;
            source.Play();
        }
    }
    
    private void PlayLightNote(Note note)
    {        
        sparkles.Play();
        particlesPool.SpawnLightWaveAtPosition(note.transform.position);
        
        if(!note.DoSound) return;

        lightNotesInstrument.PlayAnimation();

        AudioSource lightSource = GetAudioSource();
        lightSource.loop = false;
        lightSource.Stop();
        lightSource.clip = lightNotesClips[note.ID];
        lightSource.Play();
        
        /*
        float r = UnityEngine.Random.Range(0f, 100f);

        if (r <= probability)
        */
        percussionCounter++;
        
        if(percussionCounter >= eso)
        {
            percussionCounter = 0;
            
            AudioSource source = GetAudioSource();
            source.loop = false;
            source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            source.clip = triangleClip;
            source.Play();
        }
    }

    public AudioSource GetAudioSource()
    {
        AudioSource result = default;
        int i = 0;

        do
        {
            if (i >= sources.Length)
            {
                result = gameObject.AddComponent<AudioSource>();
                break;
            }
            
            result = sources[i];
            i++;
        }
        while (sources[i - 1].isPlaying);
        
        return result;
    }

    public void SpawnKillWave(Vector3 position)
    {
        particlesPool.SpawnKillWaveAtPosition(position);
    }

    public void SetIsPlaying(bool isPlaying)
    {
        CancelInvoke();
        
        this.isPlaying = isPlaying;
        Note.canMove = isPlaying;
    }

    public void AutoSelectScore()
    {
        if (PlayerPrefs.GetInt("Retry") < 1)
        {
            int r = UnityEngine.Random.Range(0, reader.ScoresCount);
            reader.GetMusicScore(r);
            PlayerPrefs.SetInt("LastScoreIndex", r);
        }
        else
        {
            reader.GetMusicScore(PlayerPrefs.GetInt("LastScoreIndex"));
            PlayerPrefs.SetInt("Retry", 0);
        }

        beatsPerMinute = reader.GetBPM();
    }

    public void Celebrate()
    {
        ParticleSystem.MainModule main = sparkles.main;
        main.loop = true;
        sparkles.Play();
    }

    public int GetTotalNotesCount(bool includeSilences = false)
    {
        if (includeSilences) return reader.NotesCount * 2;

        int result = 0;
        
        for (int i = 1; i <= reader.NotesCount; i++)
        {
            var notes = reader.GetNotes(i);

            try 
            {
                if (int.Parse(notes[0]) != -1) result++;
                if (int.Parse(notes[1]) != -1) result++;
            }
            catch 
            {
                Debug.LogWarning("Verify that there are no void lines in the music score selected.");
            }
        }

        return result;
    }

    public bool SetScore(string scoreName)
    {
        var scores = reader.Scores.musicScores;
        
        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i].name.ToLower() == scoreName.ToLower())
            {
                reader.GetMusicScore(i);
                beatsPerMinute = reader.GetBPM();
                PlayerPrefs.SetInt("LastScoreIndex", i);
                return true;
            }
        }
        
        return false;
    }
}
