using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjectPool : Pool
{
    public override Object Spawn(string name)
    {
        Object obj = base.Spawn(name);
        if(obj == null)
        {
            return null;
        }
        GameObject go = obj as GameObject;

        go.SetActive(true);

        return obj;
    }

    public override void UnSpawn(string name, Object obj)
    {
        GameObject go = obj as GameObject;
        go.SetActive(false);

        go.transform.SetParent(this.transform, false);
        base.UnSpawn(name, obj);
    }

    public override void Release()
    {
        base.Release();

        foreach(var item in _ObjectPool.ToList())
        {
            if(System.DateTime.Now.Ticks - item.lastUsedTime.Ticks >= _ReleaseTime * 10000000)
            {
                Debug.Log($"游戏物体对象池 释放对象 ：{item.Object.name}");

                Manager.Resource.DecreaseAllBundleCount(item.name);

                Destroy(item.Object);
                _ObjectPool.Remove(item);
            }
        }
    }
}
