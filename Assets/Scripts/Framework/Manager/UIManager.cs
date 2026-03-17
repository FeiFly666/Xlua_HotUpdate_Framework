using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    Dictionary<string, Transform> _UIGroup = new Dictionary<string, Transform>();

    private Transform _UIParent;

    private void Awake()
    {
        _UIParent = this.transform.parent.Find("UI");
    }

    public void SetUIGroup(List<string> groupNames)
    {
        foreach(string groupName in groupNames)
        {
            GameObject go = new GameObject($"Group_{groupName}");
            go.transform.SetParent(_UIParent, false);
            _UIGroup.Add(groupName, go.transform);
        }
    }
    public Transform GetUIGroup(string groupName)
    {
        if( _UIGroup.ContainsKey(groupName))
        {
            return _UIGroup[groupName];
        }
        Debug.LogError($"UI×é{groupName}Î´ÕÒµ½£¡");
        return null;
    }
    public void OpenUI(string uiName, string groupName,string luaName)
    {
        GameObject ui = null;
        Transform group = GetUIGroup(groupName);

        string uiPath = PathUtil.UIPath(uiName);
        Object uiObj = Manager.Pool.Spawn("UI", uiPath);

        if(uiObj != null)
        {
            ui = uiObj as GameObject;

            ui.transform.SetParent(group, false);

            UILogic logic = ui.GetComponent<UILogic>();
            logic.OnOpen();
            return;
        }

        Manager.Resource.LoadUI(uiName, (Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;

            ui.transform.SetParent(group, false);

            UILogic uiLogic = ui.AddComponent<UILogic>();
            uiLogic.assetName = uiPath;
            uiLogic.Init(luaName);
            uiLogic.OnOpen();
        });

    }
}
