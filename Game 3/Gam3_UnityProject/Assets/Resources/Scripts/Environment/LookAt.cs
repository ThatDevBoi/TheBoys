using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private GameObject player;
    public bool chargingPort=true;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PC");
        if (GetComponentInParent<GameObject>().name == "Charging Port")
            chargingPort = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Vector3.Distance(player.transform.position, GetComponentInParent<Transform>().position));
        if (player != null)
        {
            
            float distance = Vector3.Distance(player.transform.position, GetComponent<Transform>().position);
            if (chargingPort)
            {
                print("Distance to other: " + distance);

                if (distance < 35f)
                {
                   // GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
                    var lookPos = player.transform.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime );

                }
                else
                {
                    var lookPos = player.transform.position - transform.position;
                    //lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);

                   // transform.LookAt(player.transform.position);
                }
            }
        }
    }
}
