using System;
using System.Collections;
using UnityEngine;

public class TimeTickManager : MonoBehaviour
{
    public static TimeTickManager instance;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private float tickLength = 0.2f;
    public event Action OnTick;

    private void Start()
    {
        StartCoroutine(TickRoutine());
    }

    private IEnumerator TickRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickLength);
            OnTick?.Invoke();
        }
    }
}
