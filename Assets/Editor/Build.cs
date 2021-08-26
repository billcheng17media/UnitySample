#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections.Generic;
using System.IO;

// NOTE (Daniel) This process changes some of the default auto generated files for iOS so that we can put unity inside any view we want instead of letting
// it put itself infront of all the views.
public class IOSPostProcess
{
    [PostProcessBuild]
    static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS) {
            return;
        }

        string projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

        string iphoneTarget = proj.GetUnityMainTargetGuid();
        string frameworkTarget = proj.GetUnityFrameworkTargetGuid();

        proj.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", "NO");

        // Data folder needs to be set to UnityFramework target
        string dataFolder = proj.FindFileGuidByProjectPath("Data");
        proj.RemoveFileFromBuild(iphoneTarget, dataFolder);
        proj.AddFileToBuild(frameworkTarget, dataFolder);

        // Set NativeCalls.h to public
        string nativeCallsHeader = proj.FindFileGuidByProjectPath("Libraries/Plugins/iOS/NativeCallProxy.h");
        proj.AddPublicHeaderToBuild(frameworkTarget, nativeCallsHeader);        
        string unityGraphics = proj.FindFileGuidByProjectPath("Classes/Unity/IUnityGraphics.h");
        proj.AddPublicHeaderToBuild(frameworkTarget, unityGraphics);
        string unityInterface = proj.FindFileGuidByProjectPath("Classes/Unity/IUnityInterface.h");
        proj.AddPublicHeaderToBuild(frameworkTarget, unityInterface);

        proj.WriteToFile(projPath);
    }
}
#endif