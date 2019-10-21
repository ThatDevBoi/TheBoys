using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_school : MonoBehaviour
{
    public GameObject base_fish;
    public int quantity;
    private int i;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (i = 0; i <= quantity; i++)
            Instantiate(base_fish);
    }
}
