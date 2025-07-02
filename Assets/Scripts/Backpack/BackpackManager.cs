using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackManager
{
    private static BackpackManager _instance;

    public static BackpackManager Instance
    {
        get
        {
            _instance ??= new BackpackManager();
            return _instance;
        }
    }

    public List<BackpackItem> sortedBackpackItems;
    public Dictionary<ItemType, List<BackpackItem>> sortedBackpackItemsDict;

    public List<BackpackItem> GetSortedBackpackLocalData()
    {
        if (sortedBackpackItems == null)
        {
            sortedBackpackItems = BackpackLocalData.Instance.LoadBackpackData();
            BackpackItemComparer comparer = new BackpackItemComparer();
            sortedBackpackItems.Sort(comparer);

            sortedBackpackItemsDict = new Dictionary<ItemType, List<BackpackItem>>();
            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType))){
                sortedBackpackItemsDict.Add(itemType, new List<BackpackItem>());
            }

            foreach (BackpackItem item in sortedBackpackItems)
            {
                sortedBackpackItemsDict[item.type].Add(item);
            }
            foreach(List<BackpackItem> backpackItems in sortedBackpackItemsDict.Values)
            {
                backpackItems.Sort(comparer);
            }
        }
        return sortedBackpackItems;
    }

    public List<BackpackItem> GetSortedBackpackLocalData(ItemType itemType)
    {
        GetSortedBackpackLocalData();
        if (sortedBackpackItemsDict.TryGetValue(itemType, out List<BackpackItem> sortedItems)){
            return sortedItems;
        }
        return null;
    }
}

public class BackpackItemComparer : IComparer<BackpackItem>
{
    public int Compare(BackpackItem x, BackpackItem y)
    {
        // 1. quality order by desc
        int qualityComparison = y.quality.CompareTo(x.quality);
        if (qualityComparison == 0)
        {
            // 2. type order by asc
            int typeComparison = x.type.CompareTo(y.type);
            if (typeComparison == 0)
            {
                // 3. id order by asc
                int idComparison = x.uid.CompareTo(y.uid);
                return idComparison;
            }
            return typeComparison;
        }
        return qualityComparison;
    }
}