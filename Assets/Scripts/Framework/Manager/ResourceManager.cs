using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using UnityObject = UnityEngine.Object;

public class ResourceManager : MonoBehaviour
{
    private class BundleInfo
    {
        public string AssetName;
        public string BundleName;
        public List<string> Dependencies;
    }

    private Dictionary<string,BundleInfo> _BundleInfos = new Dictionary<string,BundleInfo>();

    /// <summary>
    /// 解析版本文件信息
    /// </summary>
   public void ParseFileText()
    {
        string url = $"{PathUtil.BundleResourcePath}/{AppConst.FileListName}";

        string[] data = File.ReadAllLines(url);

        //解析文件
        for (int i = 0; i < data.Length; i++)
        {
            BundleInfo bundleInfo = new BundleInfo();

            string[] splitInfos = data[i].Split('|');

            bundleInfo.AssetName = splitInfos[0];
            bundleInfo.BundleName = splitInfos[1];
            bundleInfo.Dependencies = new List<string>(splitInfos.Length - 2);
            for(int j = 2; j < splitInfos.Length; j++)
            {
                bundleInfo.Dependencies.Add(splitInfos[j]);
            }

            _BundleInfos.Add(bundleInfo.AssetName,bundleInfo);
        }
    }
#if UNITY_EDITOR
    // 编辑器调用资源加载入口
    void EditorLoadAsset(string assetName, Action<UnityObject> callback)
    {
        Debug.Log("编辑器资源加载");
        UnityObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UnityObject));
        if(obj == null)
        {
            Debug.LogError($"未找到资源，资源名称：{assetName}");
            return;
        }
        callback?.Invoke(obj);
    }
#endif
    /// <summary>
    /// 统一资源加载入口
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="callback"></param>
    private void LoadAsset(string assetName, Action<UnityObject> callback)
    {
        if(AppConst.GameMode == GameLoadMode.Editor)
        {
            #if UNITY_EDITOR
            EditorLoadAsset(assetName, callback);
            #endif
            return;
        }
        StartCoroutine(LoadBundleAsync(assetName, callback));
    }
    IEnumerator LoadBundleAsync(string assetName, Action<UnityObject> callback = null)
    {
        string bundleName = _BundleInfos[assetName].BundleName;
        string bundlePath = $"{PathUtil.BundleResourcePath}/{bundleName}";

        List<string> currentDependencies = _BundleInfos[assetName].Dependencies;

        if (currentDependencies != null && currentDependencies.Count > 0)
        {
            foreach (var dependency in currentDependencies)
            {
                yield return LoadBundleAsync(dependency);
            }
        }

        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        AssetBundleRequest bundleRequest = request.assetBundle.LoadAssetAsync(assetName);
        yield return bundleRequest;

        callback?.Invoke(bundleRequest?.asset);

    }

    public void LoadUI(string assetName, Action<UnityObject> callback = null)
    {
        LoadAsset(PathUtil.UIPath(assetName), callback);
    }
    public void LoadLua(string assetName, Action<UnityObject> callback = null)
    {
        LoadAsset(PathUtil.LuaPath(assetName), callback);
    }

}
