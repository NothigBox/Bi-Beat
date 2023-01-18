using System;
using UnityEngine;

public class DiscReceptors : MonoBehaviour
{
    public static Action OnReception;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            other.enabled = false;
            other.GetComponent<Note>().Kill(true);
            OnReception?.Invoke();
        }
    }
}
