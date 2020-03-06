using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_OpenDoor : MonoBehaviour
{
    [Header("AudioSource Settings")]
    public float volume;
    public float pitch;
    public int prority;

    AudioSource playSound;
    [Header("Trigger Altercation")]
    BoxCollider triggerZone;
    public Vector3 triggerSize;
    public Vector3 triggerOffset;
    [Space(20)]
    Transform playerPosition;
    public Vector3 doorMovePosition;
    Vector3 startPosition;
    bool doorClosed = true;
    bool play = false, allowedToPlay = true;
    // Start is called before the first frame update
    void Start()
    {
        // if there is no Audio Source on the object
        if(gameObject.GetComponent<AudioSource>() == null)
        {
            // Make one
            playSound = gameObject.AddComponent<AudioSource>();
            playSound.playOnAwake = false;  // Dont play sound when in runtime
            // Find the open door clip
            playSound.clip = Resources.Load<AudioClip>("Audio_AS/Objects_AC_obj/OpenDoor");
            playSound.volume = volume;
            playSound.pitch = pitch;
            playSound.priority = prority;
        }
        else   // if there is an audio source
        {
            playSound = gameObject.GetComponent<AudioSource>(); // variable finds current audio source
            playSound.playOnAwake = false;  // dont play on runtime
            if(playSound.clip == null)  // if there is no clip applied
            {
                //apply it for us
                playSound.clip = Resources.Load<AudioClip>("Audio_AS/Objects_AC_obj/OpenDoor");
            }
        }

        if(gameObject.GetComponent<BoxCollider>() == null)
        {
            triggerZone = gameObject.AddComponent<BoxCollider>();
            triggerZone.isTrigger = true;
            triggerZone.size = triggerSize;
            triggerZone.center = triggerOffset;
        }
        else
        {
            triggerZone = gameObject.GetComponent<BoxCollider>();
            triggerZone.isTrigger = true;
            triggerZone.size = triggerSize;
            triggerZone.center = triggerOffset;
        }
        startPosition = this.gameObject.transform.localPosition;
        triggerZone.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerPosition == null)
        {
            playerPosition = GameObject.Find("PC").GetComponent<Transform>();
        }

        if (play)
        {
            playSound.Play();
            play = false;
            allowedToPlay = false;
        }

        if (!doorClosed)
        {
            // Open the door
            this.transform.localPosition = Vector3.Slerp(transform.localPosition, doorMovePosition, Time.time);
        }
        else
        {
            allowedToPlay = true;
            // Close the door
            this.transform.localPosition = Vector3.Slerp(transform.localPosition, startPosition, Time.time);
        }

        if (Vector3.Distance(playerPosition.position, this.transform.position) < 5)
        {
            if(allowedToPlay)
            {
                play = true;
            }
            triggerZone.enabled = true;
            doorClosed = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PC")
        {
            triggerZone.enabled = false;
            doorClosed = true;
            playSound.Play();
        }
    }
}
