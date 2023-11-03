using UnityEngine;

public enum GhostState
{
    Alive,
    Dead,
    Scared,
    Recovering
}

public class GhostManager : MonoBehaviour
{
    public GhostState state = GhostState.Alive;
    
    public void StopAllGhosts()
    {
        var ghosts = FindObjectsOfType<GhostController>();
        foreach (var ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().enabled = false;
            
        }
    }
}