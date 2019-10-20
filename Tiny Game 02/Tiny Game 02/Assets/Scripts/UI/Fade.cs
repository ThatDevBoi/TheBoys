using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public SpriteRenderer sprite_renderer;
    public string FadeFunction;

    public bool turnAssetsOff = false;
    public bool readyToFade = false;
    public bool readyToDestroy = false;

    public float fadeTime = .08f;
    // Start is called before the first frame update
    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        Color c = sprite_renderer.material.color;
        c.a = 0f;
        sprite_renderer.material.color = c;

        gameObject.name = FadeFunction;
    }

    void Update()
    {
        GameObject startMenu;
        startMenu = GameObject.Find("StartMenu");
        GameObject cubeSpawn;
        cubeSpawn = GameObject.Find("Sube Spawner");
        // If we turn this Object off dont worry about it
        if (startMenu == null)
            return;
        if (cubeSpawn == null)
            return;
        // Turn off the bubble spawner and the startmenu
        if (turnAssetsOff)
        {
            startMenu.SetActive(false);
            cubeSpawn.SetActive(false);
        }
        else
        {
            startMenu.SetActive(true);
            cubeSpawn.SetActive(true);
        }

    }

    private void LateUpdate()
    {
        if (readyToFade)
            StartCoroutine(StartTheScene());

        if (readyToDestroy)
            Destroy(gameObject, 2f);
    }

    public void StartFading()
    {
        StartCoroutine(FadeChoice("FadeIn"));
    }

    public IEnumerator FadeChoice(string FadeFunction)
    {

        switch(FadeFunction)
        {
            // FadeIn
            case "FadeIn":
                for (float f = .05f; f <= 1; f += .05f)
                {
                    Color c = sprite_renderer.material.color;
                    c.a = f;
                    sprite_renderer.material.color = c;
                    turnAssetsOff = true;
                    readyToFade = true;
                    yield return new WaitForSeconds(fadeTime);
                }
                break;
                // FadeOut
            case "FadeOut":
                for (float f = 1f; f >= -0.05f; f -= 0.05f)
                {
                    Color c = sprite_renderer.material.color;
                    c.a = f;
                    sprite_renderer.material.color = c;
                    yield return new WaitForSeconds(fadeTime);
                }
                break;
        }
    }

    IEnumerator StartTheScene()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeChoice("FadeOut"));
        readyToDestroy = true;
        yield break;
    }

}
