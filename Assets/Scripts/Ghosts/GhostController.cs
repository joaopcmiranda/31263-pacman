using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

public abstract class GhostController : MonoBehaviour
{
    private static readonly int ScaredStr = Animator.StringToHash("scared");
    private static readonly int DeadStr = Animator.StringToHash("dead");
    private static readonly int RecoveringStr = Animator.StringToHash("recovering");
    private static readonly int DirectionStr = Animator.StringToHash("direction");

    public GhostState state = GhostState.Normal;
    public float speed = 1f;

    public bool isDead;

    protected Direction LastMovementDirection = Direction.NONE;


    private Animator m_Animator;
    private Collider2D m_Collider2D;
    private bool m_Locked = true;

    private Tween m_Tween;
    
    public Level1Manager levelManager;
    [FormerlySerializedAs("manager")] public GhostManager ghostManager;

    protected PacStudentController Player;
    
    private List<Direction> m_PathOutOfBox;
    
    public abstract Vector2 initialPosition { get; }
    private Vector3 m_InitialWorldPosition;

    // I decided to use a different position than the world position because
    // the world position has some quirks that make it difficult to use like
    // the fact that it doesn't go from 0,0 at the top left, the 0,0 is on
    // the bottom left instead. So even though I need to also modify this when
    // moving, it makes the code a lot easier to read and understand. 
    protected Vector2 Position ;

    protected void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Collider2D = GetComponent<Collider2D>();

        Player = GameObject.FindWithTag("Player").GetComponent<PacStudentController>();
        
        Position = initialPosition;
        
        m_InitialWorldPosition = transform.position;

        m_Tween = new Tween(m_InitialWorldPosition);
        
        m_PathOutOfBox = GetPathOutOfBox();
    }


    private void Update()
    {
        var timeFraction = (Time.time - m_Tween.startTime) / m_Tween.duration;
        if (timeFraction < 1.0f)
        {
            transform.position = Vector3.Lerp(m_Tween.startPos, m_Tween.endPos, timeFraction);
        } else if (state == GhostState.Dead)
        {
            SetState(ghostManager.state);
            m_PathOutOfBox = GetPathOutOfBox();
            Position = initialPosition;
        }
        else if (!m_Locked)
        {
            transform.position = m_Tween.endPos;

            if (m_PathOutOfBox.Count > 0)
            {
                MoveTo(m_PathOutOfBox[0]);
                m_PathOutOfBox.RemoveAt(0);
            }
            else if (state == GhostState.Scared)
                MoveTo(GetNextInputScared());
            else
                MoveTo(GetNextInput());
        }
    }

    private void MoveTo(Direction direction)
    {
        LastMovementDirection = direction;

        var transformLocal = transform; // For performance reasons

        var startPos = m_Tween?.endPos ?? transformLocal.position;
        transformLocal.position = startPos;

        // set new direction animation
        m_Animator.SetInteger(DirectionStr, (int)direction);

        // new direction vector
        var movementVector = MovementUtil.GetDirectionVector2(direction);

        // Update position with target position
        Position = MovementUtil.GetTargetPosition(Position, movementVector);

        // new end position - convert to Vector3 to for compatibility with transform.position
        var endPos = transform.position + (Vector3)movementVector;

        // new duration
        var duration = movementVector.magnitude / speed;

        // new tween
        m_Tween = new Tween(startPos, endPos, Time.time, duration);
    }

    /**
     * This method is called to get ne next movement of the ghost
     * Being abstract, it is up to the child class to implement it
     * Each ghost will have a different implementation
     */
    protected abstract Direction GetNextInput();
    
    
    protected abstract List<Direction> GetPathOutOfBox();

    // find the direction that will take the ghost away from the player
    // dont go back the way you came
    protected Direction GetNextInputScared()
    {
        var playerPosition = Player.GetPosition();

        var forbiddenDirection = MovementUtil.GetOppositeDirection(LastMovementDirection);

        return MovementUtil.FindFurthestDirectionFromPlayer(Position, playerPosition, forbiddenDirection, levelManager);
    }


    public void Begin()
    {
        m_Locked = false;
    }

    public void Die()
    {
        isDead = true;
        SetState(GhostState.Dead);
        m_Collider2D.enabled = false;
        Invoke(nameof(Revive), 5f);
    }

    public void Revive()
    {
        isDead = false;
        m_Collider2D.enabled = true;
        SetState(GhostState.Normal);
    }

    public void SetState(GhostState newState)
    {
        switch (newState)
        {
            case GhostState.Scared:
                m_Animator.SetBool(ScaredStr, true);
                m_Animator.SetBool(DeadStr, false);
                m_Animator.SetBool(RecoveringStr, false);
                break;
            case GhostState.Dead:
                m_Animator.SetBool(ScaredStr, false);
                m_Animator.SetBool(DeadStr, true);
                m_Animator.SetBool(RecoveringStr, false);
                var distance = Vector3.Distance(transform.position, m_InitialWorldPosition);
                m_Tween = new Tween(transform.position, m_InitialWorldPosition, Time.time, distance / speed);
                break;
            case GhostState.Recovering:
                m_Animator.SetBool(ScaredStr, false);
                m_Animator.SetBool(DeadStr, false);
                m_Animator.SetBool(RecoveringStr, true);
                break;
            case GhostState.Normal:
                m_Animator.SetBool(ScaredStr, false);
                m_Animator.SetBool(DeadStr, false);
                m_Animator.SetBool(RecoveringStr, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        state = newState;
    }
}