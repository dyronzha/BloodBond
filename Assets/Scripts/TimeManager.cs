using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager
{
    static TimeManager S_TimeManager;

    float timeOffset = 1.0f;

    public float GetTimeOffset() {
        return timeOffset;
    }
    public void ModifyTimeOffset(float value) {
        timeOffset = value;
    }
}
