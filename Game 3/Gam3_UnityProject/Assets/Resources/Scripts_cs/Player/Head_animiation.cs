using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_animiation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponentInParent<Player_Controller>().playerPhysics.velocity != Vector3.zero)
        {
            GetComponent<Animator>().SetTrigger("Walk");
           if(GetComponentInParent<Player_Controller>().running!=true)
            GameObject.Find("Pistol").GetComponent<Animator>().SetTrigger("Walking");
           else
                GameObject.Find("Pistol").GetComponent<Animator>().ResetTrigger("Walking");
        }
        else
        {
            GetComponent<Animator>().ResetTrigger("Walk");
            GameObject.Find("Pistol").GetComponent<Animator>().ResetTrigger("Walking");
        }
    }
}
