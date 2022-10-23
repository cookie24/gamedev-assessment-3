using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeFormatter
{
    public static string FormatTime(float timer)
    {
        int intTimer = (int)timer;
        int minutes = intTimer / 60;
        int seconds = intTimer % 60;
        float fraction = timer * 100;
        fraction = Mathf.Floor(fraction % 100);
        string timerText = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
        return timerText;
    }
}
