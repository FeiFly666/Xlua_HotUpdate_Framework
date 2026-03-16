using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    public string luaName;
    private LuaEnv _LuaEnv = Manager.Lua.luaEnv;

    protected LuaTable _ScriptEnv;

    private Action _LuaAwake;
    private Action _LuaStart;
    private Action _LuaUpdate;
    private Action _LuaOnDestroy;

    private void Awake()
    {
        _ScriptEnv = _LuaEnv.NewTable();

        LuaTable metaTable = _LuaEnv.NewTable();
        metaTable.Set("__index", _LuaEnv.Global);

        _ScriptEnv.SetMetaTable(metaTable);
        metaTable.Dispose();
        metaTable = null;

    }
    public virtual void Init(string luaName)
    {
        this.luaName = luaName;

        _LuaEnv.DoString(Manager.Lua.GetLuaScript(luaName), luaName, _ScriptEnv);

        _ScriptEnv.Set("self", this);

        _ScriptEnv.Get("Awake", out _LuaAwake);
        _ScriptEnv.Get("Start", out _LuaStart);
        _ScriptEnv.Get("Update", out _LuaUpdate);
        _ScriptEnv.Get("OnDestroy", out _LuaOnDestroy);

        _LuaAwake?.Invoke();
    }
    private void Update()
    {
        _LuaUpdate?.Invoke();
    }
    private void OnDestroy()
    {
        _LuaOnDestroy?.Invoke();
        Clear();
    }
    private void OnApplicationQuit()
    {
        Clear();
    }
    protected virtual void Clear()
    {
        _LuaAwake = null;
        _LuaStart = null;
        _LuaUpdate = null;
        _LuaOnDestroy = null;

        _ScriptEnv?.Dispose();
        _ScriptEnv = null;
    }
}
