using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //»º´æUI×Öµä
    Dictionary<string, GameObject> _UI = new Dictionary<string, GameObject>();

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
        if(_UI.TryGetValue( uiName, out ui))
        {
            UILogic uiLogic = ui.GetComponent<UILogic>();
            uiLogic.OnOpen();
            return;
        }
        Manager.Resource.LoadUI(uiName, (Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;
            _UI.Add(uiName, ui);

            Transform group = GetUIGroup(groupName);

            ui.transform.SetParent(group, false);

            UILogic uiLogic = ui.AddComponent<UILogic>();
            uiLogic.Init(luaName);
            uiLogic.OnOpen();
        });

    }
}
