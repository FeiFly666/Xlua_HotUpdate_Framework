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

    }
    private void Start()
    {
        Manager.Resource.ParseFileText();
        Manager.Lua.InitLua(Test);
    }
    private void Test()
    {
        Manager.Lua.StartLuaLoad("test");

        XLua.LuaFunction func = Manager.Lua.luaEnv.Global.Get<XLua.LuaFunction>("Main");
        func?.Call();
    }
}
