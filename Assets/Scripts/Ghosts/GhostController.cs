using UnityEngine;

public class GhostController : MonoBehaviour
{
    public bool isDead = false;
    
    public void Die()
    {
        isDead = true;
    }
}