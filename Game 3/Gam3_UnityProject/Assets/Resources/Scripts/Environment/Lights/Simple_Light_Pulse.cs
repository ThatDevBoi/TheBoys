using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//#if (UNITY_EDITOR)
//[CustomPropertyDrawer(typeof(HideAttributes))]
//#endif
public class Simple_Light_Pulse : MonoBehaviour
{
    //[Header("Variable Revealers")]
    public bool revealVariables = false;
    //[HideAttributes("revealVariables", true)]
    public bool directionLight;
    //[HideAttributes("revealVariables", true)]
    public bool spotLight;
    //[HideAttributes("revealVariables", true)]
    public bool areaLight;

    [Header("Spot Light Variables")]
    //[HideAttributes("spotLight", false)]
    public float spotLightAngle = 60f;

    [Header("Directional Light Variables")]
    //[HideAttributes("directionLight", false)]
    public float CookieSize = 10;


    [Header("Area Light Varibles")]
    //[HideAttributes("areaLight", false)]
    public Shape AreaShape = Shape.RECTANGLE;
    public enum Shape { RECTANGLE, DISC }
    //[HideAttributes("areaLight", false)]
    public float width = 1;
    //[HideAttributes("areaLight", false)]
    public float Height = 1;
    //[HideAttributes("areaLight", false)]
    public float radius = 6;


    /// <summary>
    /// Default is Point Light Mode
    /// Class allows for pulsation of a light and controls the Light Component before runtime
    /// </summary>
    #region Variables
    [Header("Light Settings")]
    // namespace for the enum
    public LightType lightTypes = LightType.POINT;
    // enum that changes the light Type
    public enum LightType { SPOT, DIRECTIONAL, POINT, AREA };
    // The Range of the light
    public float StartlightRange = 3;
    // Light Component
    private Light objectLight;
    // Color of the light
    public Color lightColor;
    // Enum for Light Mode
    public enum LightingMode {REALTIME, MIXED,  BAKED}
    // namepace for the enum Light Mode
    public LightingMode LightMode = LightingMode.REALTIME;
    // The intensity of the light
    public float StartLightIntensity = 1;
    // The InDirect Multiplier value of the light
    public float LightMultiplier = 2;
    // Enum for changing shadows
    public enum ShadowType { NO_SHADOWN, HARD_SHADOWN, SOFT_SHADOWN}
    // Namespace for shadow change
    public ShadowType TypeOfShadow = ShadowType.NO_SHADOWN;
    // Enum for renderer change
    public enum RenderMode { AUTO, IMPORTANT, NOT_IMPORTANT}
    public RenderMode TypeOfRender = RenderMode.AUTO;
    // Object layers we project light onto
    public LayerMask LightingReflections;


    [Header("Simple_Light_Pulse Settings")]
    // Duration of the pulse
    public float LightPulse = 5f;
    // timer that decides if we pulse
    float pulseTimer;
    // next time we are allowed to pulse
    public float nextTimeToPulse = 5;

    // Holds the value of intensity how intense the pulse is
    private float LightPulseIntensity;
    // Minimal amount of lighting we intesense 
    public float minIntensity = 3;
    // Max value we intense How hard the light projects
    public float maxIntensity = 6;
    // Holds the range of pulse
    private float LightPulseRange;
    // Min Light Pulse Range
    public float minLightRange = 4;
    // Max light Pulse range
    public float maxLightRange = 8;
    // Allows us to pulse
    bool readyToPulse;

    // Value which holds how long we pulse out
    float timePulseOut;
    private float minTimePulsein = 2;
    private float maxTimePulsein = 6;

    // Value which holds how long we pulse in
    float timePulseIn;
    private float minTimePulseout = 2;
    private float maxTimePulseout = 6;


    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Make the light
        objectLight = gameObject.AddComponent<Light>();
        // What type of light are we using
        #region Type Of Light
        if (lightTypes == LightType.SPOT)
        {
            objectLight.type = UnityEngine.LightType.Spot;
            objectLight.spotAngle = spotLightAngle;
        }


