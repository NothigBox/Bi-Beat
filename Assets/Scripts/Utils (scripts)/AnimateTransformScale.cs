using UnityEngine;

public class AnimateTransformScale : Animate
{
    [SerializeField] private Axis axis;
    [SerializeField] private float to;
    
    private Vector3 initialScale;

    protected override void Initialize()
    {
        initialScale = transform.localScale;
        
        if(playOnAwake) Play();
    }
    
    protected override void Animation()
    {
        switch (axis)
        {
            case Axis.x:
                transform.localScale = new Vector3
                (
                    initialScale.x + (to * curve.Evaluate((timer / duration) + animationOffset)),
                    initialScale.y,
                    initialScale.z
                );
                break;

            case Axis.y:
                transform.localScale = new Vector3
                (
                    initialScale.x,
                    initialScale.y + (to * curve.Evaluate((timer / duration) + animationOffset)),
                    initialScale.z
                );
                break;

            case Axis.z:
                transform.localScale = new Vector3
                (
                    initialScale.x,
                    initialScale.y,
                    initialScale.z + (to * curve.Evaluate((timer / duration) + animationOffset))
                );
                break;
            
            case Axis.all:
                transform.localScale = new Vector3
                (
                    initialScale.x + (to * curve.Evaluate((timer / duration) + animationOffset)),
                    initialScale.y + (to * curve.Evaluate((timer / duration) + animationOffset)),
                    initialScale.z + (to * curve.Evaluate((timer / duration) + animationOffset))
                );
                break;
        }

        timer += Time.fixedDeltaTime;
    }
    
    public override void SetAnimCurveValue(float percentage)
    {
        switch (axis)
        {
            case Axis.x:
                transform.localScale = new Vector3
                (
                    initialScale.x + (to * curve.Evaluate(percentage)),
                    initialScale.y,
                    initialScale.z
                );
                break;
                    
            case Axis.y:
                transform.localScale = new Vector3
                (
                    initialScale.x,
                    initialScale.y + (to * curve.Evaluate(percentage)),
                    initialScale.z
                );
                break;
                    
            case Axis.z:
                transform.localScale = new Vector3
                (
                    initialScale.x,
                    initialScale.y,
                    initialScale.z + (to * curve.Evaluate(percentage))
                );
                break;
        }
    }
}
