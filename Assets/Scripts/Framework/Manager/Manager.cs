using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static ResourceManager _resource;
    private static LuaManager _lua;
    private static UIManager _ui;

    public static ResourceManager Resource
    {
        get { return _resource; }
    }
    public static LuaManager Lua
    {
        get { return _lua; }
    }
    public static UIManager UI
    {
        get { return _ui; }
    }
    private void Awake()
    {
        _lua = this.AddComponent<LuaManager>();
        _resource = this.AddComponent<ResourceManager>();
        _ui = this.AddComponent<UIManager>();
    }

}
