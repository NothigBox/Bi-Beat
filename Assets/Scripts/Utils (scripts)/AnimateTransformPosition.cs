using UnityEngine;

enum Axis { x, y, z, all }
public class AnimateTransformPosition : Animate
{
    [SerializeField] private Axis axis;
    [SerializeField] private float to;
    
    private Vector3 initialPosition;

    protected override void Initialize()
    {
        initialPosition = transform.position;
        
        if(playOnAwake) Play();
    }
    
    protected override void Animation()
    {
        switch (axis)
        {
            case Axis.x:
                transform.position = new Vector3
                (
                    initialPosition.x + (to * curve.Evaluate((timer / duration) + animationOffset)),
                    initialPosition.y,
                    initialPosition.z
                );
                break;

            case Axis.y:
                transform.position = new Vector3
                (
                    initialPosition.x,
                    initialPosition.y + (to * curve.Evaluate((timer / duration) + animationOffset)),
                    initialPosition.z
                );
                break;

            case Axis.z:
                transform.position = new Vector3
                (
                    initialPosition.x,
                    initialPosition.y,
                    initialPosition.z + (to * curve.Evaluate((timer / duration) + animationOffset))
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
                transform.position = new Vector3
                (
                    initialPosition.x + (to * curve.Evaluate(percentage)),
                    initialPosition.y,
                    initialPosition.z
                );
                break;
                    
            case Axis.y:
                transform.position = new Vector3
                (
                    initialPosition.x,
                    initialPosition.y + (to * curve.Evaluate(percentage)),
                    initialPosition.z
                );
                break;
                    
            case Axis.z:
                transform.position = new Vector3
                (
                    initialPosition.x,
                    initialPosition.y,
                    initialPosition.z + (to * curve.Evaluate(percentage))
                );
                break;
        }
    }
}
