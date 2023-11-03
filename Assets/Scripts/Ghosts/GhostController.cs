using UnityEngine;

public class GhostController : MonoBehaviour
{
    public GhostState state = GhostState.Normal;
    
    public bool isDead = false;
    
    public void Die()
    {
        isDead = true;
    }
    
    public void SetState(GhostState newState)
    {
        state = newState;
    }
}