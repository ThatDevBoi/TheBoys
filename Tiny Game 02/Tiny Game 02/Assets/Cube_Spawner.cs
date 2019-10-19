using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Spawner : MonoBehaviour
{
    // reference to spawn object
    public GameObject rotating_cube;
    // Minimum amount of space we cover when spawning 
    public float minSpawn = 100;
    // maximum amount of space we cover when spawning 
    public float maxSpawn = 1250;
    // The Position we are spawning our objects from
    public Vector3 spawnRange;
    // How fast we spend spawning objects
    public float spawnTime = .2f;
    // reset the spawner cooldown value
    public float resetTime;
    // Start is called before the first frame update
    void Start()
    {
        // Make sure reset spawner adobts the value of spawnTime
        // So when we reset the spawner it will have the correct values
        resetTime = spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Set the spawn position to the camera pint
        spawnRange = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(minSpawn, maxSpawn), Screen.height / 10, Camera.main.nearClipPlane + 5));
        spawnTime -= Time.deltaTime;    // Decrease float value
        if(spawnTime <= 0)  // When value is 0 or more
        {
            // Spawn the cube
            GameObject CubeAnim = Instantiate(rotating_cube, spawnRange, Quaternion.identity) as GameObject;
            // Reset spawn timer
            spawnTime = resetTime;
        }
    }
}
