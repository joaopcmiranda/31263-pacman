using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    public Vector3 startPos { get; private set; }
    public Vector3 endPos { get; private set; }
    public float startTime { get; private set; }
    public float duration { get; private set; }

    public Tween(Vector3 startPos, Vector3 endPos, float startTime, float duration)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.startTime = startTime;
        this.duration = duration;
    }
}