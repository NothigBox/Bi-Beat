using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    private Animator animator;

    private Animator _Animator
    {
        get
        {
            if(animator == null) animator = GetComponent<Animator>();
            return animator;
        }
    }
    
    public Action<Wave> OnWaveDeath;

    private void OnDisable()
    {
        OnWaveDeath = null;
    }

    public void Play()
    {
        _Animator.enabled = true;
        _Animator.Play(0);
    } 

    public virtual void TurnOff()
    {
        _Animator.enabled = false;
        OnWaveDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
}
