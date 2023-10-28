using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    private static readonly int WalkingParam = Animator.StringToHash("walking");
    private static readonly int DirectionParam = Animator.StringToHash("direction");
    public float speed;

    // ReSharper disable once InconsistentNaming because assessment required specific variable naming
    private Direction currentInput;

    // ReSharper disable once InconsistentNaming because assessment required specific variable naming
    private Direction lastInput;

    private Animator m_Animator;

    // I decided to use a different position than the world position because
    // the world position has some quirks that make it difficult to use like
    // the fact that it doesn't go from 0,0 at the top left, the 0,0 is on
    // the bottom left instead. So even though I need to also modify this when
    // moving, it makes the code a lot easier to read and understand. 
    private Vector2 m_Position = new(1, 1);
    private Tween m_Tween;


    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool(WalkingParam, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            lastInput = Direction.UP;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            lastInput = Direction.RIGHT;
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            lastInput = Direction.DOWN;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            lastInput = Direction.LEFT;

        var timeFraction = (Time.time - m_Tween.startTime) / m_Tween.duration;
        if (timeFraction < 1.0f)
        {
            transform.position = Vector3.Lerp(m_Tween.startPos, m_Tween.endPos, timeFraction);
        }
        else
        {
            transform.position = m_Tween.endPos;

            // If the last input is not walkable, try the current input, otherwise stay still
            currentInput = LevelManager.CheckPositionWalkable(m_Position + GetDirectionVector2(lastInput)) ? lastInput
                : LevelManager.CheckPositionWalkable(m_Position + GetDirectionVector2(currentInput)) ? currentInput
                : Direction.NONE;

            MoveTo(currentInput);
        }
    }


    private void MoveTo(Direction direction)
    {
        var transformLocal = transform; // For performance reasons

        var startPos = m_Tween?.endPos ?? transformLocal.position;
        transformLocal.position = startPos;

        // set new direction animation
        m_Animator.SetInteger(DirectionParam, (int)direction);

        // new direction vector
        var movementVector = GetDirectionVector2(direction);

        // Update position with target position
        m_Position += movementVector;

        // new end position - convert to Vector3 to for compatibility with transform.position
        var endPos = transform.position + (Vector3)movementVector;

        // new duration
        var duration = movementVector.magnitude / speed;

        m_Tween = new Tween(startPos, endPos, Time.time, duration);
    }

    private Vector2 GetDirectionVector2(Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                return Vector2.up;
            case Direction.RIGHT:
                return Vector2.right;
            case Direction.DOWN:
                return Vector2.down;
            case Direction.LEFT:
                return Vector2.left;
            default:
                return Vector2.zero;
        }
    }
}