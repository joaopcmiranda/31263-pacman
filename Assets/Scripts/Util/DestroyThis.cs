using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThis : MonoBehaviour
{
    public float delay;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyThisAfterDelay());
    }
    
    IEnumerator DestroyThisAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
