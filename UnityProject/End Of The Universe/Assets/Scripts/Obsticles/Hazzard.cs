using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazzard : MonoBehaviour
{
    // Speed Value
    public float speed = 4;
    public GameObject PC;
    public int x;
    private void Start()
    {
        //x = 7;
    }
    private void Update()
    {
        // Move gameObject left depending on the screen and Time
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // distance
        float distance = PC.transform.position.x - gameObject.transform.position.x;
        if (Mathf.Abs(distance) > x)
            Destroy(gameObject);
    }

    // Kill Object Off screen
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Death")
        {
            Destroy(gameObject);
        }
    }
}
