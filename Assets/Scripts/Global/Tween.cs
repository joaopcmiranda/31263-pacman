using UnityEngine;

public class Tween
{
    public Tween(Vector3 startPos, Vector3 endPos, float startTime, float duration)
    {
        
        this.startPos = startPos;
        this.endPos = endPos;
        this.startTime = startTime;
        this.duration = duration;
    }
    
    // Dud Tween
    public Tween(Vector3 pos)
    {
        startPos = pos;
        endPos = pos;
        startTime = Time.time;
        duration = 0.0001f;
    }

    public Vector3 startPos { get; private set; }
    public Vector3 endPos { get; private set; }
    public float startTime { get; private set; }
    public float duration { get; private set; }
}