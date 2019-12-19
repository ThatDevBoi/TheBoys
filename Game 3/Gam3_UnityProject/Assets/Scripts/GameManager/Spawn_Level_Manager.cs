using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Level_Manager : MonoBehaviour
{

    public GameObject LevelManagerPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        if(!FindManager())
        {
            LevelManagerPrefab = Resources.Load<GameObject>("Level_Manager/LevelManager");
            GameObject currentObject = this.gameObject;
            Instantiate(LevelManagerPrefab, transform.position, Quaternion.identity);
            Destroy(currentObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    bool FindManager()
    {
        if (GameObject.Find("Level_Manager") == null)
        {
            // not in the scene
            return false;
        }
        else
            // in the scene
            return true;
    }
}
