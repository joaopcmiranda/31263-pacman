using System;
using UnityEngine;

public class BorderGhostController : MonoBehaviour
{

    private static readonly float MinX = -15.4f;
    private static readonly float MaxX = 15.5f;
    private static readonly float MinY = -14f;
    private static readonly float MaxY = 14f;

    private Vector2 m_MovingTarget;

    private Tween m_Tween;

    // Start is called before the first frame update
    private void Start()
    {

        // can start anywhere, find closest corner and go there
        var transformPosition = transform.position;
        var x = transformPosition.x;
        var y = transformPosition.y;

        int targetX;
        int targetY;

        if (Math.Abs(x - MinX) < Math.Abs(x - MaxX))
        {
            x = MinX;
            targetX = 0;
        }
        else
        {
            x = MaxX;
            targetX = 1;
        }

        if (Math.Abs(y - MinY) < Math.Abs(y - MaxY))
        {
            y = MinY;
            targetY = 0;
        }
        else
        {
            y = MaxY;
            targetY = 1;
        }


        var closestCorner = new Vector3(x, y, -4);
        m_MovingTarget = new Vector2(targetX, targetY);

        m_Tween = new Tween(transformPosition, closestCorner, Time.time, 0.0001f);
    }

    // Update is called once per frame
    private void Update()
    {
        var timeFraction = (Time.time - m_Tween.startTime) / m_Tween.duration;
        if (timeFraction < 1.0f)
        {
            transform.position = Vector3.Lerp(m_Tween.startPos, m_Tween.endPos, timeFraction);
        }
        else
        {
            transform.position = m_Tween.endPos;
            Vector3 endPos;

            // if currentCorner is 0,0 then target is 1,0, move there
            if (m_MovingTarget == new Vector2(0, 0))
            {
                endPos = new Vector3(MaxX, MinY, -4f);
                m_MovingTarget = new Vector2(1, 0);
            }
            else if (m_MovingTarget == new Vector2(1, 0))
            {
                endPos = new Vector3(MaxX, MaxY, -4f);
                m_MovingTarget = new Vector2(1, 1);
            }
            else if (m_MovingTarget == new Vector2(1, 1))
            {
                endPos = new Vector3(MinX, MaxY, -4f);
                m_MovingTarget = new Vector2(0, 1);
            }
            else
            {
                endPos = new Vector3(MinX, MinY, -4f);
                m_MovingTarget = new Vector2(0, 0);
            }

            var position = transform.position;
            m_Tween = new Tween(position, endPos, Time.time, CalculateDuration(position, endPos));
        }
    }

    private float CalculateDuration(Vector3 start, Vector3 end)
    {
        return (end - start).magnitude/10f;
    }
}