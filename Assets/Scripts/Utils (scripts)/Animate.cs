using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animate : MonoBehaviour
{
    [SerializeField] protected AnimationCurve curve;
    [SerializeField] protected float duration;
    [SerializeField] protected float animationOffset;
    [SerializeField] protected bool playOnAwake = false;
    [SerializeField] protected bool loop = false;
    [SerializeField] protected bool useFixedUpdate = true;
    
    protected float timer = 0f;
    protected bool isPlaying = false;

    protected float CurrentPercentage => (timer / duration) + animationOffset;
    protected float CurrentPercentageCurveValue => curve.Evaluate(CurrentPercentage);

    private void Awake()
    {
        Initialize();
        
        if (playOnAwake) Play();
    }

    private void FixedUpdate()
    {
        if(!useFixedUpdate) return;
        
        if (isPlaying)
        {
            if (timer < duration || loop)
            {
                Animation();

                timer += Time.fixedDeltaTime;
            }
            else
            {
                timer = 0f;
                isPlaying = false;
            }
        }
    }
    private void Update()
    {
        if(useFixedUpdate) return;
        
        if (isPlaying)
        {
            if (timer < duration || loop)
            {
                Animation();

                timer += Time.unscaledDeltaTime;
            }
            else
            {
                timer = 0f;
                isPlaying = false;
            }
        }
    }

    public abstract void SetAnimCurveValue(float percentage);

    public void Play()
    {
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
    }

    protected abstract void Animation();
    protected abstract void Initialize();
}
