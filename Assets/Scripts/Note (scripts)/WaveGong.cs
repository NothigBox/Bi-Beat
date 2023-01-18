using System;

public class WaveGong : Wave
{
    public static Action OnGongEnd;
    
    public override void TurnOff()
    {
        OnGongEnd?.Invoke();
        gameObject.SetActive(false);
    }
}
