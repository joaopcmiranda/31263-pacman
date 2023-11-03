using System.Collections.Generic;
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
    
    private GhostTimerManager m_GhostTimerManager;
    
    public void StopAllGhosts()
    {
        foreach (var ghost in ghosts)
        {
            ghost.enabled = false;
            
        }
    }
    
    public void SetState( GhostState newState)
    {
        switch (newState)
        {
            case GhostState.Scared:
                m_GhostTimerManager.BeginTimer();
                break;
        }
        state = newState;
        foreach (var ghost in ghosts)
        {
            if (ghost.state != GhostState.Dead)
            {
                ghost.SetState(newState);
            }
        }
    } 
}