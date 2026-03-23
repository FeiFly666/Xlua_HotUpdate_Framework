using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLogic : LuaBehaviour
{
    Action Show;
    Action Hide;
    Action<Collision> CollisionEnter;
    public void SetLuaValue(string key, object value)
    {
        _ScriptEnv.Set(key, value);
    }
    public override void Init(string luaName)
    {
        base.Init(luaName);
        _ScriptEnv.Get("OnShow", out Show);
        _ScriptEnv.Get("OnHide", out Hide);
        _ScriptEnv.Get("OnCollisionEnter", out CollisionEnter);
    }
    public void OnShow()
    {
        Show?.Invoke();
    }
    public void OnHide()
    {
        Hide?.Invoke();
        Destroy(this.gameObject);
    }
    protected override void Clear()
    {
        base.Clear();
        Show = null;
        Hide = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionEnter?.Invoke(collision);
    }
}
