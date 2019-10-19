using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region KeyNote Before Alteration
    // These values i have set work best in my opinion from testing them during runtime.
    // Edit them but remember that you could risk breaking the immersion of an underwater environment
    // Also remember that the audio is controlled in groups from the master audio mixer
    // You might want to edit the volume control if you play around in this script
    #endregion

    #region Variables
    // This is for the whale noise
    public AudioSource PC_AD;

    private float fadTime = 5f;

    private float minTimeOff = 5f;
    private float maxTimeOff = 15f;
    float timeOff;

    private float minTimeOn = 5f;
    private float maxTimeOn = 15f;
    float timeOn;

    private float minVol = 0.5f;
    private float maxVol = 1;
    float whaleVol;

    private float timer;
    private float timeTillNextEvent;

    public bool whaleNotCalling;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        PC_AD.volume = 0;
        RandomiseValues();
        timeTillNextEvent = timeOff;
    }

    // Update is called once per frame
    void Update()
    {
        // Count up the timer we need this value to meet another
        timer += Time.deltaTime;
        // When the timer is greater than the randomized value
        if(timer > timeTillNextEvent)
        {
            // Is our whale actually calling rn?
            if(whaleNotCalling)    // No. Lets tell the game that
            {
                // turn down the audio from the while loop so we cannot hear the whale anymore
                StartCoroutine(FadeWhale(PC_AD.volume, 0, fadTime));
                whaleNotCalling = false;   // We are rather fading back to no noise or there was never noise
                RandomiseValues();  // Make a new value so the timer can meet the new timeTillNextEvent Time
                timeTillNextEvent = timeOff + fadTime;  // Set the new randomized Value
            }
            else if (!whaleNotCalling) // if the whale is ready to call
            {
                // Fade in the whale noise slowly
                StartCoroutine(FadeWhale(0, whaleVol, fadTime));
                whaleNotCalling = true; // the whale is making noise so let the boolean take control
                RandomiseValues();  // Make new values so we can fade out when the new value is met
                timeTillNextEvent = timeOn + fadTime;
            }
            // reset the timer (If we dont do this then the script will break cuz the value keeps increasing)
            timer = 0;
        }
    }
    // Random values for audio sources
    void RandomiseValues()
    {
        // Set random values so we can call them whenever we fade the audio in and out
        timeOff = Random.Range(minTimeOff, maxTimeOff);
        timeOn = Random.Range(minTimeOn, maxTimeOn);
        whaleVol = Random.Range(minVol, maxVol);
    }

    IEnumerator FadeWhale(float startValue, float endValue, float duration)
    {
        float currentTIme = 0;
        // Slowly increase the volume from the audio source variable
        while(currentTIme <= duration)
        {
            PC_AD.volume = Mathf.Lerp(startValue, endValue, (currentTIme / duration));
            currentTIme += Time.deltaTime;
            yield return null;
        }
    }
}
