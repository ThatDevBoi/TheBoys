using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateObjective : MonoBehaviour
{
    public string newObjective;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PC")
        {
            GameObject.Find("PlayerUIController/Objective").GetComponent<TextMeshPro>().text = newObjective;
        }
    }
}
