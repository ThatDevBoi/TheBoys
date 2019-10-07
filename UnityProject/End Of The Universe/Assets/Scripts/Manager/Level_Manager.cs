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
    public Canvas Restart;
    public Image deeposcreen;
    //Strings
    public string gameOverMessage;
    // Ints
    public int currentDistance = 1;
    // Float
    public float playersDistance = 1;
    // Booleans
    public static bool gameOver = false;
    //Gameoobjicts
    public GameObject UIHandler;


    // Fad Effect Values
    public Texture2D fadeOutTexture;        // The Texture that will overlay the screen this can be a black image or a loading graphic
    public float fadeSpeed = 5f;        // The Fading Speed

    public int drawDepth = 1000;      // the texture order in the draw hierarchy a low number means in renders on top
    public float alpha = 1.0f;     // the texture alpha value between 0 and 1
    public int fadeDir = -1;       // the direction to fade in = -1 or out = 1


    public void Awake()
    {
        playersDistance = 1;
        // Assign the canvas variable
        StartMenu = GameObject.Find("StartMenu").GetComponent<Canvas>();
        //restartCanvas = GameObject.Find("RestartMenu").GetComponent<Canvas>();
        
        deeposcreen.color = new Color(deeposcreen.color.r, deeposcreen.color.g, deeposcreen.color.b, 0);
        deeposcreen.enabled = false;
        UIHandler.SetActive(false);

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
            GameOverMessage();
            
        }
        
        // When the game is currently not in a play state dont call the function 
        if(Time.timeScale > 0)
        {
            StartCoroutine(DistanceCalulator());
        }
    }

    public void StartGame()
    {

        
        // Change the time so the game will play
        Time.timeScale = 1;
        // Turn off the canvas
        StartMenu.enabled = false;
    }

    public void RestartGame(string levelName)
    {
       
        //Reloads the scene
        Application.LoadLevel(levelName);
        //Turn off the canvas
        Restart.enabled = false;
        
        gameOver = false;
    }

    public void EndApplication()
    {
        EndApplication();
    }

    public void GameOverMessage()
    {
        Text gmOverText = GameObject.Find("Game_Over").GetComponent<Text>();
        gmOverText.text = gameOverMessage.ToString();
        if (gameOver)
        {
            //gameOverMessage.Length = Random.Range()
        }
    }

    public void DoFade()
    {
        alpha = deeposcreen.color.a;
        deeposcreen.enabled = true;
        alpha += 5 * Time.deltaTime;
        deeposcreen.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b, alpha);
        GameObject GravitySlider = GameObject.Find("Gravity_Slider");
        GravitySlider.SetActive(false);
        //deeposcreen.CrossFadeAlpha(alpha += increaseAlpha, fadeDir, true);

    }



    //private void OnGUI()
    //{
    //    if(gameOver)
    //    {
    //        //alpha += fadeDir * fadeSpeed * Time.deltaTime;      // Fade in/out the alpha value using  direction, a speed and time.delta to convert the operation to seconds

    //        //alpha = Mathf.Clamp01(alpha);       // Force (clamp) the number between 0 and 1 because GUI color uses alpha values getween 0 and 1 
    //        //                                    // set color of our GUI (in this case our Texture).All color values remain the same & the Apla is set to the alpha
    //        //GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);        // Set the alpha values
    //        ////GUI.depth = drawDepth;      // Make the black Texture render on top (drawn last)
    //        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    //        //GUI.depth = -1;

    //    }
    //}


    public IEnumerator DistanceCalulator()
    {
        if (gameOver)
        {
            playersDistance = 0;

        }
        else
        {
            //playersDistance += Time.deltaTime * currentDistance;
            // Make the float value round up to an int value so it shows in UI as a whole number
            playersDistance = Mathf.RoundToInt(Time.time * currentDistance + 1);
            // Let the UI text update when the value increases
            distanceMeter.text = "Miles: " + playersDistance;
            // Console the value
            Debug.Log(playersDistance);
            yield break;
        }
    }



    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(.1f);
        if (deeposcreen.color.a < 255)
        {
            DoFade();
        }
        else
            yield break;

        yield return new WaitForSeconds(.2f);
        Restart.enabled = true;
        UIHandler.SetActive(true);
        Time.timeScale = 0;
        yield break;
    }

}