using UnityEngine;

public class DeathAnimationDemo : MonoBehaviour
{
    private static readonly int DeathAnimation = Animator.StringToHash("death_animation");

    private void Start()
    {
        gameObject.GetComponent<Animator>().SetBool(DeathAnimation, true);
    }
}