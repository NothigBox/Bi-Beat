using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Progression : MonoBehaviour
{
    private List<Transform> indicators;
    private ParticleSystem[] particles;

    public int Counter { get; set; }

    public static Action OnGongReached;

    private void Awake()
    {
        indicators = new List<Transform>();
        var _indicators = GameObject.FindGameObjectsWithTag("Indicator").ToList();
        foreach (var i in _indicators) indicators.Add(i.transform);
        
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.SetActive(false);
        OnGongReached?.Invoke();
    }

    public void SetPercentage(float percentage)
    {
        foreach (var i in indicators)
        {
            Vector3 localScale = i.localScale;
            i.localScale = new Vector3(percentage, localScale.y, localScale.z);
            
            PlayParticles();
        }
    }

    public void PlayParticles(bool loop = false)
    {
        foreach (var p in particles)
        {
            var main = p.main;
            main.loop = loop;
            p.Play();
        }
    }
    
    public void StopParticles()
    {
        foreach (var p in particles) p.Stop();
    }

    public void SetColor(Color color)
    {
        foreach (var i in indicators)
        {
            if (i == transform) continue;

            i.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
