using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Font_Manager : MonoBehaviour
{
    [Header("UI Components")]
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
    #region Font Chnage (Text Mesh Pro)
    public TMP_Text[] allTMPTextComp;
    public TMP_FontAsset[] TMP_Fonts;
    private TMP_FontAsset currentTMPFont;
    int TMP_FontTraccker = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Find all the text
        //allTextComp = FindObjectsOfType<Text>();
        allTMPTextComp = FindObjectsOfType<TMP_Text>();
        //currentFont = fonts[0];
        currentTMPFont = TMP_Fonts[0];
        // Carry it through scenes
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Find all the text
        //allTextComp = FindObjectsOfType<Text>();
        allTMPTextComp = FindObjectsOfType<TMP_Text>();
        TextMonitor();
        // Start at the beginning of the array
        DropDownFontChange(0);
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
