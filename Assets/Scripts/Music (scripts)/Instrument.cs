using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    [SerializeField] private float timeBetweenNotes;
    [SerializeField] private Sprite[] sprites;

    private SpriteRenderer renderer;
    private AnimateColor animation;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        animation = GetComponent<AnimateColor>();

        DiscReceptors.OnReception += ChangeSprite;
    }

    private void ChangeSprite()
    {
        StartCoroutine(ChangeSpriteCoroutine());
    }
    
    public void PlayAnimation()
    {
        animation.Play();
        ChangeSprite();
    }

    IEnumerator ChangeSpriteCoroutine() 
    {
        int r = UnityEngine.Random.Range(0, sprites.Length);

        yield return new WaitForSeconds(timeBetweenNotes);

        renderer.sprite = sprites[r];
    }
}

