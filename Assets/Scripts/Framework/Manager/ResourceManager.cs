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
    internal class BundleData
    {
        public AssetBundle assetBundle;
        public int Count;

        public BundleData(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
            this.Count = 1;
        }
    }

    private Dictionary<string,BundleInfo> _BundleInfos = new Dictionary<string,BundleInfo>();

    private Dictionary<string, BundleData> _AssetBundles = new Dictionary<string,BundleData>();

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

            if(!_BundleInfos.ContainsKey(bundleInfo.AssetName))
                _BundleInfos.Add(bundleInfo.AssetName,bundleInfo);

            if (splitInfos[0].IndexOf("LuaScripts")>0)
            {
                if (!Manager.Lua.LuaNames.Contains(splitInfos[0]))
                    Manager.Lua.LuaNames.Add(splitInfos[0]);
            }
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
    private BundleData GetAssetBundle(string bundleName)
    {
        BundleData bundle = null;
        if(_AssetBundles.ContainsKey(bundleName))
        {
            bundle = _AssetBundles[bundleName];
            bundle.Count++;
        }
        return bundle;
    }
    IEnumerator LoadBundleAsync(string assetName, Action<UnityObject> callback = null)
    {
        string bundleName = _BundleInfos[assetName].BundleName;
        string bundlePath = $"{PathUtil.BundleResourcePath}/{bundleName}";
        List<string> currentDependencies = _BundleInfos[assetName].Dependencies;

        BundleData bundle = GetAssetBundle(bundleName);
        if (bundle == null)
        {
            UnityObject obj = Manager.Pool.Spawn("AssetBundle", bundleName);
            if(obj != null)
            {
                AssetBundle b = obj as AssetBundle;
                bundle = new BundleData(b);
            }
            else
            {
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return request;

                bundle = new BundleData(request.assetBundle);
            }

            _AssetBundles.Add(bundleName, bundle);
        }

        if (currentDependencies != null && currentDependencies.Count > 0)
        {
            foreach (var dependency in currentDependencies)
            {
                yield return LoadBundleAsync(dependency);
            }
        }

        if (assetName.EndsWith(".unity"))
        {
            callback?.Invoke(null);
            yield break;
        }

        if(callback == null)//依赖资源不需要加载具体资源以及回调
        {
            yield break;
        }

        AssetBundleRequest bundleRequest = bundle.assetBundle.LoadAssetAsync(assetName);
        yield return bundleRequest;

        callback?.Invoke(bundleRequest?.asset);

    }

    public void LoadUI(string assetName, Action<UnityObject> callback = null)
    {
        LoadAsset(PathUtil.UIPath(assetName), callback);
    }
    public void LoadLua(string assetName, Action<UnityObject> callback = null)
    {
        LoadAsset(assetName, callback);
    }
    public void LoadPrefab(string assetName, Action<UnityObject> callback = null)
    {
        LoadAsset(assetName, callback);
    }
    public void LoadScene(string assetName, Action<UnityObject> callback = null)
    {
        LoadAsset(PathUtil.ScenePath(assetName), callback);
    }
    public void LoadMusic(string assetName, Action<UnityObject> callback = null)
    {
        LoadAsset(PathUtil.MusicPath(assetName), callback);
    }
    public void LoadSound(string assetName, Action<UnityObject> callback = null)
    {
        LoadAsset(PathUtil.SoundPath(assetName), callback);
    }
    //减去bundle及其依赖的计数
    public void DecreaseAllBundleCount(string assetName)
    {
        string bundleName = _BundleInfos[assetName].BundleName;
        List<string> currentDependencies = _BundleInfos[assetName].Dependencies;

        DecreaseBundleCount(assetName);

        if(currentDependencies != null)
        {
            foreach(var dependency in currentDependencies)
            {
                string name = _BundleInfos[dependency].BundleName;
                DecreaseBundleCount(name);
            }
        }

    }

    public void DecreaseBundleCount(string bundleName)
    {
        if(_AssetBundles.TryGetValue(bundleName, out var bundle))
        {
            if(bundle.Count > 0)
            {
                bundle.Count--;
                Debug.Log($"Bundle: {bundleName} 计数减一 count:{bundle.Count}");
            }
            if(bundle.Count <= 0)
            {
                Debug.Log($"Bundle: {bundleName} 移入对象池，长时间不再使用将销毁");
                Manager.Pool.UnSpawn("AssetBundle", bundleName, bundle.assetBundle);
                _AssetBundles.Remove(bundleName);
            }
        }
    }

    public void UnloadBundle(UnityObject obj)
    {
        AssetBundle ab = obj as AssetBundle;
        ab.Unload(true);
    }
}
