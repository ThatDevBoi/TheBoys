using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Swarmer : MonoBehaviour
{
    // The Crate We Spawn
    public GameObject smallCrate;
    [SerializeField]
    // The amount of crates we want to spawn
    // Keep it private we dont really want to edit this value unless needed
    int amountToSpawn = 10;
    // Vector2 x and y position
    public float maxpositionx, minpositionx;
    public float maxpositiony, minpositiony;
    // Position we spawn the cubes
    public Transform miniCrateSpawnPoint;
    public Sprite turnOffSprite;
    public Vector2 currentposition = Vector2.zero;
    private bool canSpawnSwarm = false;
    HazzardHealth objectHealth;

    // Start is called before the first frame update
    void Start()
    {
        amountToSpawn = 10;
        // Decaring Current Position in the world
        currentposition = new Vector2(miniCrateSpawnPoint.position.x, miniCrateSpawnPoint.position.y);
        // Finding Script
        objectHealth = GetComponent<HazzardHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (objectHealth.Durability <= 0)
            canSpawnSwarm = true;
        // Functions
        if(canSpawnSwarm)
        {
            Swarm();
        }
    }

    public void Swarm()
    {
        if (amountToSpawn > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = turnOffSprite;
            // Where is these crates going to spawn
            currentposition = new Vector2(miniCrateSpawnPoint.position.x, miniCrateSpawnPoint.position.y);
            maxpositionx = currentposition.x;
            maxpositiony = currentposition.y;
            int toSpawn = Random.Range(amountToSpawn, amountToSpawn);
            Vector2 crateSpawn = new Vector2(Random.Range(maxpositionx, maxpositiony), maxpositiony);
            GameObject GO = Instantiate(smallCrate, crateSpawn, Quaternion.identity) as GameObject;
            amountToSpawn--;
        }
        else
            return;
    }
}
