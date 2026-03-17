using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILogic : LuaBehaviour
{
    public string assetName;
    Action _LuaOpen;
    Action _LuaClose;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        _ScriptEnv.Get("OnOpen", out _LuaOpen);
        _ScriptEnv.Get("OnClose", out _LuaClose);
    }

    public void OnOpen()
    {
        _LuaOpen();
    }

    public void OnClose()
    {
        _LuaClose();
        Manager.Pool.UnSpawn("UI",assetName,this.gameObject);
    }
    protected override void Clear()
    {
        base.Clear();
        _LuaOpen = null;
        _LuaClose = null;
    }
}
