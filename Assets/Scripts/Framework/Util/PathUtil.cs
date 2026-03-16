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
    //只读目录
    public static readonly string ReadPath = Application.streamingAssetsPath;
    //可读写目录
    public static readonly string WritablePath = Application.persistentDataPath;
    //Lua脚本文件路径
    public static readonly string LuaFilePath = BuildResourcesPath + "LuaScripts/";
    //版本文件读取路径
    public static string BundleResourcePath
    {
        get 
        { 
            if(AppConst.GameMode == GameLoadMode.Update)
            {
                return WritablePath;
            }
            return ReadPath; 
        }
    }

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

    public static string UIPath(string name)
    {
        return $"Assets/BuildResources/UI/Prefab/{name}.prefab";
    }
    public static string LuaPath(string name)
    {
        return $"Assets/BuildResources/LuaScripts/{name}.bytes";
    }
    public static string SoundPath(string name)
    {
        return $"Assets/BuildResources/Audio/Sound/{name}";
    }
    public static string MusicPath(string name)
    {
        return $"Assets/BuildResources/Audio/Music/{name}";
    }
    public static string ScenePath(string name)
    {
        return $"Assets/BuildResources/Scene/{name}.unity";
    }
    public static string SpritePath(string name)
    {
        return $"Assets/BuildResources/Sprites/{name}";
    }
}
