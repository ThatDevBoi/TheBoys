using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject hazzard;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(hazzard, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
