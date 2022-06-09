/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

#if UNITY_EDITOR && UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

public class IOSSettingsPostProcess : IPostprocessBuildWithReport
{
    public int callbackOrder
    {
        get { return 0; }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS)
        {
            
            var plistPath = Path.Combine(report.summary.outputPath, "Info.plist");
            var plist = new PlistDocument();
            try
            {
                plist.ReadFromFile(plistPath);

                plist.root.SetBoolean("UIFileSharingEnabled", true);
                plist.root.SetBoolean("LSSupportsOpeningDocumentsInPlace", true);

                plist.WriteToFile(plistPath);
            } catch (System.Exception e)
            {
                UnityEngine.Debug.Log("Setting flags failed. Reason: " + e.Message);
            }
        }
        
    }

}
#endif
