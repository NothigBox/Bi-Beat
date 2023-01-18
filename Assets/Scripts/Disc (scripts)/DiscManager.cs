using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiscState
{
    normal,
    gong,
    celebrate
}

[RequireComponent(typeof(DiscMovement))]
[RequireComponent(typeof(AudioSource))]
public class DiscManager : MonoBehaviour
{
    [SerializeField] private Color normalStateColor;
    [SerializeField] private Color celebrateStateColor;
    [SerializeField] private Color gongStateColor;
    [SerializeField] private Vector2 gongPitchLimits;
    [SerializeField] private DiscState awakeState;
    
    private DiscMovement movement;
    public SpriteRenderer[] renderers;
    private Wave gongWave;
    private AudioSource source;
    private DiscState state;
    private bool rotation;
    
    public bool IsPlayingGong { get; private set; } 

    public Color NormalStateColor => normalStateColor;
    public Color CelebrateStateColor => celebrateStateColor;
    public Color GongStateColor => gongStateColor;

    private void Awake()
    {
        rotation = false;

        movement = GetComponent<DiscMovement>();
        source = GetComponent<AudioSource>();

        gongWave = GetComponentInChildren<Wave>();
        gongWave.TurnOff();
        
        SetDiscState(awakeState);
    }

    public void SetMovement(float horizontal, bool canMove = true)
    {
        if(horizontal != 0f) 
        {
            rotation = horizontal > 0f;
            if(!movement.invertRotation) rotation = !rotation; 
        }

        renderers[0].flipX = rotation;

        movement.Move(horizontal, canMove);
    }

    public void SetInvertRotation(bool isOn)
    {
        movement.invertRotation = isOn;
    }

    public void SetDiscState(DiscState state)
    {
        float gongAlpha = renderers[1].color.a;
        renderers[2].gameObject.SetActive(false);
        
        switch (state)
        {
            case DiscState.normal:
                renderers[0].color = normalStateColor;
                renderers[1].color = new Color( normalStateColor.r,  normalStateColor.g,  normalStateColor.b, gongAlpha);             
                break;
            
            case DiscState.gong:
                renderers[0].color = gongStateColor;
                renderers[1].color = new Color(gongStateColor.r, gongStateColor.g, gongStateColor.b, gongAlpha);
                renderers[2].gameObject.SetActive(true);
                break;
            
            case DiscState.celebrate:
                renderers[0].color = celebrateStateColor;
                renderers[1].color = new Color( celebrateStateColor.r,  celebrateStateColor.g,  celebrateStateColor.b, gongAlpha);   
                break;
        }

        this.state = state;
    }

    public bool PlayGong(bool force = false)
    {
        if(!force)
            if(state == DiscState.normal || IsPlayingGong) return false;

        IsPlayingGong = true;

        source.pitch = UnityEngine.Random.Range(gongPitchLimits.x, gongPitchLimits.y);
        source.Play();
        
        gongWave.gameObject.SetActive(true);
        gongWave.Play();

        return true;
    }

    public void StopGong()
    {
        IsPlayingGong = false;
    }
}
