// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEditor;
using UnityEngine;

namespace Doozy.Installer
{
    [Serializable]
    public class InstallerSettings : ScriptableObject
    {
        private const string FILE_NAME = "InstallerSettings";
        public static string ResourcesPath { get { return "Assets/DoozyInstaller/Editor/Resources"; } }

        private static InstallerSettings s_instance;

        public static InstallerSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = GetScriptableObject<InstallerSettings>(FILE_NAME, ResourcesPath);
                return s_instance;
            }
        }

        public GUISkin Skin;

        public bool DOTweenDetected;
        public bool DoozyUIVersion2Detected;
        public bool DoozyUIVersion3Detected;

        public bool InstallExecuted;

        #region Version

        public int VersionMajor = 3;
        public int VersionMinor = 0;
        public int VersionStatus = 2;
        public int VersionRevision = 0;

        public string VersionReleaseStatus
        {
            get
            {
                switch (VersionStatus)
                {
                    case 0:  return "a"; //alpha
                    case 1:  return "b"; //beta
                    case 2:  return "c"; //commercial distribution
                    case 3:  return "d"; //debug
                    default: return "?";
                }
            }
        }

        public string Version
        {
            get
            {
                return VersionMajor + "." +
                       VersionMinor + "." +
                       VersionReleaseStatus + VersionRevision;
            }
        }

        /// <summary> Set DoozyUI version that will get installed </summary>
        /// <param name="major"> Major Version Number </param>
        /// <param name="minor"> Minor Version Number </param>
        /// <param name="status"> Version Status (0 - alpha, 1 - beta, 2 - commercial distribution, 3 - debug) (a,b,c,d) </param>
        /// <param name="revision"> Revision Number </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetVersion(int major, int minor, int status, int revision, bool saveAssets = false)
        {
            VersionMajor = major;
            VersionMinor = minor;
            VersionStatus = status;
            VersionRevision = revision;
            SetDirty(saveAssets);
        }

        #endregion


        public string DataPath = "Assets/DoozyInstaller/Editor/Data/";
        public string PackageName = "Data";
        public string PackageExtension = ".unitypackage";

        public static T GetScriptableObject<T>(string fileName, string resourcesPath) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(resourcesPath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!resourcesPath[resourcesPath.Length - 1].Equals(@"\")) resourcesPath += @"\";

            var obj = (T) Resources.Load(fileName, typeof(T));
#if UNITY_EDITOR
            if (obj != null) return obj;
            obj = CreateAsset<T>(resourcesPath, fileName);
#endif
            return obj;
        }

        private static T CreateAsset<T>(string relativePath, string fileName, string extension = ".asset")
            where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(relativePath)) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!relativePath[relativePath.Length - 1].Equals(@"\")) relativePath += @"\";
            var asset = CreateInstance<T>();
            if (!AssetDatabase.IsValidFolder(relativePath)) return null;
            AssetDatabase.CreateAsset(asset, relativePath + fileName + extension);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
            return asset;
        }

        public void SetDirty(bool saveAssets)
        {
            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();
        }
    }
}