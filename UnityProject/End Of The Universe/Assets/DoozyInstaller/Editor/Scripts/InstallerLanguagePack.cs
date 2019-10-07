// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Installer
{
    [Serializable]
    public class InstallerLanguagePack : LanguagePack
    {
        private static Language s_loadedLanguage = Language.Unknown;
        private static InstallerLanguagePack s_instance;

        public static InstallerLanguagePack Instance
        {
            get
            {
                if (s_instance != null && s_loadedLanguage == CurrentLanguage) return s_instance;
                s_instance = Resources.Load<InstallerLanguagePack>(typeof(InstallerLanguagePack).Name + CurrentLanguage);
                s_loadedLanguage = CurrentLanguage;
                return s_instance;
            }
        }

        public Language TargetLanguage = DEFAULT_LANGUAGE;

        public string AreYouSureYouWantToUninstallDoozyUIVersion2 = "Are you sure you want to uninstall DoozyUI version 2?";
        public string DoozyUIVersion2 = "DoozyUI version 2";
        public string Download = "Download";
        public string FixIssues = "Fix Issues";
        public string Install = "Install";
        public string InstallDoozyUI = "Install DoozyUI";
        public string Installed = "Installed";
        public string No = "No";
        public string NotInstalled = "Not Installed";
        public string Ready = "Ready";
        public string ToContinueDownloadAndInstallDOTween = "To continue, please download and install DOTween";
        public string ToContinueRemoveDoozyUI2 = "To continue, please remove DoozyUI version 2 from your project";
        public string ToContinueRunTheUpgradeProcess = "To continue, please run the 'Upgrade' process first";
        public string Uninstall = "Uninstall";
        public string Upgrade = "Upgrade";
        public string Version3IsNotCompatibleWithVersion2 = "DOOZY UI version 3 is not compatible with version 2";
        public string Yes = "Yes";
    }
}