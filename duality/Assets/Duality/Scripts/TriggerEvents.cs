using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    public UnityEvent triggerEnter;

    public void OnTriggerEnter2D(Collider2D col)
    {
        triggerEnter?.Invoke();
    }
}
