using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform goal;
    Rigidbody RB_pc;
    // Start is called before the first frame update
    void Start()
    {
       RB_pc= gameObject.GetComponent<Rigidbody>();
       agent = GetComponent<NavMeshAgent>();
       // agent.destination = goal.position;
    }

    // Update is called once per frame
    void Update()
    {
        Movement3d();
    }

    void Movement3d()
    {
        
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    agent.destination = hit.point;
                }
            }
        if (Input.GetKey(KeyCode.Space))
        {
            agent.enabled = false;
            RB_pc.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
            RB_pc.AddRelativeForce(Vector3.up * 0.1f, ForceMode.Impulse);
        }
        //agent.enabled = true;
        // transform.Translate(Vector3.up * 260 * Time.deltaTime, Space.World);

    }
}
