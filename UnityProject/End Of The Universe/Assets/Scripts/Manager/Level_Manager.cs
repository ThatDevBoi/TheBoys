using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Level_Manager : MonoBehaviour
{
    // Variables
    // UI
    public Text distanceMeter;
    public Canvas StartMenu;
    // public Canvas restartCanvas;
    // Ints
    int currentDistance = 1;
    // Float
    float playersDistance = 1;
    // Booleans
    public static bool gameOver = false;


    // Fad Effect Values
    public Texture2D fadeOutTexture;        // The Texture that will overlay the screen this can be a black image or a loading graphic
    public float fadeSpeed = 5f;        // The Fading Speed

    public int drawDepth = -1000;      // the texture order in the draw hierarchy a low number means in renders on top
    public float alpha = 1.0f;     // the texture alpha value between 0 and 1
    public int fadeDir = -1;       // the direction to fade in = -1 or out = 1


    public void Awake()
    {
        // Assign the canvas variable
        StartMenu = GameObject.Find("StartMenu").GetComponent<Canvas>();
        //restartCanvas = GameObject.Find("RestartMenu").GetComponent<Canvas>();
      
        Time.timeScale = 0;
        StartMenu.enabled = true;

        // Debug
        if (StartMenu == null)
            Debug.LogWarning("You Have Not Applied The Relevant Canvas");
        else
            return;
    }

    public void Update()
    {
        Debug.Log(gameOver);
        if (gameOver)
        {
            StartCoroutine(GameOver());
            Time.timeScale = 0;
        }
        
        // When the game is currently not in a play state dont call the function 
        if(Time.timeScale > 0)
        {
            StartCoroutine(DistanceCalulator());
        }
    }

    public void StartGame()
    {
        // Chnage the time so the game will play
        Time.timeScale = 1;
        // Turn off the canvas
        StartMenu.enabled = false;
    }

    private void OnGUI()
    {
        if(gameOver)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;      // Fade in/out the alpha value using  direction, a speed and time.delta to convert the operation to seconds

            alpha = Mathf.Clamp01(alpha);       // Force (clamp) the number between 0 and 1 because GUI color uses alpha vlues getween 0 and 1 
                                                // set color of our GUI (in this case our Texture).All color values remain the same & the Apla is set to the alpha
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);        // Set the alpha values
            GUI.depth = drawDepth;      // Make the black Texture render on top (drawn last)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
        }
    }


    public IEnumerator DistanceCalulator()
    {
        playersDistance += Time.deltaTime * currentDistance;
        // Make the float value round up to an int value so it shows in UI as a whole number
        playersDistance = Mathf.RoundToInt(Time.time * currentDistance + 1);
        // Let the UI text update when the value increases
        distanceMeter.text = "Miles: " + playersDistance;
        // Console the value
        Debug.Log(playersDistance);
        yield break;
    }

    public float StartFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(.5f);
        StartFade(1);
        yield break;
    }

}