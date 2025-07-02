using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GMCmd
{
    [MenuItem("GMCmd/Read Backpack Table")]
    public static void ReadBackpackTable()
    {
        BackpackTable backpackTable = Resources.Load<BackpackTable>("BackpackTable/BackpackTable");
        foreach (BackpackTableItem item in backpackTable.items)
        {
            Debug.LogFormat("item(id = {0}, type = {1}, quality = {2})", item.id, item.type.ToString(), item.quality);
        }
    }

    [MenuItem("GMCmd/Create Local Backpack Data")]
    public static void CreateLocalBackpackData()
    {
        const int maxCount = 2000;

        BackpackTable backpackTable = Resources.Load<BackpackTable>("BackpackTable/BackpackTable");
        BackpackLocalData.Instance.items = new List<BackpackItem>();
        for (int i = 0; i < maxCount; i++)
        {
            // BackpackItem tableItem = backpackTable.items[i % backpackTable.items.Count()];
            BackpackTableItem tableItem = backpackTable.items[Random.Range(0, backpackTable.items.Count())];
            BackpackItem item = new BackpackItem()
            {
                uid = (uint)(i + 1),
                id = tableItem.id,
                type = tableItem.type,
                quality = tableItem.quality,
                icon = tableItem.icon,
                amount = (uint)Random.Range(1, 1001)
            };
            BackpackLocalData.Instance.items.Add(item);
        }
        BackpackLocalData.Instance.SaveBackpackData();
    }

    [MenuItem("GMCmd/Read Local Backpack Data")]
    public static void ReadLocalBackpackData()
    {
        List<BackpackItem> items = BackpackLocalData.Instance.LoadBackpackData();
        foreach (BackpackItem item in items)
        {
            Debug.Log(item);
        }
    }
}
