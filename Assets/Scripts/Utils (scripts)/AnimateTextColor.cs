using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateTextColor : Animate
{
    [SerializeField] private Color finalColor;

    private Color initialColor;
    private Text text;

    protected override void Initialize()
    {
        text = GetComponent<Text>();

        initialColor = text.color;
    }

    protected override void Animation()
    {
        text.color = Color.Lerp(initialColor, finalColor, CurrentPercentageCurveValue);
    }

    public override void SetAnimCurveValue(float percentage)
    {
        text.color = Color.Lerp(initialColor, finalColor, percentage);
    }
}
