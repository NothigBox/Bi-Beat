using UnityEngine;
using System;
using UnityEngine.EventSystems;
 
public class ControllerButtons : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private float horizontalValue;
    
    public Action<float> OnButtonPressed;

    private bool isPressed;

    private void Awake()
    {
        isPressed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        OnButtonPressed?.Invoke(horizontalValue);
    }
 
    public void OnPointerUp(PointerEventData eventData){
        if(isPressed) return;
        
        OnButtonPressed?.Invoke(0f);
    }
}

