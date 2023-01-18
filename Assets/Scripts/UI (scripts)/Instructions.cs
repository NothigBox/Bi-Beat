using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    private const float ENABLE_DELAY = 0.5f;
    
    [SerializeField] private float timeBetweenInstructions;
    [SerializeField] private GameObject[] hands;
    [SerializeField] private GameObject wrong;

    private int instructionIndex;
    private bool canSkip;
    
    private void OnEnable()
    {
        instructionIndex = 0;
        canSkip = false;
        
        StartCoroutine(AnimatingCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        Time.timeScale = 1;
    }

    public void Skip()
    {
        if(!canSkip) return;
        gameObject.SetActive(false);
    }

    IEnumerator AnimatingCoroutine()
    {
        Time.timeScale = 0;
        
        yield return new WaitForSecondsRealtime(ENABLE_DELAY);
        canSkip = true;
        
        while (true)
        {
            switch (instructionIndex)
            {
                case 0:
                    hands[0].SetActive(true);
                    hands[1].SetActive(false);
                    wrong.SetActive(false);
                break;
                
                case 1:
                    hands[0].SetActive(true);
                    hands[1].SetActive(true);
                    wrong.SetActive(true);
                    break;
                
                case 2:
                    hands[0].SetActive(false);
                    hands[1].SetActive(true);
                    wrong.SetActive(false);
                    break;
            }

            instructionIndex++;

            if (instructionIndex > 2) instructionIndex = 0;
            
            yield return new WaitForSecondsRealtime(timeBetweenInstructions);
        }
    }
}
