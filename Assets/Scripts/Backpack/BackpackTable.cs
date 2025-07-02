using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptbleObject/BackpackTable", fileName = "BackpackTable")]
public class BackpackTable : ScriptableObject
{
    public List<BackpackTableItem> items = new();
}

public enum ItemType
{
    Prop = 1,
    Material = 2,
    Fragment = 3
}

[System.Serializable]
public class BackpackTableItem
{
    public uint id;
    public ItemType type;
    public uint quality;
    public string icon;
}
