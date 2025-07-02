using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    private Transform _uiRoot;
    private Dictionary<string, string> pathDict;
    private Dictionary<string, GameObject> prefabDict;
    private Dictionary<string, BasePanel> panelDict;

    private UIManager()
    {
        Initialize();
    }

    private void Initialize()
    {
        _uiRoot = GameObject.Find("Canvas").transform;
        pathDict = new Dictionary<string, string>()
        {
            {UIConst.MenuPanel, "Prefabs/UI/MenuPanel" },
            {UIConst.BackpackPanel, "Prefabs/UI/BackpackPanel" }
        };
        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();
    }

    public BasePanel OpenPanel(string name)
    {
        return OpenPanel<BasePanel>(name);
    }

    public TPanel OpenPanel<TPanel>(string name) where TPanel: BasePanel
    {
        if (panelDict.TryGetValue(name, out BasePanel panel))
        {
            Debug.LogError($"Panel `{name}` already opened.");
            return null;
        }

        string path;
        if (!pathDict.TryGetValue(name, out path))
        {
            Debug.LogError($"Panel `{name}` not exists.");
            return null;
        }

        GameObject prefab;
        if (!prefabDict.TryGetValue(name, out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            prefabDict.Add(name, prefab);
        }

        GameObject panelObject = GameObject.Instantiate(prefab, _uiRoot, false);
        TPanel returnPanel = panelObject.GetComponent<TPanel>();
        panelDict.Add(name, returnPanel);
        returnPanel.OpenPanel();
        return returnPanel;
    }

    public bool ClosePanel(string name)
    {
        if (panelDict.TryGetValue (name, out BasePanel panel))
        {
            panel.ClosePanel();
            panelDict.Remove(name);
            return true;
        }
        return false;
    }
}

public class UIConst
{
    public const string MenuPanel = "MenuPanel";
    public const string BackpackPanel = "BackpackPanel";
}