// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Doozy.Installer
{
    public class DoozyInstallerWindow : EditorWindow
    {
        #region Instance

        private static DoozyInstallerWindow s_instance;

        // ReSharper disable once MemberCanBePrivate.Global
        public static DoozyInstallerWindow Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = Window;
                if (s_instance != null) return s_instance;
                s_instance = GetWindow<DoozyInstallerWindow>(true, Labels.InstallDoozyUI);
                return s_instance;
            }
        }

        /*
       * An alternative way to get Window, because
       * GetWindow<DoozyInstallerWindow>() forces window to be active and present
       */
        private static DoozyInstallerWindow Window
        {
            get
            {
                DoozyInstallerWindow[] windows = Resources.FindObjectsOfTypeAll<DoozyInstallerWindow>();
                return windows.Length > 0 ? windows[0] : null;
            }
        }

        #endregion

        private static InstallerLanguagePack Labels { get { return InstallerLanguagePack.Instance; } }
        private static InstallerSettings Settings { get { return InstallerSettings.Instance; } }

        private static GUIStyle
//            s_commentGreen,
//            s_commentOrange,
            s_commentRed,
            s_commentWhite,
            s_downloadButton,
            s_iconError,
            s_iconOk,
//            s_iconWarning,
            s_installButton,
            s_itemBackground,
            s_labelGreen,
//            s_labelOrange,
            s_labelRed,
//            s_labelWhite,
            s_logoDOTween,
            s_logoDoozyUIVersion2,
            s_logoDoozyUIVersion3,
            s_uninstallButton,
