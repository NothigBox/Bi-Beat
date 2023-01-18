using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Limit : MonoBehaviour
{
    private AudioSource source;

    public static Action<Vector3> OnLimitReached;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            other.gameObject.GetComponent<Note>().Kill(false);
            source.Play();
            OnLimitReached?.Invoke(other.transform.position);
        }
    }
}
