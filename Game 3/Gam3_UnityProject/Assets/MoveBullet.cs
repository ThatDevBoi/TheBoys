using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Camera.main.transform.TransformDirection(Vector3.forward) * 3);

        Destroy(gameObject, 2f);
    }
}
