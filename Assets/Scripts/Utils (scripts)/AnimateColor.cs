using UnityEngine;

public class AnimateColor : Animate
{
    [SerializeField] private Color finalColor;
    
    private Color initialColor;
    private SpriteRenderer renderer;

    protected override void Initialize()
    {
        renderer = GetComponent<SpriteRenderer>();

        initialColor = renderer.color;
    }

    protected override void Animation()
    {
        renderer.color = Color.Lerp(initialColor, finalColor, CurrentPercentageCurveValue);
    }

    public override void SetAnimCurveValue(float percentage) 
    {
        renderer.color = Color.Lerp(initialColor, finalColor, curve.Evaluate(percentage));
    }
}
