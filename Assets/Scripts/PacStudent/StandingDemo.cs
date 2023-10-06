using System.Collections;
using UnityEngine;

public class StandingDemo : MonoBehaviour
{
    private static readonly int WalkingParam = Animator.StringToHash("walking");
    private Animator m_Animator;
    private static readonly int DirectionParam = Animator.StringToHash("direction");

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool(WalkingParam, false);
        StartCoroutine(CycleDirections());
    }

    private IEnumerator CycleDirections()
    {
        while (true)
        {
            for (var i = 0; i < 4; i++)
            {
                m_Animator.SetInteger(DirectionParam, i);
                yield return new WaitForSeconds(3);
            }
        }
    }
}