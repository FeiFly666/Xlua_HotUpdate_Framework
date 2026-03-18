using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameLoadMode gameMode;
    public bool OpenLog = true;
    // Start is called before the first frame update
    void Start()
    {
        Manager.Event.Subscribe((int)GameEvent.StartLua, StartLua);
        Manager.Event.Subscribe((int)GameEvent.GameInit, GameInit);
        AppConst.GameMode = gameMode; 
        AppConst.OpenLog = OpenLog;
        DontDestroyOnLoad(this.gameObject);
        
        if(AppConst.GameMode == GameLoadMode.Update)
        {
            this.AddComponent<HotUpdate>();
        }
        else
        {
            Manager.Event.Execute((int)GameEvent.GameInit);
        }

    }
    private void GameInit(object args)
    {
        if(AppConst.GameMode != GameLoadMode.Editor)
            Manager.Resource.ParseFileText();
        Manager.Lua.InitLua();
    }
    private void StartLua(object args)
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
        Manager.Event.Unsubscribe((int)GameEvent.StartLua, StartLua);
        Manager.Event.Unsubscribe((int)GameEvent.GameInit, GameInit);
    }
}
