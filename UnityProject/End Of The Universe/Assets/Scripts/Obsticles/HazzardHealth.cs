using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazzardHealth : MonoBehaviour
{

    // Also known as health i called it durability as its not a person more of a thing 
    // Crates or rocks
    public float Durability = 50f;

    private void Awake()
    {
        // We need to define the layer as whenever the crate is not called crate
        // The gun wont force it back or even interact with it
        gameObject.layer = 8;
    }

    private void Update()
    {
        if(gameObject.name == "Mini Crate(Clone)")
        {
            gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, transform.rotation.y, transform.rotation.z, 0);
        }
    }

    public void TakeDamage(float amount)
    {
        Durability -= amount;
        if (Durability <= 0f)
        {
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {
        yield return new WaitForSeconds(.4f);
        Destroy(this.gameObject);
    }
}
