using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pool : MonoBehaviour
{
    protected float _ReleaseTime;

    protected long _LastReleaseTime = 0;

    protected List<PoolObject> _ObjectPool;

    public void Init(float time)
    {
        _ReleaseTime = time;
        _LastReleaseTime = System.DateTime.Now.Ticks;
        _ObjectPool = new List<PoolObject>(); 
    }
    private void Update()
    {
        if(System.DateTime.Now.Ticks - _LastReleaseTime >= _ReleaseTime * 10000000)
        {
            Release();
            _LastReleaseTime = System.DateTime.Now.Ticks;
        }
    }
    public virtual Object Spawn(string name)
    {
        foreach (var Object in _ObjectPool)
        {
            if(Object.name == name)
            {
                _ObjectPool.Remove(Object);
                return Object.Object;
            }
        }
        return null;
    }
    public virtual void UnSpawn(string name, Object obj)
    {
        PoolObject po = new PoolObject(name, obj);

        _ObjectPool.Add(po);
    }

    public virtual void Release()
    {

    }
}