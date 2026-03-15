using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.VisualScripting;
using System.Xml;
using System.Linq;
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
        List<string> bundleInfos = new List<string>();

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
            string bundleName = standardFilePath.Replace(PathUtil.BuildResourcesPath,"").ToLower();


            assetBundle.assetNames = new string[] { assetName };
            assetBundle.assetBundleName = $"{bundleName}{AppConst.BundleExtension}";

            assetBundleBuilds.Add(assetBundle);

            //生成Bundle信息： 资源名称|Bundle名称|依赖资源列表
            List<string> bundleDependencies = GetAllDependencies(assetName);
            string bundleInfo = $"{assetName}|{assetBundle.assetBundleName}";

            if(bundleDependencies.Count > 0)
            {
                bundleInfo = bundleInfo + "|" + string.Join("|",bundleDependencies) ;
            }

            bundleInfos.Add(bundleInfo);
        }


        if(Directory.Exists(PathUtil.BundleBuildOutPath))
        {
            Directory.Delete(PathUtil.BundleBuildOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleBuildOutPath);
        File.WriteAllLines($"{PathUtil.BundleBuildOutPath}/{AppConst.FileListName}", bundleInfos);

        BuildPipeline.BuildAssetBundles(PathUtil.BundleBuildOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, targetPlatform);

        AssetDatabase.Refresh();

    }

    /// <summary>
    /// 获取所有依赖文件，返回列表
    /// </summary>
    /// <param name="currentFile"></param>
    /// <returns></returns>
    static List<string> GetAllDependencies(string currentFile)
    {
        List<string> dependencies = new List<string>();

        string[] files = AssetDatabase.GetDependencies(currentFile);

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];

            if (!file.EndsWith(".cs")&& !file.Equals(currentFile))
            {
                dependencies.Add(PathUtil.GetStandardPath(file));
            }
        }
        return dependencies;
    }
}
