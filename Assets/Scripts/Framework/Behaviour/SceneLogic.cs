using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLogic : LuaBehaviour
{
    public string sceneName;

    Action Active;
    Action InActive;
    Action Enter;
    Action Exit;
    public override void Init(string luaName)
    {
        base.Init(luaName);
        _ScriptEnv.Get("OnActive", out Active);
        _ScriptEnv.Get("OnInActive", out InActive);
        _ScriptEnv.Get("OnEnter", out Enter);
        _ScriptEnv.Get("OnExit", out Exit);
    }
    public void OnActive()
    {
        Active?.Invoke();
    }
    public void OnInActive()
    {
        InActive?.Invoke();
    }
    public void OnEnter()
    {
        Enter?.Invoke();
    }
    public void OnExit()
    {
        Exit?.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        Active = null;
        InActive = null;
        Enter = null;
        Exit = null;
    }
}
