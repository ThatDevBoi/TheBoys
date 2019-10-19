using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public SpriteRenderer renderer;
    public string FadeFunction;

    bool turnAssetsOff = false;
    bool FadeIntoGame = false;

    public float fadeTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        Color c = renderer.material.color;
        c.a = 0f;
        renderer.material.color = c;

        gameObject.name = FadeFunction;
    }

    private void Update()
    {
        // If we turn this Object off dont worry about it
        if (GameObject.Find("StartMenu") == null)
            return;
        if (GameObject.Find("Cube Spawner") == null)
            return;

        // Turn off the bubble spawner and the startmenu
        if (turnAssetsOff)
        {
            GameObject.Find("StartMenu").SetActive(false);
            GameObject.Find("Cube Spawner").SetActive(false);
        }

        // FadeOut
        if(turnAssetsOff)
        {
            StartCoroutine(StartTheScene());
        }


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
                    Color c = renderer.material.color;
                    c.a = f;
                    renderer.material.color = c;
                    turnAssetsOff = true;
                    yield return new WaitForSeconds(fadeTime);
                }
                break;
                // FadeOut
            case "FadeOut":
                for (float f = 1f; f >= -0.05f; f -= 0.05f)
                {
                    Color c = renderer.material.color;
                    c.a = f;
                    renderer.material.color = c;
                    yield return new WaitForSeconds(fadeTime);
                }
                break;
        }
    }

    IEnumerator StartTheScene()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeChoice("FadeOut"));
        yield break;
    }

}
