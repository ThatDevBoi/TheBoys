using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyClusterHolder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(gameObject.transform.Find("Enemy")==null)
            Destroy(gameObject);
    }
}
