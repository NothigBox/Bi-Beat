using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateImageColor : Animate
{
    [SerializeField] private Color finalColor;

    private Color initialColor;
    private Image image;

    protected override void Initialize()
    {
        image = GetComponent<Image>();

        initialColor = image.color;
    }

    protected override void Animation()
    {
        image.color = Color.Lerp(initialColor, finalColor, CurrentPercentageCurveValue);
    }

    public override void SetAnimCurveValue(float percentage)
    {
        image.color = Color.Lerp(initialColor, finalColor, percentage);
    }
}
