using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject
{
    public Object Object;

    public string name;

    public System.DateTime lastUsedTime;

    public PoolObject(string name, Object obj)
    {
        this.name = name;

        this.Object = obj;

        lastUsedTime = System.DateTime.Now;
    }
}
