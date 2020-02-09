using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PC");
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(Vector3.Distance(player.transform.position, GetComponentInParent<Transform>().position));
        
            transform.LookAt(player.transform.position);
            
        
        
    }
}
