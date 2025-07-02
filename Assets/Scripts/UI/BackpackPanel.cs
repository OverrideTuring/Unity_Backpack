using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackpackPanel :  BasePanel
{
    [SerializeField] private GameObject itemPrefab;

    private VirtualScrollView virtualScrollView;

    private Dictionary<ItemType, BackpackSwitchButton> backpackSwitchButtonDict = new();

    private Dictionary<ItemType, List<BackpackItem>> sortedHeapedBackpackItemsDict = new();

    public int SelectedIndex { get; private set; }

    public ItemType ShownItemType { get; private set; }

    public void CloseBackpack()
    {
        UIManager.Instance.ClosePanel(UIConst.BackpackPanel);
    }

    private void Awake()
    {
        InitUIReferences();
    }

    private void Start()
    {
        RefreshContent();
    }

    private void InitUIReferences()
    {
        virtualScrollView = transform.Find("Center/Scroll View").GetComponent<VirtualScrollView>();

        BackpackSwitchButton propButton = transform.Find("Center/Foot/PropButton").GetComponent<BackpackSwitchButton>();
        BackpackSwitchButton materialButton = transform.Find("Center/Foot/MaterialButton").GetComponent<BackpackSwitchButton>();
        BackpackSwitchButton fragmentButton = transform.Find("Center/Foot/FragmentButton").GetComponent<BackpackSwitchButton>();
        backpackSwitchButtonDict.Add(ItemType.Prop, propButton);
        backpackSwitchButtonDict.Add(ItemType.Material, materialButton);
        backpackSwitchButtonDict.Add(ItemType.Fragment, fragmentButton);
    }

    private void RefreshContent()
    {
        OnTabChanged(ItemType.Prop);
    }

    public void OnTabChanged(ItemType type)
    {
        if (ShownItemType == type) return;

        if (backpackSwitchButtonDict.TryGetValue(ShownItemType, out BackpackSwitchButton currentButton))
        {
            currentButton.ChangeToNormal();
        }

        if (backpackSwitchButtonDict.TryGetValue(type, out BackpackSwitchButton newButton)) {
            newButton.ChangeToSelected();
        }

        SelectedIndex = -1;
        virtualScrollView.SetSelectedIndex(SelectedIndex);

        ShownItemType = type;
        DisplayItems(type);
    }

    public void DisplayItems(ItemType type)
    {
        if (sortedHeapedBackpackItemsDict.TryGetValue(type, out List<BackpackItem> sortedHeapedBackpackItems))
        {
            virtualScrollView.SetContentItems(sortedHeapedBackpackItems);
            virtualScrollView.ReturnTop();
            return;
        }

        List<BackpackItem> items = BackpackManager.Instance.GetSortedBackpackLocalData(type);
        if (items == null)
        {
            Debug.LogError($"BackpackManager returned null for ItemType: {type}");
            throw new ArgumentException($"Invalid ItemType: {type}");
        }

        List<BackpackItem> heapedItems = new();
        foreach (BackpackItem item in items)
        {
            uint remaining = item.amount;

            while (remaining > 99)
            {
                BackpackItem currentItem = new()
                {
                    uid = item.uid,
                    id = item.id,
                    type = item.type,
                    quality = item.quality,
                    icon = item.icon,
                    amount = 99
                };
                heapedItems.Add(currentItem);
                remaining -= 99;
            }
            {
                BackpackItem currentItem = new()
                {
                    uid = item.uid,
                    id = item.id,
                    type = item.type,
                    quality = item.quality,
                    icon = item.icon,
                    amount = remaining
                };
                heapedItems.Add(currentItem);
            }
        }
        sortedHeapedBackpackItemsDict.Add(type, heapedItems);
        virtualScrollView.SetContentItems(heapedItems);
        virtualScrollView.ReturnTop();
    }

    public void OnCellSelected(BackpackCell cell)
    {
        if (SelectedIndex == cell.index) return;

        virtualScrollView.SetSelectedIndex(cell.index);
    }
}
