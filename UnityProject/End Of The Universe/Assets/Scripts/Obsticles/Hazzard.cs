using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazzard : MonoBehaviour
{
    public float speed = 4;
    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Call GameOver Function in Level Manager
        }
    }

    public void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
