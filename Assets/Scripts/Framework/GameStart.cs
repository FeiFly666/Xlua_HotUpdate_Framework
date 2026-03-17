using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameLoadMode gameMode;
    // Start is called before the first frame update
    void Awake()
    {
        AppConst.GameMode = gameMode; 
        DontDestroyOnLoad(this.gameObject);

        Manager.Event.Subscribe(1, OnLuaInit);

    }
    private void Start()
    {
        Manager.Resource.ParseFileText();
        Manager.Lua.InitLua();
    }
    private void OnLuaInit(object args)
    {
        Manager.Lua.StartLuaLoad("test");

        XLua.LuaFunction func = Manager.Lua.luaEnv.Global.Get<XLua.LuaFunction>("Main");
        func?.Call();

        Manager.Pool.CreateGameObjectPool("UI", 10);
        Manager.Pool.CreateGameObjectPool("Monster", 100);
        Manager.Pool.CreateGameObjectPool("Effect", 100);
        Manager.Pool.CreateBundlePool("AssetBundle", 10);
    }
    private void OnApplicationQuit()
    {
        Manager.Event.Unsubscribe(1, OnLuaInit);
    }
}
