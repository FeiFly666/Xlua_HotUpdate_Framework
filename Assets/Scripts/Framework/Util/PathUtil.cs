using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    //Unity根目录
    public static readonly string AssetPath = Application.dataPath;
    //打AB包资源目录
    public static readonly string BuildResourcesPath = AssetPath + "/BuildResources/";
    //AB包输出目录
    public static readonly string BundleBuildOutPath = Application.streamingAssetsPath;

    /// <summary>
    /// 获取Unity资源相对路径
    /// </summary>
    public static string GetUnityPath(string path)
    {
        if(string.IsNullOrEmpty(path)) return null;

        return path.Substring(path.IndexOf("Assets"));
    }

    /// <summary>
    /// 获取标准路径
    /// </summary>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        return path.Trim().Replace("\\","/");
    }
}
