using System;
using UnityEngine;
using UnityEngine.Rendering;

public class WorldManager : MonoBehaviour
{
    public static WorldManager ins;
    public float minute { get; private set; }
    public float hour { get; private set; }
    public float day { get; private set; }
    [SerializeField] private Volume lightVolume;
    public float timeSpeed = 6f;
    private void Awake()
    {
        if (ins == null) ins = this;
    }
    private void Start()
    {
        minute = 0;
        hour = 6;
        day = 1;
        InvokeRepeating(nameof(TimePass), timeSpeed, timeSpeed);
        UpdateLighting();
    }

    private void TimePass()
    {
        minute += 1;
        if (minute >= 60)
        {
            minute = 0;
            hour++;
            if (hour >= 24)
            {
                hour = 0;
                day++;
            }
        }

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        float timeFactor = 0f;
        if (hour >= 6f && hour < 7f)
        {
            float t = (minute / 60f) + (hour - 6f);
            timeFactor = Mathf.Lerp(0.5f, 0f, t);
        }
        else if (hour >= 7f && hour < 16f)
        {
            timeFactor = 0f;
        }
        else if (hour >= 16f && hour < 18f)
        {
            float t = (hour - 16f) + (minute / 60f);
            timeFactor = Mathf.Lerp(0f, 0.2f, t / 2f);
        }
        else if (hour >= 18f && hour < 19f)
        {
            float t = (hour - 18f) + (minute / 60f);
            timeFactor = Mathf.Lerp(0.2f, 0.5f, t);
        }
        else
        {
            timeFactor = 0.5f;
        }

        lightVolume.weight = timeFactor;
    }

}
