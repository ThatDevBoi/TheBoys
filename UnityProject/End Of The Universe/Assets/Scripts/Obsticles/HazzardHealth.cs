using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazzardHealth : MonoBehaviour
{

    // Also known as health i called it durability as its not a person more of a thing 
    // Crates or rocks
    public float Durability = 50f;

    public void TakeDamage(float amount)
    {
        Durability -= amount;
        if (Durability <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