//            s_upgradeButton,
            s_windowBackground;


        [MenuItem("Tools/Install DoozyUI", false, 0)]
        public static void Open()
        {
            GetWindow<DoozyInstallerWindow>(true, Labels.InstallDoozyUI);
            Instance.InitWindow();
        }

        private void OnEnable()
        {
            InitWindow();
            InstallerProcessor.Run();
            EditorUtility.ClearProgressBar();
        }

        private void InitWindow()
        {
            titleContent = new GUIContent(Labels.InstallDoozyUI);
            wantsMouseMove = true;
            minSize = new Vector2(508, 572);
            maxSize = minSize;

            LoadStyles();
        }

        private static void LoadStyles()
        {
            if (Settings.Skin == null)
                Settings.Skin = InstallerSettings.GetScriptableObject<GUISkin>("InstallerSkin", InstallerSettings.ResourcesPath);

//            s_commentGreen = Settings.Skin.GetStyle("CommentGreen");
//            s_commentOrange = Settings.Skin.GetStyle("CommentOrange");
            s_commentRed = Settings.Skin.GetStyle("CommentRed");
            s_commentWhite = Settings.Skin.GetStyle("CommentWhite");
            s_downloadButton = Settings.Skin.GetStyle("DownloadButton");
            s_iconError = Settings.Skin.GetStyle("IconError");
            s_iconOk = Settings.Skin.GetStyle("IconOk");
//            s_iconWarning = Settings.Skin.GetStyle("IconWarning");
            s_installButton = Settings.Skin.GetStyle("InstallButton");
            s_itemBackground = Settings.Skin.GetStyle("ItemBackground");
            s_labelGreen = Settings.Skin.GetStyle("LabelGreen");
//            s_labelOrange = Settings.Skin.GetStyle("LabelOrange");
            s_labelRed = Settings.Skin.GetStyle("LabelRed");
//            s_labelWhite = Settings.Skin.GetStyle("LabelWhite");
            s_logoDOTween = Settings.Skin.GetStyle("LogoDOTween");
            s_logoDoozyUIVersion2 = Settings.Skin.GetStyle("LogoDoozyUIVersion2");
            s_logoDoozyUIVersion3 = Settings.Skin.GetStyle("LogoDoozyUIVersion3");
            s_uninstallButton = Settings.Skin.GetStyle("UninstallButton");
//            s_upgradeButton = Settings.Skin.GetStyle("UpgradeButton");
            s_windowBackground = Settings.Skin.GetStyle("WindowBackground");
        }

        private void OnInspectorUpdate() { Repaint(); }

        private void OnGUI()
        {
            GUI.Label(new Rect(-2, 0, s_windowBackground.fixedWidth, s_windowBackground.fixedHeight), s_windowBackground.normal.background);
            GUILayout.BeginArea(new Rect(0, 272, position.width, position.height - 272));
            {
                DrawContent();
            }
            GUILayout.EndArea();
        }

        private void DrawContent()
        {
            bool runInstaller = false;
            GUILayout.BeginVertical();
            {
                DrawDOTween();
                DrawDoozyUIVersion2();
                DrawDoozyUIVersion3();

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    {
                        if (!Settings.DOTweenDetected)
                        {
                            GUILayout.Label(Labels.ToContinueDownloadAndInstallDOTween, s_commentWhite, GUILayout.ExpandWidth(true));
                            GUILayout.Space(16);
                        }
                        else if (Settings.DoozyUIVersion2Detected)
                        {
                            GUILayout.Label(Labels.Version3IsNotCompatibleWithVersion2, s_commentRed, GUILayout.ExpandWidth(true));
                            GUILayout.Space(EditorGUIUtility.singleLineHeight);
                            GUILayout.Label(Labels.ToContinueRemoveDoozyUI2, s_commentWhite, GUILayout.ExpandWidth(true));
                            GUILayout.Space(16);
                        }
                        else
                        {
                            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
                            if (GUILayout.Button(Labels.Install, s_installButton))
                                runInstaller = true;

                            GUI.enabled = true;
                            
                            GUILayout.Space(8);
                            Color color = GUI.color;
                            GUI.color = new Color(color.r, color.g, color.b, 0.4f);
                            GUILayout.Label(Settings.Version, s_commentWhite, GUILayout.ExpandWidth(true));
                            GUI.color = color;
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(16);
            }
            GUILayout.EndVertical();
            
            if (!runInstaller || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            AssetDatabase.ImportPackage(InstallerSettings.Instance.DataPath +
                                        InstallerSettings.Instance.PackageName +
                                        InstallerSettings.Instance.PackageExtension,
                                        true);
        }

        private void DrawDOTween()
        {
            GUIStyle iconStyle = Settings.DOTweenDetected ? s_iconOk : s_iconError;

            GUILayout.BeginHorizontal(s_itemBackground);
            {
                GUILayout.Space(24);
                GUILayout.BeginVertical(GUILayout.Height(s_itemBackground.fixedHeight));
                {
                    GUILayout.Space(s_itemBackground.fixedHeight / 2 - iconStyle.fixedHeight / 2);
                    GUILayout.Label(iconStyle.normal.background);
                }
                GUILayout.EndVertical();
                GUILayout.Space(48);
                GUILayout.BeginVertical(GUILayout.Height(s_itemBackground.fixedHeight));
                {
                    GUILayout.Space(s_itemBackground.fixedHeight / 2 - s_logoDOTween.fixedHeight / 2);
                    GUILayout.Label(s_logoDOTween.normal.background);
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                if (Settings.DOTweenDetected)
                {
                    GUILayout.Label(Labels.Installed, s_labelGreen, GUILayout.Height(s_itemBackground.fixedHeight));
                }
                else
                {
                    GUILayout.BeginVertical(GUILayout.Height(s_itemBackground.fixedHeight));
                    {
                        GUILayout.Space(s_itemBackground.fixedHeight / 2 - s_downloadButton.fixedHeight / 2);
                        if (GUILayout.Button(Labels.Download, s_downloadButton)) AssetStore.Open("content/27676"); //DOTween Asset Store Id
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.Space(32);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDoozyUIVersion2()
        {
            GUIStyle iconStyle = !Settings.DoozyUIVersion2Detected ? s_iconOk : s_iconError;

            GUILayout.BeginHorizontal(s_itemBackground);
            {
                GUILayout.Space(24);
                GUILayout.BeginVertical(GUILayout.Height(s_itemBackground.fixedHeight));
                {
                    GUILayout.Space(s_itemBackground.fixedHeight / 2 - iconStyle.fixedHeight / 2);
                    GUILayout.Label(iconStyle.normal.background);
                }
                GUILayout.EndVertical();
                GUILayout.Space(48);
                GUILayout.BeginVertical(GUILayout.Height(s_itemBackground.fixedHeight));
                {
                    GUILayout.Space(s_itemBackground.fixedHeight / 2 - s_logoDOTween.fixedHeight / 2);
                    GUILayout.Label(s_logoDoozyUIVersion2.normal.background);
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                if (!Settings.DoozyUIVersion2Detected)
                {
                    GUILayout.Label(Labels.NotInstalled, s_labelGreen, GUILayout.Height(s_itemBackground.fixedHeight));
                }
                else
                {
                    GUILayout.BeginVertical(GUILayout.Height(s_itemBackground.fixedHeight));
                    {
                        GUILayout.Space(s_itemBackground.fixedHeight / 2 - s_downloadButton.fixedHeight / 2);
                        if (GUILayout.Button(Labels.Uninstall, s_uninstallButton))
                        {
                            if (EditorUtility.DisplayDialog(Labels.Uninstall, Labels.AreYouSureYouWantToUninstallDoozyUIVersion2, Labels.Yes, Labels.No))
                            {
                                EditorUtility.DisplayProgressBar(Labels.Uninstall, Labels.DoozyUIVersion2, 0.1f);

                                string[] guids = AssetDatabase.FindAssets("t:DUIVersion", null);
                                if (guids != null && guids.Length > 0)
                                {
                                    string doozyPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                                    doozyPath = doozyPath.Replace(@"/Editor/Resources/DUI/Data/DUIVersion.asset", "");

                                    EditorUtility.DisplayProgressBar(Labels.Uninstall, Labels.DoozyUIVersion2, 0.4f);
                                    if (AssetDatabase.IsValidFolder(doozyPath))
                                    {
                                        AssetDatabase.MoveAssetToTrash(doozyPath);
                                        AssetDatabase.ForceReserializeAssets(new[] {doozyPath});
                                    }
                                }

                                EditorUtility.DisplayProgressBar(Labels.Uninstall, Labels.DoozyUIVersion2, 0.7f);
                                if (AssetDatabase.IsValidFolder("Assets/Plugins/Quick"))
                                {
                                    AssetDatabase.MoveAssetToTrash("Assets/Plugins/Quick");
                                    AssetDatabase.ForceReserializeAssets(new[] {"Assets/Plugins/Quick"});
                                }

                                EditorUtility.DisplayProgressBar(Labels.Uninstall, Labels.DoozyUIVersion2, 0.9f);
                                DefineSymbolsUtils.RemoveGlobalDefine("dUI_DoozyUI");
                                EditorUtility.ClearProgressBar();
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.Space(32);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDoozyUIVersion3()
        {
            GUIStyle iconStyle = Settings.DOTweenDetected && !Settings.DoozyUIVersion2Detected ? s_iconOk : s_iconError;

            GUILayout.BeginHorizontal(s_itemBackground);
            {
                GUILayout.Space(24);
                GUILayout.BeginVertical(GUILayout.Height(s_itemBackground.fixedHeight));
                {
                    GUILayout.Space(s_itemBackground.fixedHeight / 2 - iconStyle.fixedHeight / 2);
                    GUILayout.Label(iconStyle.normal.background);
                }
                GUILayout.EndVertical();
                GUILayout.Space(48);
                GUILayout.BeginVertical(GUILayout.Height(s_itemBackground.fixedHeight));
                {
                    GUILayout.Space(s_itemBackground.fixedHeight / 2 - s_logoDOTween.fixedHeight / 2);
                    GUILayout.Label(s_logoDoozyUIVersion3.normal.background);
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                if (!Settings.DOTweenDetected || Settings.DoozyUIVersion2Detected)
                    GUILayout.Label(Labels.FixIssues, s_labelRed, GUILayout.Height(s_itemBackground.fixedHeight));
                else
                    GUILayout.Label(Labels.Ready, s_labelGreen, GUILayout.Height(s_itemBackground.fixedHeight));

                GUILayout.Space(32);
            }
            GUILayout.EndHorizontal();
        }
    }
}