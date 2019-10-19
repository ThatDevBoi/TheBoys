using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // This is for the whale noise
    public AudioSource PC_AD;

    public float fadTime = 5f;

    public float minTimeOff = 5f;
    public float maxTimeOff = 15f;
    float timeOff;

    public float minTimeOn = 5f;
    public float maxTimeOn = 15f;
    float timeOn;

    public float minVol = 0.5f;
    public float maxVol = 1;
    float whaleVol;

    public float timer;
    public float timeTillNextEvent;

    bool whaleCalling;
    // Start is called before the first frame update
    void Start()
    {
        RandomiseValues();
        timeTillNextEvent = timeOff;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > timeTillNextEvent)
        {
            if(whaleCalling)
            {
                StartCoroutine(FadeWhale(PC_AD.volume, 0, fadTime));
                whaleCalling = false;
                RandomiseValues();
                timeTillNextEvent = timeOff + fadTime;
            }
            else if (!whaleCalling)
            {
                StartCoroutine(FadeWhale(0, whaleVol, fadTime));
                whaleCalling = true;
                RandomiseValues();
                timeTillNextEvent = timeOn + fadTime;
            }
            timer = 0;
        }
    }
    // Random values for audio sources
    void RandomiseValues()
    {
        timeOff = Random.Range(minTimeOff, maxTimeOff);
        timeOn = Random.Range(minTimeOn, maxTimeOn);
        whaleVol = Random.Range(minVol, maxVol);
    }

    IEnumerator FadeWhale(float startValue, float endValue, float duration)
    {
        float currentTIme = 0;

        while(currentTIme <= duration)
        {
            PC_AD.volume = Mathf.Lerp(startValue, endValue, (currentTIme / duration));
            currentTIme += Time.deltaTime;
            yield return null;
        }
    }
}
