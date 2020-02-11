using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charging_Port : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "Charging Port";
        player = GameObject.Find("PC");
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
           
        }

    }
}
