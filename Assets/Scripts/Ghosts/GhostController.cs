using System;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public GhostState state = GhostState.Normal;
    
    private Animator m_Animator;
    
    public bool isDead = false;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void Die()
    {
        isDead = true;
    }
    
    public void SetState(GhostState newState)
    {
        switch (newState)
        {
            case GhostState.Scared:
                m_Animator.SetBool("scared", true);
                m_Animator.SetBool("dead", false);
                m_Animator.SetBool("recovering", false);
                break;
            case GhostState.Dead:
                m_Animator.SetBool("scared", false);
                m_Animator.SetBool("dead", true);
                m_Animator.SetBool("recovering", false);
                break;
            case GhostState.Recovering:
                m_Animator.SetBool("scared", false);
                m_Animator.SetBool("dead", false);
                m_Animator.SetBool("recovering", true);
                break;
            case GhostState.Normal:
                m_Animator.SetBool("scared", false);
                m_Animator.SetBool("dead", false);
                m_Animator.SetBool("recovering", false);
                break;
        }
        state = newState;
    }
}