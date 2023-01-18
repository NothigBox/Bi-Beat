using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticlesPool : MonoBehaviour
{
    [SerializeField] private GameObject darkWaveParticlePrefab;
    [SerializeField] private GameObject lightWaveParticlePrefab;
    [SerializeField] private GameObject killWaveParticlePrefab;

    private Queue<Wave> lightWaveParticles;
    private Queue<Wave> darkWaveParticles;

    private void Awake()
    {
        lightWaveParticles = new Queue<Wave>();
        darkWaveParticles = new Queue<Wave>();
    }

    public Wave SpawnDarkWaveAtPosition(Vector3 position)
    {
        if (darkWaveParticles.Count <= 0)
        {
            Wave wave = Instantiate(darkWaveParticlePrefab, position, Quaternion.identity)
                .GetComponent<Wave>();
            wave.Play();

            wave.OnWaveDeath = darkWaveParticles.Enqueue;
            
            darkWaveParticles.Enqueue(wave);
        }

        Wave _wave = darkWaveParticles.Dequeue();
        _wave.gameObject.SetActive(true);
        _wave.transform.position = position;
        _wave.Play();
        
        return _wave;
    }
    
    public Wave SpawnLightWaveAtPosition(Vector3 position)
    {
        if (lightWaveParticles.Count <= 0)
        {
            Wave wave = Instantiate(lightWaveParticlePrefab, Vector3.up * 100f, Quaternion.identity)
                .GetComponent<Wave>();
            
            wave.gameObject.SetActive(false);

            lightWaveParticles.Enqueue(wave);
        }

        Wave _wave = lightWaveParticles.Dequeue();
        _wave.gameObject.SetActive(true);
        _wave.transform.position = position;
        _wave.OnWaveDeath = lightWaveParticles.Enqueue;
        _wave.Play();
        
        return _wave;
    }
    
    public Wave SpawnKillWaveAtPosition(Vector3 position)
    {
        Wave wave = Instantiate(killWaveParticlePrefab, position, Quaternion.identity)
             .GetComponent<Wave>();
        
        wave.Play();
            
        return wave;
    }
}
