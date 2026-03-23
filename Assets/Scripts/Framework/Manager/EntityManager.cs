using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EntityManager : MonoBehaviour
{
    Dictionary<string,GameObject> _Entities = new Dictionary<string,GameObject>();

    Dictionary<string,Transform> _EntityGroups = new Dictionary<string,Transform>();

    private Transform _EntityTransform;

    private void Awake()
    {
        _EntityTransform = this.transform.parent.Find("Entity");
    }

    public void SetEntityGroup(List<string> groupNames)
    {
        foreach(var groupName in groupNames)
        {
            GameObject go = new GameObject($"Group_{groupName}");
            go.transform.SetParent(_EntityTransform, false);

            _EntityGroups.Add(groupName, go.transform);
        }
    }
    public Transform GetEntityGroup(string groupName)
    {
        if(_EntityGroups.ContainsKey(groupName))
        {
            return _EntityGroups[groupName];
        }
        Debug.LogError($"实体组{groupName}未找到！！");
        return null;
    }

    public void ShowEntity(string name, string groupName, string luaName, System.Action<EntityLogic> onComplete = null)
    {
        GameObject entity = null;
/*        if(_Entities.TryGetValue(name, out entity))
        {
            EntityLogic entityLogic = entity.GetComponent<EntityLogic>();

            onComplete?.Invoke(entityLogic);

            entityLogic.OnShow();
            return;
        }*/

        Manager.Resource.LoadPrefab(name, (Object) =>
        {
            entity = Instantiate(Object) as GameObject;

            Transform group = GetEntityGroup(groupName);
            entity.transform.SetParent(group, false);

            //_Entities.Add(name, entity);

            EntityLogic logic = entity.AddComponent<EntityLogic>();

            logic.Init(luaName);

            onComplete?.Invoke(logic);

            logic.OnShow();
        });
    }
}
