using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialState
{
    buttons,
    notes,
    progression,
    shards,
    gong,
    speed
}

public class TutorialController : MonoBehaviour
{
    private const float MIN_CAN_PASS_STATE_DELAY = 0.8f;
    
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private GameObject notesPanel;
    [SerializeField] private GameObject progressionPanel;
    [SerializeField] private GameObject shardsPanel;
    [SerializeField] private GameObject gongPanel;
    [SerializeField] private GameObject speedPanel;
    [SerializeField] private GameObject passStateButton;

    private bool canPassState = false;

    private Action onStatePassed;

    public bool ProgressionShown { get; private set; }
    public bool SpeedShown { get; private set; }
    public bool GongShown { get; private set; }

    public void SetState(TutorialState state, bool activatePassButton = true, 
        Action onStatePassed = null, float canPassStateDelay = MIN_CAN_PASS_STATE_DELAY)
    {
        this.onStatePassed = onStatePassed;

        ClearScreen();
        
        switch (state)
        {
            case TutorialState.buttons:
                buttonsPanel.SetActive(true);
                break;

            case TutorialState.notes:
                notesPanel.SetActive(true);
                break;

            case TutorialState.progression:
                progressionPanel.SetActive(true);
                ProgressionShown = true;
                break;

            case TutorialState.shards:
                shardsPanel.SetActive(true);
                break;

            case TutorialState.gong:
                gongPanel.SetActive(true);
                GongShown = true;
                break;
            
            case TutorialState.speed:
                speedPanel.SetActive(true);
                SpeedShown = true;
                break;
        }

        bool passActivated = activatePassButton || onStatePassed != null;
        passStateButton.SetActive(passActivated);

        StartCoroutine(CanPassStateDelayCoroutine(canPassStateDelay));
    }

    public void PassState()
    {
        if(!canPassState) return;
            
        ClearScreen();
        onStatePassed?.Invoke();
    }

    public void ClearScreen()
    {
        buttonsPanel.SetActive(false);
        notesPanel.SetActive(false);
        progressionPanel.SetActive(false);
        shardsPanel.SetActive(false);
        gongPanel.SetActive(false);
        speedPanel.SetActive(false);

        passStateButton.SetActive(false);
    }

    IEnumerator CanPassStateDelayCoroutine(float canPassStateDelay)
    {
        canPassState = false;

        yield return new WaitForSecondsRealtime(canPassStateDelay);

        canPassState = true;
    }
}
