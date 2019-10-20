using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash_Spawner : MonoBehaviour
{
    // Variables
    public GameObject[] trashGOs;   // Different trash prefabs
    int GOchoice = 0;   // Holds array logic

    // Min and max int. Scrolls through the array betwee 0 and 2 to chose a random object
    int randomGOChoiceMin = 0;
    int randomGOChoiceMax = 2;

    // Trash Spawner Timers and logic
    float nextTimeToSpawn = 3f; // Next time we spawn a wave of trash
    float trashSpawnTime = 1f;  // How long it takes to spawn 1 object
    float trashSpawnWait = 2f;  // How long we wait until we spawn another object
    float trashCount = 5;   // How many objects we will spawn per waiting session

    // Value the game keeps in mind with how many pieces of trash are in the scene
    public static int maxTrash;
    // Where we store the positions of where we spawn the GameObjects on the x y and z
    Vector3 spawnPosition;
    // X spawn position boundaries
    float spawnPointMin_x = -480;
    float spawnPointMax_x = 480;

    // Y spawn position Boundaries
    float spawnPointMin_Y = 1;
    float spawnPointMax_Y = 80;

    // Z Spawn position Boundaries
    float spawnPointMin_Z = -480;
    float spawnPointMax_Z = 480;
    // Start is called before the first frame update
    void Start()
    {
        GOchoice = trashGOs.Length;
        // Spawn the trash
        StartCoroutine(SpawnTrash());
    }

    // Update is called once per frame
    void Update()
    {
        // Amount of trash in the scene
        Debug.Log(maxTrash);
        // Set random spawnpoints
        spawnPosition = new Vector3(Random.Range(spawnPointMin_x, spawnPointMax_x), Random.Range(spawnPointMin_Y, spawnPointMax_Y), Random.Range(spawnPointMin_Z, spawnPointMax_Z));
    }

    void Randomiser()
    {
        // pick a random object
        GOchoice = Random.Range(randomGOChoiceMin, randomGOChoiceMax);
        // Spawn only if we are not at 10 trash in the scene yet
        if (maxTrash < 10)
        {
            // Spawn the trash
            Instantiate(trashGOs[GOchoice], spawnPosition, Quaternion.identity);
            maxTrash += 1;  // increase value
        }
        else    // We are at 10 so we return nothing
            return;
    }
    // Spawning the trash 
    IEnumerator SpawnTrash()
    {
        yield return new WaitForSeconds(trashSpawnTime);
        while(true) 
        {
            for(int i = 0; i < trashCount; i++)
            {
                Randomiser();
                yield return new WaitForSeconds(trashSpawnWait);
            }
            yield return new WaitForSeconds(nextTimeToSpawn);
        }
    }
}
