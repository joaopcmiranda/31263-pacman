using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GhostState
{
    Normal,
    Dead,
    Scared,
    Recovering
}

public class GhostManager : MonoBehaviour
{
    public List<GhostController> ghosts;
    public GhostState state = GhostState.Normal;
    private BackgroundMusicManager m_BackgroundMusicManager;

    private GhostTimerManager m_GhostTimerManager;

    private void Start()
    {
        m_BackgroundMusicManager = GetComponent<BackgroundMusicManager>();
        m_GhostTimerManager = GetComponent<GhostTimerManager>();
    }

    public void StopAllGhosts()
    {
        foreach (var ghost in ghosts) ghost.enabled = false;
    }

    public void SetState(GhostState newState)
    {
        switch (newState)
        {
            case GhostState.Scared:
                m_GhostTimerManager.BeginTimer();
                m_BackgroundMusicManager.PlayScaredMusic();
                break;
            case GhostState.Normal:
                if (AreAllGhostsAlive())
                    m_BackgroundMusicManager.PlayNormalMusic();
                else
                    m_BackgroundMusicManager.PlayOneDeathMusic();
                break;
        }

        state = newState;
        foreach (var ghost in ghosts.Where(ghost => ghost.state != GhostState.Dead))
            ghost.SetState(newState);
    }

    private bool AreAllGhostsAlive()
    {
        return ghosts.All(ghost => ghost.state != GhostState.Dead);
    }
}