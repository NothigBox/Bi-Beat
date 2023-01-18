using System;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(MusicScores))]
public class MusicScoreReader : MonoBehaviour
{
    [SerializeField] private MusicScores scores;
    private string[] currentScore;
    
    public int NotesCount => (currentScore?? GetMusicScore(0)).Length - 1;
    public int ScoresCount => scores.musicScores.Length;
    public MusicScores Scores => scores;
    public static MusicScores _Scores { get; private set; }

    private void Awake()
    {
        _Scores = scores;
    }

    public float GetBPM()
    {
        return float.Parse(currentScore[0].Split('/')[0]);
    }

    public string[] GetNotes(int index)
    {
        return currentScore[index].Split('/');
    }

    public string[] GetMusicScore(int index)
    {
        return currentScore = scores.musicScores[index].ToString().Split('\n');
    }

    public static int GetAvailableness(TextAsset textAsset)
    {
        var scoreInfo = PlayerPrefs.GetString(textAsset.name);
        return int.Parse(scoreInfo != ""? scoreInfo : GetMusicScore(textAsset)[0].Split('/')[1]);
    }

    public static string[] GetMusicScore(TextAsset textAsset)
    {
        return textAsset.ToString().Split('\n');
    }

    public static void SetScoreAvailableness(TextAsset textAsset, int availableness)
    {
        PlayerPrefs.SetString(textAsset.name, availableness.ToString());
    }
}
