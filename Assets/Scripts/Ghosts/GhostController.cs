using System;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public GhostState state = GhostState.Normal;

    public bool isDead;

    private Animator m_Animator;
    private Collider2D m_Collider2D;
    
    private static readonly int Scared = Animator.StringToHash("scared");
    private static readonly int Dead = Animator.StringToHash("dead");
    private static readonly int Recovering = Animator.StringToHash("recovering");

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Collider2D = GetComponent<Collider2D>();
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
                m_Animator.SetBool(Scared, true);
                m_Animator.SetBool(Dead, false);
                m_Animator.SetBool(Recovering, false);
                break;
            case GhostState.Dead:
                m_Animator.SetBool(Scared, false);
                m_Animator.SetBool(Dead, true);
                m_Animator.SetBool(Recovering, false);
                break;
            case GhostState.Recovering:
                m_Animator.SetBool(Scared, false);
                m_Animator.SetBool(Dead, false);
                m_Animator.SetBool(Recovering, true);
                break;
            case GhostState.Normal:
                m_Animator.SetBool(Scared, false);
                m_Animator.SetBool(Dead, false);
                m_Animator.SetBool(Recovering, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        state = newState;
    }
}