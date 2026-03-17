using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BundlePool : Pool
{
    public override void Release()
    {
        base.Release();

        foreach (var item in _ObjectPool.ToList())
        {
            if (System.DateTime.Now.Ticks - item.lastUsedTime.Ticks >= _ReleaseTime * 10000000)
            {
                Debug.Log($"Bundle对象池 释放Bundle：{item.name}");

                Manager.Resource.UnloadBundle(item.Object);

                _ObjectPool.Remove(item);
            }
        }
    }
}

