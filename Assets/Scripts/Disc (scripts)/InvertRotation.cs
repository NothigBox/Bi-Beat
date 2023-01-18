using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvertRotation : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        toggle.isOn = PlayerPrefs.GetInt("Rotation") == 1;
    }

    public bool IsOn
    {
        get
        {
            bool isOn = toggle.isOn;
            PlayerPrefs.SetInt("Rotation", isOn ? 1 : 0);
            return isOn;
        }
    }
}
