using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    #region Variables

    #region UI Components 
    [Header("UI Components")]
    // The Drop Down Menu that handles the fonts
    public TMP_Dropdown fontMenu;
    Canvas Options_Can; // The Canvas that holds the font Menu
    Slider SensertivitySlider;  // Slider that controls the player mouse rotation
    #endregion

    #region Font change (Normal Text)
    [Header("Normal Unity Text")]
    //public Text[] allTextComp;
    //public Font[] fonts;
    //public Font currentFont;
    //int FontTracker = 0;
    #endregion

    #region Font Change (Text Mesh Pro)
    [Header("Text Mesh Pro Text")]
    public TMP_Text[] allTMPTextComp;   // All the Text Mesh Pro Components
    // This needs to be set manually
    // Write the strings for each Array element. It allows us to keep arrays in the correct order
    [LabelArray(new string[] { "NextFont", "JetBrainsMono", "Digitalism", "RogueHero", "Liberation" })]
    public TMP_FontAsset[] TMP_Fonts;   // All the TextMeshPro Font Types
    // The Current Font We are using
    private TMP_FontAsset currentTMPFont;   // The Current Font We Have Selected
    int TMP_FontTracker = 0;   // What font we chose from the drop down tracker
    private static UI_Manager managerIstance = null; // The First original Font Manager
    #endregion

    #region Game State Monitor Variables 
    int changeScene = 0;    // Data int that tells the script when we need to find Variables again
    #endregion

    #region Audio Managment
    [Header("Audio Manager")]
    // NEW
    [LabelArray(new string[] { "Player", "AI", "Background"})]
    public Slider[] volumeChangers;
    private AudioMixer mastermix;
    #endregion

    #region Sub Menu Options
    [SerializeField]
    [LabelArray(new string[] { "Options Button", "Audio Button", "Gameplay Button", "Visuals Button" })]
    private Button[] SubMenuButtons = new Button[4];    // Set length of array
    [LabelArray(new string[] { "Current", "Next Button"})]
    public Color[] ButtonClickChange;
    private int colorMonitor;
    #endregion

    #region Player Alteration Through UI
    Player_Controller playerScript;
    public static float playersSensertivity;
    #endregion

    #endregion
    #region Start And Update Function
    // Start is called before the first frame update
    void Start()
    {
        ButtonClickChange[0].a = 255;
        ButtonClickChange[1].a = 255;
        ButtonClickChange[0] = Color.cyan;
        ButtonClickChange[1] = Color.yellow;

        #region Find Components

        // Find the Canvas
        Options_Can = GameObject.Find("Options_Canvas").GetComponent<Canvas>();
        // Make sure the Canvas Component is off
        Options_Can.enabled = false;
        // Find the Drop Down Menu
        fontMenu = Options_Can.transform.GetChild(5).GetComponentInChildren<TMP_Dropdown>();
        // Find all the sliders in charge of manipulating the audio mixer groups
        volumeChangers = Options_Can.transform.GetChild(3).transform.GetChild(1).GetComponentsInChildren<Slider>(); // fill array
        // Find the Mouse Rotation Sensertivity Slider
        SensertivitySlider = Options_Can.transform.GetChild(4).gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Slider>();
        
        // Find All Buttons Fill the array
        SubMenuButtons[0] = Options_Can.transform.GetChild(2).transform.GetChild(0).GetComponent<Button>();
        SubMenuButtons[1] = Options_Can.transform.GetChild(3).transform.GetChild(0).GetComponent<Button>();
        SubMenuButtons[2] = Options_Can.transform.GetChild(4).transform.GetChild(0).GetComponent<Button>();
        SubMenuButtons[3] = Options_Can.transform.GetChild(5).transform.GetChild(0).GetComponent<Button>();

        #endregion
        #region Component / Value Set Up

        currentTMPFont = TMP_Fonts[0];  // Current Font is the first font in the array
        // Find all the text
        //allTextComp = FindObjectsOfType<Text>();
        // Find All Text Components
        allTMPTextComp = FindObjectsOfType<TMP_Text>();
        //currentFont = fonts[0];

        #endregion
        #region Find The Mixers

        // We need to find the master mixer in charge of the games sound output
        mastermix = Resources.Load<AudioMixer>("Audio_AS/Master Mixers_AS_mix/Master");

        #endregion
        #region Singleton

        // Carry it through scenes
        if (managerIstance == null) // if we dont have the first instace of the Font Manager
        {
            managerIstance = this;  // Apply the Font Manager
            DontDestroyOnLoad(managerIstance.gameObject);   // Make sure it can pass through scenes
        }
        else if (managerIstance != this)    // if we have the font manager
            Destroy(gameObject);    // Destroy any other Font Manager but the very first one

        #endregion
        // Debug Checks
        if(fontMenu == null | volumeChangers == null | SensertivitySlider == null)
        {
            // Print message in consol
            Debug.LogError("The Parent Child Relationship In The" + Options_Can.name + "Has Been Changed. Meaning " +
                "Variables Will Not Be Found Correclty");
            // End Play Session
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
        #endif
        }

        if(mastermix == null)
        {
            Debug.LogError("The Asset We Are Trying To Find May Have Been Moved In The Project Folder." +
                "So Our String Is Not Finding The Variable");
            // End Play Session
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
        #endif

        }

    }

    // Update is called once per frame
    void Update()
    {
        // Functions 
        TextMonitor();          // Change each TMP font 
        ChangeButtonColor();
        if(playerScript != null)
        {
            ChangePlayerSettings();
        }
        // Find all the text
        //allTextComp = FindObjectsOfType<Text>();
        allTMPTextComp = FindObjectsOfType<TMP_Text>();

        // If we have no drop down menu we cant call the function
        if (fontMenu != null)
        {
            // Start at the beginning of the array
            DropDownFontChange(0);
        }
        #region Scene Checks
        // If we are in the main level
        if (SceneManager.GetSceneByBuildIndex(1).buildIndex == 1)
        {
            playerScript = GameObject.Find("PC").GetComponent<Player_Controller>(); // find player script
            changeScene = 1;    // increase a value so we can gather data if we go back to the main menu
        }
        else if (SceneManager.GetSceneByName("MainMenu") != null)   // if we are in the mian menu
        {
            if (changeScene == 1)   // if the data collection is ready (If we have gone from Main Menu to Main Game to Main Menu)
            {
                // Find components
                Options_Can = GameObject.Find("Options_Canvas").GetComponent<Canvas>(); // Options Canvas
                // Fill Audio Slider Array
                volumeChangers = Options_Can.transform.GetChild(3).transform.GetChild(1).GetComponentsInChildren<Slider>();
                // Find The DropDownMenu for the font selector
                fontMenu = Options_Can.transform.GetChild(2).GetComponent<TMP_Dropdown>();  // (Drop Down Menu - Manages Fonts)
                // Reset value to acknoldge we are back at the main menu
                changeScene = 0;
            }
        }
        #endregion
        #region Player Changes
            playersSensertivity = SensertivitySlider.value;
            Debug.Log(playersSensertivity);
        #endregion
    }
