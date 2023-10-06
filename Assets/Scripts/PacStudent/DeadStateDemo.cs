using UnityEngine;

public class DeadStateDemo : MonoBehaviour
{
    private static readonly int Dead = Animator.StringToHash("dead");

    private void Start()
    {
        gameObject.GetComponent<Animator>().SetBool(Dead, true);
    }
}