        if (lightTypes == LightType.DIRECTIONAL)
        {
            objectLight.type = UnityEngine.LightType.Directional;
            objectLight.cookieSize = CookieSize;
        }


        if (lightTypes == LightType.POINT)
            objectLight.type = UnityEngine.LightType.Point;

        //if (lightTypes == LightType.AREA)
        //{
        //    objectLight.type = UnityEngine.LightType.Area;
        //    if (AreaShape == Shape.RECTANGLE)
        //    {
        //        objectLight.type = UnityEngine.LightType.Rectangle;
        //        // For Area Light
        //        objectLight.areaSize = new Vector2(width, Height);
        //    }
        //    else if (AreaShape == Shape.DISC)
        //    {
        //        objectLight.type = UnityEngine.LightType.Disc;
        //        objectLight.areaSize = new Vector2(radius, 0);
        //    }

        //}


            #endregion
        // What mode are we rendering
        #region Mode Lighting
        //if (LightMode == LightingMode.REALTIME)
        //    objectLight.lightmapBakeType = LightmapBakeType.Realtime;
        //if (LightMode == LightingMode.MIXED)
        //    objectLight.lightmapBakeType = LightmapBakeType.Mixed;
        //if (LightMode == LightingMode.BAKED)
        //    objectLight.lightmapBakeType = LightmapBakeType.Baked;
        #endregion
        // Different light value settings
        #region Light Unit Set Up
        // The Light Color
        objectLight.color = lightColor;
        // Start Value For The Lights Range
        objectLight.range = StartlightRange;
        // Start value for the Lights intensity
        objectLight.intensity = StartLightIntensity;
        // The light multiplier
        objectLight.bounceIntensity = LightMultiplier;
        #endregion
        // Shadow Settings
        #region Shadows
        if (TypeOfShadow == ShadowType.NO_SHADOWN)
            objectLight.shadows = UnityEngine.LightShadows.None;

        if (TypeOfShadow == ShadowType.HARD_SHADOWN)
            objectLight.shadows = UnityEngine.LightShadows.Hard;

        if (TypeOfShadow == ShadowType.SOFT_SHADOWN)
            objectLight.shadows = UnityEngine.LightShadows.Soft;
        #endregion

        #region Culling Mask
        objectLight.cullingMask = LightingReflections;
        #endregion

        Randomizer();
    }

    // Update is called once per frame
    void Update()
    {
        pulseTimer += Time.deltaTime;
        if(pulseTimer > nextTimeToPulse)
        {
            if(readyToPulse)
            {
                StartCoroutine(LightingPulseRange(objectLight.range, StartlightRange, LightPulse));
                StartCoroutine(LightingPulseIntensity(objectLight.intensity, StartLightIntensity, LightPulse));
                readyToPulse = false;
                Randomizer();
                nextTimeToPulse = timePulseIn + LightPulse;
            }
            else if(!readyToPulse)
            {
                StartCoroutine(LightingPulseRange(0, LightPulseRange, LightPulse));
                StartCoroutine(LightingPulseIntensity(0, LightPulseIntensity, LightPulse));
                readyToPulse = true;
                Randomizer();
                nextTimeToPulse = timePulseOut + LightPulse;
            }
            pulseTimer = 0;
        }
    }

    void Randomizer()
    {
        // Decide a random light intensity
        LightPulseIntensity = Random.Range(minIntensity, maxIntensity);
        //
        LightPulseRange = Random.Range(minLightRange, maxLightRange);
        timePulseOut = Random.Range(minTimePulsein, maxTimePulsein);
        timePulseIn = Random.Range(minTimePulseout, maxTimePulseout);
    }

    IEnumerator LightingPulseRange(float startValue, float endValue, float duration)
    {
        float currentTime = 0;
       // objectLight.range = 0;
        while(currentTime <= duration)
        {
            objectLight.range = Mathf.Lerp(startValue, endValue, (currentTime / duration));
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator LightingPulseIntensity(float startValue, float endValue, float duration)
    {
        float currentTime = 0;
        //objectLight.intensity = 0;
        while (currentTime <= duration)
        {
            objectLight.intensity = Mathf.Lerp(startValue, endValue, (currentTime / duration));
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
