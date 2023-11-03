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
}