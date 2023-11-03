using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenPacStudent : MonoBehaviour
{
    
    private static readonly int WalkingParam = Animator.StringToHash("walking");
    private static readonly int DirectionParam = Animator.StringToHash("direction");
    
    private Animator m_Animator;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool(WalkingParam, false);
        
        m_Animator.SetInteger(DirectionParam, (int)Direction.LEFT);
        
    }

}
