using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static ResourceManager _resource;
    private static LuaManager _lua;
    private static UIManager _ui;
    private static EntityManager _entity;
    private static MySceneManager _scene;

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
    public static EntityManager Entity
    {
        get { return _entity; }
    }
    public static MySceneManager Scene
    {
        get { return _scene; }
    }
    private void Awake()
    {
        _lua = this.AddComponent<LuaManager>();
        _resource = this.AddComponent<ResourceManager>();
        _ui = this.AddComponent<UIManager>();
        _entity = this.AddComponent<EntityManager>();
        _scene = this.AddComponent<MySceneManager>();
    }

}
