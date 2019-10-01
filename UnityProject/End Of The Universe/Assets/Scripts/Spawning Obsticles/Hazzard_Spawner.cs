using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazzard_Spawner : MonoBehaviour
{
    public GameObject[] TilePatterns;

    private float timeBetweenSpawn;
    public float startTimeBetweenSpawn;
    public float decreaseTime;
    public float minTime = 0.65f;
    private void Update()
    {
        if(timeBetweenSpawn <=0)
        {
            int random = Random.Range(0, TilePatterns.Length);
            // Spawn Hazzard iun the scene
            Instantiate(TilePatterns[random], transform.position, Quaternion.identity);
            // Set the time of spawning
            timeBetweenSpawn = startTimeBetweenSpawn;
            if(startTimeBetweenSpawn > minTime) // if the start time is greater than the minTime
            {
                startTimeBetweenSpawn -= decreaseTime;  // Decrease the time
            }
        }
        else
        {
            timeBetweenSpawn -= Time.deltaTime;
        }
    }

}
