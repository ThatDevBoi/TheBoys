using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera PC_Cam;
    public Vector3 offset;
    public Transform Target;
    public float smoothTime = .5f;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        PC_Cam = Camera.main;
        Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraFollow();
    }

    void CameraFollow()
    {
        Vector3 pointOfAttention = new Vector3(Target.position.x, Target.position.y, transform.position.z - Target.position.z);
        Vector3 newPosition = pointOfAttention + offset;
        PC_Cam.transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
}
