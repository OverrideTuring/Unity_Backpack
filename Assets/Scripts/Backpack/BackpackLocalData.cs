using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class BackpackLocalData
{
    private static BackpackLocalData _instance;

    public static BackpackLocalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackpackLocalData();
            }
            return _instance;
        }
    }

    public List<BackpackItem> items;
    // private string SaveFilePath => Path.Combine(Application.persistentDataPath, "backpack.json");
    private string SaveFilePath => Path.Combine(Application.streamingAssetsPath, "backpack.json");

    public List<BackpackItem> LoadBackpackData()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("Backpack data not found! Empty backpack initialized.");
            items = new List<BackpackItem>();
            return items;
        }

        try
        {
            string inventoryJson = File.ReadAllText(SaveFilePath);
            BackpackLocalData backpackLocalData = JsonUtility.FromJson<BackpackLocalData>(inventoryJson);
            items = backpackLocalData.items;
            Debug.Log("Backpack data loaded.");
        }
        catch (IOException e)
        {
            Debug.Log($"Backpack data failed loading: {e.Message}");
            items = new List<BackpackItem>();
        }
        return items;
    }

    public void SaveBackpackData()
    {
        items ??= new List<BackpackItem>();
        string inventoryJson = JsonUtility.ToJson(this, true);
        try
        {
            File.WriteAllText(SaveFilePath, inventoryJson);
            Debug.Log($"Backpack data saved to: {SaveFilePath}");
        }catch (IOException e)
        {
            Debug.LogError($"Backpack data failed saving: {e.Message}");
        }
    }
}

[System.Serializable]
public class BackpackItem
{
    public uint uid;

    public uint id;
    public ItemType type;
    public uint quality;
    public string icon;

    public uint amount;

    public override string ToString()
    {
        return $"item(uid = {uid}, id = {id}, type = {type.ToString()}, quality = {quality}, amount = {amount})";
    }
}