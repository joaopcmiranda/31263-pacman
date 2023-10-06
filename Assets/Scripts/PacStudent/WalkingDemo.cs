using UnityEngine;

public class WalkingDemo : MonoBehaviour
{
    private static readonly int WalkingParam = Animator.StringToHash("walking");
    private static readonly int DirectionParam = Animator.StringToHash("direction");
    private Animator m_Animator;
    private int m_currentDirection = (int)Direction.RIGHT;
    private Tween m_Tween;
    public float speed;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool(WalkingParam, true);
        MoveToDirection(m_currentDirection);
    }

    private void Update()
    {
        var timeFraction = (Time.time - m_Tween.startTime) / m_Tween.duration;
        if (timeFraction < 1.0f)
            transform.position = Vector3.Lerp(m_Tween.startPos, m_Tween.endPos, timeFraction);
        else
        {
            m_currentDirection = (m_currentDirection + 1) % 4; // new direction cycling clockwise
            MoveToDirection(m_currentDirection);
        }
    }

    private void MoveToDirection(int direction)
    {
        Vector3 startPos = m_Tween?.endPos ?? transform.position;
        transform.position = startPos;
        m_Animator.SetInteger(DirectionParam, direction); // set new direction animation
        Vector3 movementVector = GetDirectionVector(); // new direction vector
        Vector3 endPos = transform.position + movementVector; // new end position
        float duration = movementVector.magnitude / speed; // new duration

        m_Tween = new Tween(startPos, endPos, Time.time, duration);
    }

    private Vector3 GetDirectionVector()
    {
        switch (m_currentDirection)
        {
            case (int)Direction.UP:
                return Vector3.up * 4;
            case (int)Direction.RIGHT:
                return Vector3.right * 5;
            case (int)Direction.DOWN:
                return Vector3.down * 4;
            case (int)Direction.LEFT:
                return Vector3.left * 5;
            default:
                return Vector3.zero;
        }
    }
}