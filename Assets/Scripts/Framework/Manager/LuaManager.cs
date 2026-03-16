using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;
using XLua;
using System;

public class LuaManager : MonoBehaviour
{
    public List<string> LuaNames = new List<string>();

    private Dictionary<string, byte[]> _LuaScripts;

    Action OnInitFinished;
    //Lua虚拟机，全局唯一
    public LuaEnv luaEnv;

    private void Awake()
    {
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(LuaLoader);
    }
    public void InitLua(Action initFinished)
    {
        OnInitFinished += initFinished;
        _LuaScripts = new Dictionary<string, byte[]>();
        if (AppConst.GameMode != GameLoadMode.Editor)
        {
            LoadLuaScript();
            return;
        }
#if UNITY_EDITOR
        EditorGetLuaScript();
#endif
    }
    byte[] LuaLoader(ref string name)
    {
        return GetLuaScript(name);
    }

    public byte[] GetLuaScript(string name)
    {
        name = name.Replace('.', '/');
        string fileName = PathUtil.LuaPath(name);

        byte[] scrpit = null;
        if(!_LuaScripts.TryGetValue(fileName, out scrpit))
        {
            Debug.LogError($"Lua脚本{name}不存在");
        }

        return scrpit;
    }
    public void StartLuaLoad(string fileName)
    {
        luaEnv.DoString($"require '{fileName}'");
    }
    public void LoadLuaScript()
    {
        foreach(string name in LuaNames)
        {
            Manager.Resource.LoadLua(name, (UnityObject obj)=>
            {
                AddLuaScript(name, (obj as TextAsset).bytes);

                if(_LuaScripts.Count >= LuaNames.Count)
                {
                    OnInitFinished?.Invoke();
                    LuaNames.Clear();
                    LuaNames = null;

                }
            });
        }
    }
    public void AddLuaScript(string fileName, byte[] content)
    {
        if(!_LuaScripts.ContainsKey(fileName))
        {
            _LuaScripts.Add(fileName, content);
        }
        else
        {
            _LuaScripts[fileName] = content;
        }
    }
#if UNITY_EDITOR
    void EditorGetLuaScript()
    {
        string[] luaFiles = Directory.GetFiles(PathUtil.LuaFilePath, "*.bytes" ,SearchOption.AllDirectories);
        foreach(var luaFile in luaFiles)
        {
            string fileName = PathUtil.GetStandardPath(luaFile);

            byte[] script = File.ReadAllBytes(fileName);

            AddLuaScript(PathUtil.GetUnityPath(fileName), script);
        }
        OnInitFinished?.Invoke();
    }
    
#endif
    [SerializeField] private float GCInterval = 0.2f;
    private float lastGCTime = 0;
    private void Update()
    {
        if(Time.time - lastGCTime > GCInterval)
        {
            lastGCTime = Time.time;
            if(luaEnv != null)
                luaEnv.Tick();
        }
    }
    private void OnDestroy()
    {
        if(luaEnv != null)
        {
            luaEnv.Dispose();
            luaEnv = null;
        }
    }
}