#endregion
#region Font Logic
    public void fontChange(int fontType)
    {
        //currentFont = fonts[fontType];
    }
    /// <summary>
    /// This function allows you to change the Font by clicking a button
    /// </summary>
    public void TextMonitor()
    {
        //foreach (Text text in allTextComp)
        //    text.font = currentFont;

        foreach(TMP_Text text in allTMPTextComp)
        {
            text.font = currentTMPFont;
        }
    }
    /// <summary>
    /// This function allows you to change the font via the value of the current drop down selection
    /// </summary>
    public void DropDownFontChange(int dropdownValue)
    {
        // allow int to obtain frop down value 
        dropdownValue = fontMenu.value;
        // allow the font tracker to obtain the current drop down value (DropDownValue = current font)
        //FontTracker = dropdownValue;
        TMP_FontTracker = dropdownValue;
        // change the font
        //currentFont = fonts[FontTracker];
        currentTMPFont = TMP_Fonts[TMP_FontTracker];
    }
#endregion

#region Audio Functions
    public void PlayerAudio(float volume)
    {
        volume = volumeChangers[0].value;
        mastermix.SetFloat("PCVolume", volume);
    }

    public void AIAudio(float volume)
    {
        volume = volumeChangers[1].value;
        mastermix.SetFloat("AI_volume", volume);
    }

    public void BackgroundAudio(float volume)
    {
        volume = volumeChangers[2].value;
        mastermix.SetFloat("BackgroundVol", volume);
    }
    #endregion

    #region Sub Menu Changes
    public void ValueIncrease(int value)
    {
        colorMonitor = value;
    }

    void ChangeButtonColor()
    {
        if(colorMonitor == 0)
        {
            SubMenuButtons[0].image.color = ButtonClickChange[0];
            SubMenuButtons[1].image.color = ButtonClickChange[1];
            SubMenuButtons[2].image.color = ButtonClickChange[1];
            SubMenuButtons[3].image.color = ButtonClickChange[1];
        }
        else if(colorMonitor == 1)
        {
            SubMenuButtons[0].image.color = ButtonClickChange[1];
            SubMenuButtons[1].image.color = ButtonClickChange[0];
            SubMenuButtons[2].image.color = ButtonClickChange[1];
            SubMenuButtons[3].image.color = ButtonClickChange[1];
        }
        else if(colorMonitor == 2)
        {
            SubMenuButtons[0].image.color = ButtonClickChange[1];
            SubMenuButtons[1].image.color = ButtonClickChange[1];
            SubMenuButtons[2].image.color = ButtonClickChange[0];
            SubMenuButtons[3].image.color = ButtonClickChange[1];
        }
        else
        {
            SubMenuButtons[0].image.color = ButtonClickChange[1];
            SubMenuButtons[1].image.color = ButtonClickChange[1];
            SubMenuButtons[2].image.color = ButtonClickChange[1];
            SubMenuButtons[3].image.color = ButtonClickChange[0];
        }
    }
    #endregion

    void ChangePlayerSettings()
    {
        // player rotation changes with slider value
        playerScript.cameraRotationRate = playersSensertivity;
    }
}
