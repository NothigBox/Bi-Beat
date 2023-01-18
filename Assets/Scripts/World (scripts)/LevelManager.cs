using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform levelsParent;
    [SerializeField] private GameObject songButtonPrefab;
    [SerializeField] private Color passedColor;
    [SerializeField] private float movingDuration;
    [SerializeField] private float movingDelay;
    [SerializeField] private bool newGame;
    [SerializeField] private Animate[] celebrateAnimations;
    [SerializeField] private ParticleSystem celebrateParticle;
    
    private bool isCelebrating;
    private TextAsset[] scores;
    private float _movingDuration;
    private Vector3 _initialPosition;

    public Action<string> OnLevelChosen;

    private bool UseCelebration => PlayerPrefs.GetInt("Celebrate") == 1;

    private void Awake()
    {
        _movingDuration = movingDuration;
        _initialPosition = transform.localPosition;
    }

    public void SetUpLevels(MusicScores musicScores)
    {
        foreach(var animation in celebrateAnimations)
        {
            animation.SetAnimCurveValue(0f);
        }

        scores = musicScores.musicScores;

        foreach (var score in scores)
        {
            if (newGame) PlayerPrefs.SetString(score.name, "");

            Transform newButton = Instantiate(songButtonPrefab).transform;
            newButton.name = score.name;
            newButton.SetParent(levelsParent);
            newButton.localScale = Vector3.one;
            newButton.localPosition = Vector3.zero;
            newButton.GetComponentInChildren<Text>().text = score.name;
            
            Button b = newButton.GetComponent<Button>();
            b.onClick.AddListener(() => OnLevelChosen(newButton.name));
            b.interactable = false;
        }

        SetAvailablenessOfLevels();
    }

    public void SetAvailablenessOfLevels()
    {
        int lastSongIndex = 0;

        for (int i = 0; i < levelsParent.childCount; i++)
        {
            int availablenessIndex = MusicScoreReader.GetAvailableness(scores[i]);
            Button b = levelsParent.GetChild(i).GetComponent<Button>();

            if (availablenessIndex == 1)
            {
                b.interactable = true;
                lastSongIndex = i;
            }
            else if (availablenessIndex == 2)
            {
                ColorBlock block = ColorBlock.defaultColorBlock;
                block.normalColor = passedColor;
                block.pressedColor= passedColor;
                block.selectedColor = passedColor;
                block.disabledColor = passedColor;
                block.highlightedColor = passedColor;
                b.colors = block;

                b.interactable = true;
                lastSongIndex = i;
            }
        }

        StartCoroutine(MovingLevelsCoroutine(lastSongIndex));
    }

    public void OnLevelPassed(string songName)
    {
        scores ??= MusicScoreReader._Scores.musicScores;
        
        for (int i = 0; i < scores.Length; i++) 
        {
            if (scores[i].name == songName)
            {
                MusicScoreReader.SetScoreAvailableness(scores[i], 2);
                if(i + 1 < scores.Length)
                {
                    if(MusicScoreReader.GetAvailableness(scores[i + 1]) < 2) 
                        MusicScoreReader.SetScoreAvailableness(scores[i + 1], 1);
                }
                return;
            }
        }
    }

    IEnumerator MovingLevelsCoroutine(int lastSongIndex) 
    {
        float timer = 0f;
        var finalPos = Vector3.right * (-750f * lastSongIndex);

        movingDuration = _movingDuration * lastSongIndex;

        //  Activate celebration
        if(UseCelebration) StartCoroutine(CelebrationSetActiveCoroutine(true, 0.5f));

        yield return new WaitForSeconds(movingDelay);

        while (timer < movingDuration)
        {
            levelsParent.localPosition = Vector3.Lerp(_initialPosition, finalPos, timer / movingDuration);
            timer += Time.deltaTime / movingDuration;
            yield return null;
        }
        
        //  Deactivate celebration
        if(UseCelebration) StartCoroutine(CelebrationSetActiveCoroutine(false));
    }

    IEnumerator CelebrationSetActiveCoroutine(bool setActive, float duration = 1f)
    {
        float timer;

        if (!setActive)
        {
            PlayerPrefs.SetInt("Celebrate", 0);
            
            timer = duration;
            
            while (timer > 0f)
            {
                foreach (var animation in celebrateAnimations)
                {
                    animation.SetAnimCurveValue(timer);
                }
                timer -= Time.deltaTime;

                yield return null;
            }
            
            celebrateParticle.Stop();
        }
        else
        {
            timer = 0f;
            
            while (timer < duration)
            {
                foreach (var animation in celebrateAnimations)
                {
                    animation.SetAnimCurveValue(timer);
                }
                timer += Time.deltaTime;

                yield return null;
            }
            
            celebrateParticle.Play();
        }
    }
}
