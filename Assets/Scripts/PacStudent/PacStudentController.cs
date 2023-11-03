using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    private static readonly int WalkingParam = Animator.StringToHash("walking");
    private static readonly int DirectionParam = Animator.StringToHash("direction");
    public float speed;

    public AudioClip walkingOnEmptySound;
    public AudioClip walkingOnPelletSound;
    public AudioClip hitWallSound;

    // public GameObject dustParticleSystem;
    public ParticleSystem dustParticleSystem;
    public ParticleSystem abductionParticleSystem;

    public GameObject manager;

    // ReSharper disable once InconsistentNaming because assessment required specific variable naming
    private Direction currentInput = Direction.NONE;

    // ReSharper disable once InconsistentNaming because assessment required specific variable naming
    private Direction lastInput = Direction.NONE;

    private Animator m_Animator;
    private CherryController m_CherryController;
    private GhostManager m_GhostManager;
    private InputManager m_InputManager;

    private bool m_IsMoving;
    private bool m_locked = false;
    private Level1Manager m_LevelManager;
    private LifeManager m_LifeManager;

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
        m_LevelManager = manager.GetComponent<Level1Manager>();
        m_CherryController = manager.GetComponent<CherryController>();
        m_GhostManager = manager.GetComponent<GhostManager>();
        m_LifeManager = manager.GetComponent<LifeManager>();
        m_InputManager = manager.GetComponent<InputManager>();

        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool(WalkingParam, false);

        dustParticleSystem.Stop();

        m_Tween = new Tween(transform.position, transform.position, Time.time, 0.0001f);

        m_WalkingAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!m_locked)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                lastInput = Direction.UP;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                lastInput = Direction.RIGHT;
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                lastInput = Direction.DOWN;
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                lastInput = Direction.LEFT;
        }

        var timeFraction = (Time.time - m_Tween.startTime) / m_Tween.duration;
        if (timeFraction < 1.0f)
        {
            transform.position = Vector3.Lerp(m_Tween.startPos, m_Tween.endPos, timeFraction);
        }
        else if (!m_locked)
        {
            transform.position = m_Tween.endPos;

            // If the last input is not walkable, try the current input, otherwise stay still
            currentInput = m_LevelManager.IsTileWalkable(GetTargetPosition(m_Position, GetDirectionVector2(lastInput)))
                ? lastInput
                : m_LevelManager.IsTileWalkable(GetTargetPosition(m_Position, GetDirectionVector2(currentInput)))
                    ? currentInput
                    : Direction.NONE;

            if (currentInput != Direction.NONE)
            {
                m_IsMoving = true;
                dustParticleSystem.Play();
                MoveTo(currentInput);
            }
            else if (m_IsMoving)
            {
                m_IsMoving = false;
                m_WalkingAudioSource.Stop();
                m_WalkingAudioSource.loop = false;
                m_WalkingAudioSource.clip = hitWallSound;
                m_WalkingAudioSource.Play();

                dustParticleSystem.Stop();

                m_Animator.SetBool(WalkingParam, false);
            }
        }
    }


    // Collisions 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            m_LevelManager.PelletEaten(m_Position);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PowerPellet"))
        {
            m_LevelManager.PowerPelletEaten(m_Position);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Ghost"))
        {
            HandleGhostCollision(other);
        }
        else if (other.CompareTag("Cherry"))
        {
            m_LevelManager.CherryEaten();
            m_CherryController.CherryEaten();
        }
        else if (other.CompareTag("RightTeleporter"))
        {
            var rightTeleporter = other.GetComponent<RightTeleporterController>();
            var otherTeleporterPosition = rightTeleporter.leftTeleporter.transform.position;
            otherTeleporterPosition.x += 1;
            m_Position = new Vector2(-1, m_LevelManager.LEVEL_MAP.GetLength(1) / 2f + .5f);
            m_Tween = new Tween(transform.position, otherTeleporterPosition, Time.time, 0.00001f);
        }
        else if (other.CompareTag("LeftTeleporter"))
        {
            var leftTeleporter = other.GetComponent<LeftTeleporterController>();
            var otherTeleporterPosition = leftTeleporter.rightTeleporter.transform.position;
            otherTeleporterPosition.x -= 1;
            m_Position = new Vector2(m_LevelManager.LEVEL_MAP.GetLength(0) - 1,
                m_LevelManager.LEVEL_MAP.GetLength(1) / 2f + .5f);
            m_Tween = new Tween(transform.position, otherTeleporterPosition, Time.time, 0.00001f);
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

        if (m_LevelManager.GetTileOnPosition(m_Position) == TileContent.EMPTY)
            m_WalkingAudioSource.clip = walkingOnEmptySound;
        else if (m_LevelManager.GetTileOnPosition(m_Position) == TileContent.PELLET)
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

    private void HandleGhostCollision(Collider2D other)
    {
        var ghostController = other.GetComponent<GhostController>();
        if (ghostController.isDead) return;

        var globalGhostState = m_GhostManager.state;
        if (globalGhostState is GhostState.Scared or GhostState.Recovering)
        {
            ghostController.Die();
            m_LevelManager.GhostKilled();
        }
        else
        {
            if (m_LifeManager.LoseLife() <= 0) m_LevelManager.GameOver();
            Die();
        }
    }

    private void Die()
    {
        StopPlayer();

        // Animate death
        m_Animator.SetTrigger("die");
        m_Animator.SetBool("dead", true);
        
        // Particles
        dustParticleSystem.Stop();
        abductionParticleSystem.Play();
        
        // move to original position after 2 seconds if not game over
        if (m_LifeManager.lifeCount > 0)
        {
            Invoke(nameof(Respawn), 2f);
        }
    }

    private void Respawn()
    {
        m_Tween = new Tween(transform.position, new Vector3(2.56f, 28.5f, -3f), Time.time, 0.00001f);
        m_Animator.SetBool("dead", false);
        m_Position = new Vector2(1, 1);
        m_InputManager.enabled = true;
        m_locked = false;
    }
    
    public void StopPlayer()
    {
        m_WalkingAudioSource.Stop();
        m_Tween = new Tween(transform.position, transform.position, Time.time, 0.00001f);
        m_InputManager.enabled = false;
        m_IsMoving = false;
        m_locked = true;
        lastInput = Direction.NONE; // overriding to prevent player from moving after death
        currentInput = Direction.NONE;
        m_Animator.SetBool(WalkingParam, false);
    }
}