using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    private static readonly int WalkingParam = Animator.StringToHash("walking");
    private static readonly int DirectionParam = Animator.StringToHash("direction");
    public float speed;

    public AudioClip walkingOnEmptySound;
    public AudioClip walkingOnPelletSound;
    public AudioClip hitWallSound;

    public GameObject dustParticleSystem;

    // ReSharper disable once InconsistentNaming because assessment required specific variable naming
    private Direction currentInput = Direction.NONE;

    // ReSharper disable once InconsistentNaming because assessment required specific variable naming
    private Direction lastInput = Direction.NONE;

    private Animator m_Animator;
    private ParticleSystem m_DustParticleSystem;

    private bool m_IsMoving;

    // I decided to use a different position than the world position because
    // the world position has some quirks that make it difficult to use like
    // the fact that it doesn't go from 0,0 at the top left, the 0,0 is on
    // the bottom left instead. So even though I need to also modify this when
    // moving, it makes the code a lot easier to read and understand. 
    private Vector2 m_Position = new(1, 1);
    private Tween m_Tween;
    private AudioSource m_WalkingAudioSource;


    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool(WalkingParam, false);

        m_DustParticleSystem = dustParticleSystem.GetComponent<ParticleSystem>();
        m_DustParticleSystem.Stop();

        m_Tween = new Tween(transform.position, transform.position, Time.time, 0.0001f);

        m_WalkingAudioSource = GetComponent<AudioSource>();
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
            currentInput = Level1Manager.IsTileWalkable(GetTargetPosition(m_Position, GetDirectionVector2(lastInput)))
                ? lastInput
                : Level1Manager.IsTileWalkable(GetTargetPosition(m_Position, GetDirectionVector2(currentInput)))
                    ? currentInput
                    : Direction.NONE;

            if (currentInput != Direction.NONE)
            {
                m_IsMoving = true;
                m_DustParticleSystem.Play();
                MoveTo(currentInput);
            }
            else if (m_IsMoving)
            {
                m_IsMoving = false;
                m_WalkingAudioSource.Stop();
                m_WalkingAudioSource.loop = false;
                m_WalkingAudioSource.clip = hitWallSound;
                m_WalkingAudioSource.Play();

                m_DustParticleSystem.Stop();

                m_Animator.SetBool(WalkingParam, false);
            }
        }
    }


    private void MoveTo(Direction direction)
    {
        var transformLocal = transform; // For performance reasons

        var startPos = m_Tween?.endPos ?? transformLocal.position;
        transformLocal.position = startPos;

        // set new direction animation
        m_Animator.SetInteger(DirectionParam, (int)direction);
        m_Animator.SetBool(WalkingParam, true);

        // new direction vector
        var movementVector = GetDirectionVector2(direction);

        // Update position with target position
        m_Position = GetTargetPosition(m_Position, movementVector);

        // Play walking sound
        m_WalkingAudioSource.Stop();

        if (Level1Manager.GetTileOnPosition(m_Position) == TileContent.EMPTY)
            m_WalkingAudioSource.clip = walkingOnEmptySound;
        else if (Level1Manager.GetTileOnPosition(m_Position) == TileContent.PELLET)
            m_WalkingAudioSource.clip = walkingOnPelletSound;

        m_WalkingAudioSource.loop = true;
        m_WalkingAudioSource.Play();

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

    private Vector2 GetTargetPosition(Vector2 currentPosition, Vector2 change)
    {
        return new Vector2(currentPosition.x + change.x, currentPosition.y - change.y);
    }
}