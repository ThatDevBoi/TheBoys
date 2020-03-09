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
        // Slider in the start menu that changes audio
        Slider volumeChange;    // slider that edits sound
        [LabelArray(new string[] { "Player And NPC Mixer", "Environment Mixer", "Third" })]
        public AudioMixer[] Mixers;     // All the mixers we will manipulate
        private AudioMixer currentMixer;    // the current Mixer we have selected from the drop down menu
        private TMP_Dropdown typeOfMixer;   // The dropdown menu for the mixers 
        int mixertype;  // what drop down value we have selected
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
        #region Find Components
        // Find the Canvas
        Options_Can = GameObject.Find("Options_Canvas").GetComponent<Canvas>();
        Options_Can.enabled = false;    // Make sure the Canvas Component is off
        fontMenu = Options_Can.transform.GetChild(2).GetComponent<TMP_Dropdown>();  // Find the Drop Down Menu
        typeOfMixer = Options_Can.transform.GetChild(4).GetComponent<TMP_Dropdown>(); // Changes the mixer we edit
        volumeChange = Options_Can.transform.GetChild(3).GetComponent<Slider>();    // Find the slider in the canvas 
        SensertivitySlider = Options_Can.transform.GetChild(5).GetComponent<Slider>();  // Slider for the change in player rotation
        #endregion
        currentTMPFont = TMP_Fonts[0];  // Current Font is the first font in the array
        // Find all the text
        //allTextComp = FindObjectsOfType<Text>();
        // Find All Text Components
        allTMPTextComp = FindObjectsOfType<TMP_Text>();
        //currentFont = fonts[0];
        #region Find The Mixers
        // Find the Mixer in the resources
        Mixers[0] = Resources.Load<AudioMixer>("Audio_AS/Master Mixers_AS_mix/PC_n_NPC/Player_NPC_Master");
        Mixers[0].SetFloat("Volume", volumeChange.value);   // Change start value of this mixer in the array
        Mixers[1] = Resources.Load<AudioMixer>("Audio_AS/Master Mixers_AS_mix/Environment/Environment_Master");
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
    }

    // Update is called once per frame
    void Update()
    {
        // Functions 
        TextMonitor();          // Change each TMP font 
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
            MixerMenu(typeOfMixer, mixertype);
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
            // Edit The Audio when in the Main Menu
            AudioChangeListeners();
            if (changeScene == 1)   // if the data collection is ready (If we have gone from Main Menu to Main Game to Main Menu)
            {
                // Find components
                Options_Can = GameObject.Find("Options_Canvas").GetComponent<Canvas>(); // Options Canvas
                volumeChange = Options_Can.transform.GetChild(3).GetComponent<Slider>();    // Audio Controller
                fontMenu = Options_Can.transform.GetChild(2).GetComponent<TMP_Dropdown>();  // (Drop Down Menu - Manages Fonts)
                typeOfMixer = Options_Can.transform.GetChild(4).GetComponent<TMP_Dropdown>(); // Changes the mixer we edit
                // 
                changeScene = 0;    // reset
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
    public void BalanceAudio(float volume)
    {

        if(mixertype == 0)
        {
            Mixers[0].SetFloat("Volume", volume);
        }
    }

    public void MixerMenu(TMP_Dropdown menu, int dropdownValue)
    {
        // value of the dropdown menu
        dropdownValue = menu.value;
        mixertype = dropdownValue;
        currentMixer = Mixers[mixertype];   // Mixer changes with the value change

    }

    public void AudioChangeListeners()
    {
        // Alter Audio for Player or NPC 
        volumeChange.onValueChanged.AddListener(BalanceAudio);
    }

    void ChangePlayerSettings()
    {
        // player rotation changes with slider value
        playerScript.cameraRotationRate = playersSensertivity;
    }
    #endregion
}
