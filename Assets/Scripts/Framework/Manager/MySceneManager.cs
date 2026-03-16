using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    private string _GoName = "{SceneLogic}";

    private void Awake()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    //叠加加载
    public void LoadScene(string sceneName, string luaName)
    {
        Manager.Resource.LoadScene(sceneName, (Object)=>
        {
            StartCoroutine(StartLoadScene(sceneName,luaName,LoadSceneMode.Additive));
        });
    }
    //默认切换
    public void ChangeScene(string sceneName,string luaName)
    {
        Manager.Resource.LoadScene(sceneName, (Object) =>
        {
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
        });
    }

    public void ActiveScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }
    private void OnActiveSceneChanged(Scene lastScene, Scene currentScene)
    {
        if(!lastScene.isLoaded || !currentScene.isLoaded)
        {
            return;
        }
        SceneLogic lastSceneLogic = GetSceneLogic(lastScene);
        SceneLogic currentSceneLogic = GetSceneLogic(currentScene);

        lastSceneLogic?.OnInActive();
        currentSceneLogic?.OnActive();
    }
    //卸载

    IEnumerator StartLoadScene(string sceneName, string luaName, LoadSceneMode mode)
    {
        if(IsLoadedScene(sceneName))
        {
            yield break;
        }
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, mode);
        async.allowSceneActivation = true; //不设置true会加载不全
        yield return async;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        
        GameObject go = new GameObject(_GoName);

        SceneManager.MoveGameObjectToScene(go, scene);

        SceneLogic logic = go.AddComponent<SceneLogic>();
        logic.sceneName = sceneName;
        logic.Init(luaName);
        logic.OnEnter();
    }

    IEnumerator UnloadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if(!scene.isLoaded)
        {
            Debug.LogError($"正尝试卸载一个未加载场景{sceneName}");
            yield break;
        }
        SceneLogic logic = GetSceneLogic(scene);
        logic?.OnExit();

        AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
        yield return async;
    }

    private bool IsLoadedScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene != null && scene.isLoaded;
    }
    private SceneLogic GetSceneLogic(Scene scene)
    {
        GameObject[] allGo = scene.GetRootGameObjects();
        foreach(var go in allGo)
        {
            if(go.name.CompareTo(_GoName) == 0)
            {
                SceneLogic logic = go.GetComponent<SceneLogic>();
                return logic;
            }
        }
        return null;
    }
}
