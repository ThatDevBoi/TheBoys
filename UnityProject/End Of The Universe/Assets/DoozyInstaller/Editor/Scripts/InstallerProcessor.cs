// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Doozy.Installer
{
    [InitializeOnLoad]
    public static class InstallerProcessor
    {
        /// <summary> Define Symbol for Doozy UI Manager </summary>
        public const string DEFINE_DOOZY_MANAGER = "dUI_MANAGER";

        /// <summary> Define Symbol for Doozy UI Designer </summary>
        public const string DEFINE_DOOZY_DESIGNER = "dUI_DESIGNER";

        /// <summary> Namespace for DOTween </summary>
        private const string NAMESPACE_DG_TWEENING = "DG.Tweening";

        /// <summary> Namespace for DoozyUI version 2 </summary>
        private const string NAMESPACE_DOOZYUI = "DoozyUI";

        /// <summary> Namespace for DoozyUI version 3 </summary>
        private const string NAMESPACE_DOOZY_ENGINE = "Doozy.Engine";

        private static InstallerSettings Settings { get { return InstallerSettings.Instance; } }
        private static bool s_saveAssets;
        private static List<Assembly> s_assemblies = new List<Assembly>();

        static InstallerProcessor() { ExecuteProcessor(); }

        private static void ExecuteProcessor()
        {
            DelayedCall.Run(1f, () =>
                                {
                                    Run();
#if !dUI_MASTER
                                      if (Settings != null && !Settings.InstallExecuted && DoozyInstallerWindow.Instance == null)
                                          EditorWindow.GetWindow<DoozyInstallerWindow>(true);
#endif
                                });

            AssetDatabase.importPackageCompleted += AssetDatabaseOnImportPackageCompleted;
        }

        public static void Run()
        {
            s_saveAssets = false;
            UpdateAssemblies();
            UpdateInstalledPluginsInDoozySettings();
            if (s_saveAssets) AssetDatabase.SaveAssets();
        }

        private static void AssetDatabaseOnImportPackageCompleted(string packageName)
        {
            if (Settings == null) return;

            if (!Settings.PackageName.Equals(packageName)) return;
            InstallerSettings.Instance.InstallExecuted = true;
            InstallerSettings.Instance.SetDirty(false);
            DoozyInstallerWindow.Instance.Close();
            UnityEngine.Debug.Log("DoozyUI " + InstallerSettings.Instance.Version + " Installed!");

#if dUI_MANAGER
//           Doozy.Editor.Internal.DoozyAssetsProcessor.Run();
           Doozy.Editor.Internal.ProcessorsSettings.Instance.RunDoozyAssetsProcessor = true;
#endif

#if !dUI_MASTER
            AssetDatabase.MoveAssetToTrash("Assets/DoozyInstaller");
#endif
        }

        private static IEnumerable<Assembly> GetAssemblies() { return AppDomain.CurrentDomain.GetAssemblies(); }

        //https://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx/
        private static bool NamespaceExists(string targetNamespace)
        {
            if (s_assemblies == null || s_assemblies.Count == 0)
            {
                UpdateAssemblies();
                if (s_assemblies == null || s_assemblies.Count == 0)
                    return false;
            }

            foreach (Assembly assembly in s_assemblies)
            {
                if (assembly == null) continue;
                Type[] typesInAsm;
                try
                {
                    typesInAsm = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    typesInAsm = ex.Types.Where(t => t != null).ToArray();
                }

                if (typesInAsm.Any(type => type.Namespace == targetNamespace))
                    return true;
            }

            return false;
        }

        private static void UpdateAssemblies()
        {
            s_assemblies.Clear();
            s_assemblies = GetAssemblies().ToList();
        }

        private static void UpdateInstalledPluginsInDoozySettings()
        {
            if (Settings == null) return;

            bool saveAssets = false;

            //DOTween - DG.Tweening
            bool hasDOTween = NamespaceExists(NAMESPACE_DG_TWEENING);
            if (Settings.DOTweenDetected != hasDOTween)
            {
                Settings.DOTweenDetected = hasDOTween;
                saveAssets = true;
            }

            //DOOZY UI version 2
            //Previous DoozyUI version - DoozyUI
            bool hasDoozyUI = NamespaceExists(NAMESPACE_DOOZYUI);
            if (Settings.DoozyUIVersion2Detected != hasDoozyUI)
            {
                Settings.DoozyUIVersion2Detected = hasDoozyUI;
                saveAssets = true;
            }

            //DOOZY UI version 3
            //Current DoozyUI version - Doozy.Engine
            bool hasDoozyEngine = NamespaceExists(NAMESPACE_DOOZY_ENGINE);
            if (Settings.DoozyUIVersion3Detected != hasDoozyEngine)
            {
                Settings.DoozyUIVersion3Detected = hasDoozyEngine;
                saveAssets = true;
            }

            if (hasDoozyEngine && !DefineSymbolsUtils.HasGlobalDefine(DEFINE_DOOZY_MANAGER))
                DefineSymbolsUtils.AddGlobalDefine(DEFINE_DOOZY_MANAGER);

            if (!saveAssets) return;
            Settings.SetDirty(false);
            s_saveAssets = true;
        }
    }
}