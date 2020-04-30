using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateZip : MonoBehaviour
{
    public Player_Controller ZipelineFunction;
    public bool canZip = false;
    // Start is called before the first frame update
    void Start()
    {
        ZipelineFunction = GameObject.Find("PC").GetComponent<Player_Controller>(); // find the script
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(canZip);
        if(canZip == true &&  Input.GetKeyDown(ZipelineFunction.Player_Key_Binds[4]))
        {
            StartCoroutine(ZipelineFunction.UseZipline());
            ZipelineFunction.usezipeline = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PC")
        {
            canZip = !canZip;
        }
    }
}
