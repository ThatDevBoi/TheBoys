﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazzard : MonoBehaviour
{
    // Speed Value
    public float speed = 4;

    private void Update()
    {
        // Move gameObject left depending on the screen and Time
        transform.Translate(Vector3.left * speed * Time.deltaTime);
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
