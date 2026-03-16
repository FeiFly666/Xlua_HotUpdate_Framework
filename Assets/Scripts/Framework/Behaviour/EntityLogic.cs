using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLogic : LuaBehaviour
{
    Action Show;
    Action Hide;
    public override void Init(string luaName)
    {
        base.Init(luaName);
        _ScriptEnv.Get("OnShow", out Show);
        _ScriptEnv.Get("OnHide", out Hide);
    }
    public void OnShow()
    {
        Show?.Invoke();
    }
    public void OnHide()
    {
        Hide?.Invoke();
    }
    protected override void Clear()
    {
        base.Clear();
        Show = null;
        Hide = null;
    }
}
