using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    Transform _PoolParent;

    Dictionary<string,Pool> _ObjectPools = new Dictionary<string,Pool>();
    void Awake()
    {
        _PoolParent = this.transform.parent.Find("Pool").transform;
    }

    void CreatePool<T>(string poolName, float releaseTime) where T : Pool
    {
        if(!_ObjectPools.ContainsKey(poolName))
        {
            GameObject go = new GameObject(poolName);

            go.transform.SetParent(_PoolParent);

            T pool = go.AddComponent<T>();
            pool.Init(releaseTime);

            _ObjectPools.Add(poolName, pool);
        }
    }

    public void CreateGameObjectPool(string poolName, float releaseTime)
    {
        CreatePool<GameObjectPool>(poolName, releaseTime);
    }

    public void CreateBundlePool(string poolName, float releaseTime)
    {
        CreatePool<BundlePool>(poolName, releaseTime);
    }

    public Object Spawn(string poolName, string assetName)
    {
        if(_ObjectPools.TryGetValue(poolName, out Pool pool))
        {
            return pool.Spawn(assetName);
        }
        return null;
    }
    public void UnSpawn(string poolName, string assetName, Object asset)
    {
        if (_ObjectPools.TryGetValue(poolName, out Pool pool))
        {
            pool.UnSpawn(assetName, asset);
        }
    }
}
