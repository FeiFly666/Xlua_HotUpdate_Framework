using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.VisualScripting;
public class BuildTool : Editor
{
    [MenuItem("Tools/Build Windows AssetBundle")]
    static void BuildWindowsBundle()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build Android AssetBundle")]
    static void BuildAndroidBundle()
    {
        Build(BuildTarget.Android);
    }

    [MenuItem("Tools/Build IOS AssetBundle")]
    static void BuildIOSBundle()
    {
        Build(BuildTarget.iOS);
    }
    static void Build(BuildTarget targetPlatform)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();

        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if(file.EndsWith(".meta"))
            {
                continue;
            }
            AssetBundleBuild assetBundle = new AssetBundleBuild();
            string standardFilePath = PathUtil.GetStandardPath(file);
            
            string assetName = PathUtil.GetUnityPath(standardFilePath);
            string bundleName = file.Replace(PathUtil.BuildResourcesPath,"").ToLower();


            assetBundle.assetNames = new string[] { assetName };
            assetBundle.assetBundleName = $"{bundleName}.ab";

            assetBundleBuilds.Add(assetBundle);
        }
        
        if(Directory.Exists(PathUtil.BundleBuildOutPath))
        {
            Directory.Delete(PathUtil.BundleBuildOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleBuildOutPath);

        BuildPipeline.BuildAssetBundles(PathUtil.BundleBuildOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, targetPlatform);


    }
}
