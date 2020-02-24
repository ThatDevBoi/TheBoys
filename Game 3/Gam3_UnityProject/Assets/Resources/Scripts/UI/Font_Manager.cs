using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Font_Manager : MonoBehaviour
{
    #region Variables
    [Header("UI Components")]
    // The Drop Down Menu that handles the fonts
    public Dropdown fontMenu;

    [Header("Normal Unity Text")]
    #region Font change (Normal Text)
    //public Text[] allTextComp;
    //public Font[] fonts;
    //public Font currentFont;
    //int FontTracker = 0;
    #endregion
    [Space(10)]
    [Header("Text Mesh Pro Text")]
    #region Font Change (Text Mesh Pro)
    public TMP_Text[] allTMPTextComp;   // All the Text Mesh Pro Components
    public TMP_FontAsset[] TMP_Fonts;   // All the TextMeshPro Font Types
    private TMP_FontAsset currentTMPFont;   // The Current Font We Have Selected
    int TMP_FontTraccker = 0;   // What font we chose from the drop down tracker
    private static Font_Manager managerIstance = null; // The First original Font Manager
    #endregion
    Canvas Options_Can; // The Canvas that holds the font Menu
    int changeScene = 0;    // Data int that tells the script when we need to find Variables again
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Find the Canvas
        Options_Can = GameObject.Find("Options_Canvas").GetComponent<Canvas>();
        Options_Can.enabled = false;    // Make sure the Canvas Component is off
        fontMenu = Options_Can.transform.GetChild(2).GetComponent<Dropdown>();  // Find the Drop Down Menu
        // Find all the text
        //allTextComp = FindObjectsOfType<Text>();
        // Find All Text Components
        allTMPTextComp = FindObjectsOfType<TMP_Text>();
        //currentFont = fonts[0];
        currentTMPFont = TMP_Fonts[0];  // Current Font is the first font in the array
        // Carry it through scenes
        if (managerIstance == null) // if we dont have the first instace of the Font Manager
        {
            managerIstance = this;  // Apply the Font Manager
            DontDestroyOnLoad(managerIstance.gameObject);   // Make sure it can pass through scenes
        }
        else if (managerIstance != this)    // if we have the font manager
            Destroy(gameObject);    // Destroy any other Font Manager but the very first one
    }

    // Update is called once per frame
    void Update()
    {
        // Find all the text
        //allTextComp = FindObjectsOfType<Text>();
        allTMPTextComp = FindObjectsOfType<TMP_Text>();
        // Chnge each TMP font 
        TextMonitor();
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
            changeScene = 1;    // increase a value so we can gather data if we go back to the main menu
        }
        else if (SceneManager.GetSceneByName("MainMenu") != null)   // if we are in the mian menu
        {
            if (changeScene == 1)   // if the data collection is ready (If we have gone from Main Menu to Main Game to Main Menu)
            {
                // Find components
                Options_Can = GameObject.Find("Options_Canvas").GetComponent<Canvas>();
                fontMenu = Options_Can.transform.GetChild(2).GetComponent<Dropdown>();
                changeScene = 0;    // reset
            }
        }
        #endregion
    }

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
        TMP_FontTraccker = dropdownValue;
        // change the font
        //currentFont = fonts[FontTracker];
        currentTMPFont = TMP_Fonts[TMP_FontTraccker];
    }
}
