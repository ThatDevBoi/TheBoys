using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateZip : MonoBehaviour
{
    public Player_Controller zipfunction;
    public ActivateZip activation;
    public bool reset = false;
    public float timerReset = .1f;
    // Start is called before the first frame update
    void Start()
    {
        zipfunction = GameObject.Find("PC").GetComponent<Player_Controller>();
        activation = GameObject.Find("StartZip").GetComponent<ActivateZip>();
    }

    private void Update()
    {
        if (reset == true)
        {
            timerReset -= Time.deltaTime;
            if (timerReset >= 0)
            {
                zipfunction.time = 10;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        zipfunction.usezipeline = false;
        activation.canZip = false;
        StopCoroutine(zipfunction.UseZipline());
        zipfunction.time = 0;
        zipfunction.elapsedTime = 0;
        reset = true;
    }

    private void OnTriggerExit(Collider other)
    {
        timerReset = 0.1f;
        zipfunction.time = 10;
        reset = false;
    }
}
