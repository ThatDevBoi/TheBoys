using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Elevator : MonoBehaviour
{
    public GameObject miniLevel;
    public GameObject miniLevelrails;
    BoxCollider triggerZone;
    public Vector3 colliderSize;
    public Vector3 colliderCenter;
    [Space(20)]
    public Vector3 homePosition;
    public Vector3 endPosition;
    [Space(20)]
    [Header("Elevator Logic")]
    public float speed = 3;
    public GameObject playerCharacter;
    private bool parentPlayer;
    public bool elevate;
    public int triggerTime=0;
    public bool reachedPoint = false;


    // Start is called before the first frame update
    void Start()
    {
        // Make trigger collider
        triggerZone = gameObject.AddComponent<BoxCollider>();
        triggerZone.isTrigger = true;
        triggerZone.size = colliderSize;
        triggerZone.center = colliderCenter;
        if (miniLevel)
            miniLevel.SetActive(false);
        if (miniLevelrails)
            miniLevelrails.SetActive(false);
        if (GameObject.Find("PC") == null)
        {
            return;
        }
        else
        {
            // find the Player Character
            playerCharacter = GameObject.Find("PC");
        }
        // set up vector3 position
        homePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCharacter == null)
        {
            // find the Player Character
            playerCharacter = GameObject.Find("PC");
        }


        if(transform.position.y == endPosition.y)
        {
            reachedPoint = true;
        }
        else
        {
            reachedPoint = false;
        }

        //if (triggerTime >= 2)
        //    elevate = false;

        // Parent player to the elevator
        if (parentPlayer)
        {
            playerCharacter.transform.parent = gameObject.transform;
        }
        else
        {
            playerCharacter.transform.parent = null;
        }
        // go up
        if (elevate)
        {
            miniLevel.SetActive(true);
            miniLevelrails.SetActive(true);
            transform.position = Vector3.MoveTowards(transform.position, endPosition, Time.deltaTime * speed);
        }
        else if(!elevate && triggerTime >=2)    // go down
        {
            transform.position = Vector3.MoveTowards(transform.position, homePosition, Time.deltaTime * speed);
            if(transform.position.y == homePosition.y)
            {
                triggerTime = 0;
            }
        }
        // back up if the player jumps off before reaching the top
        else if(!reachedPoint && triggerTime < 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, homePosition, Time.deltaTime * speed);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "PC")
        {
            // when false
            if (!elevate)
            {
                GameObject.Find("PlayerUIController/Interact/Exposition_Text").GetComponent<TextMeshPro>().enabled = true;

                // when key is pressed
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // we can now go up
                    elevate = true;
                    // + 1
                    triggerTime++;
                    if (elevate)
                        GameObject.Find("PlayerUIController/Interact/Exposition_Text").GetComponent<TextMeshPro>().enabled = false;

                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PC")
        {
            parentPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "PC")
        {
            parentPlayer = false;
            elevate = false;
            GameObject.Find("PlayerUIController/Interact/Exposition_Text").GetComponent<TextMeshPro>().enabled = false;
            if (reachedPoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, homePosition, Time.deltaTime * speed);
            }
        }
    }
}
