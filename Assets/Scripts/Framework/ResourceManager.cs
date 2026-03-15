using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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
   private void ParseFileText()
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
    /// <summary>
    /// 资源加载入口
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="callback"></param>
    public void LoadAsset(string assetName, Action<UnityEngine.Object> callback)
    {
        StartCoroutine(LoadBundleAsync(assetName, callback));
    }

    IEnumerator LoadBundleAsync(string assetName, Action<UnityEngine.Object> callback = null)
    {
        string bundleName = _BundleInfos[assetName].BundleName;
        string bundlePath = $"{PathUtil.BundleResourcePath}/{bundleName}";

        List<string> currentDependencies = _BundleInfos[assetName].Dependencies;

        if(currentDependencies != null && currentDependencies.Count > 0)
        {
            foreach(var dependency in currentDependencies)
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
    private void Start()
    {
        ParseFileText();
        LoadAsset("Assets/BuildResources/UI/Prefab/UITest.prefab", OnComplete);
    }

    private void OnComplete(UnityEngine.Object obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}
