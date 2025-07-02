using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class VirtualScrollView : MonoBehaviour
{
    [SerializeField] private BackpackPanel backpackPanel;
    [SerializeField] private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;
    private GridLayoutGroup gridLayoutGroup;

    [SerializeField] private GameObject backpackItemPrefab;
    private ObjectPool<GameObject> itemPool;

    private int originalPaddingTop;
    private int maxActualItemCount;

    private int startIndex;
    private int endIndex;
    private int selectedIndex = -1;

    private float RowHeight { get
        {
            return gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y;
        }
    }

    private int ColumnCount { get
        {
            return gridLayoutGroup.constraintCount;
        } 
    }

    private List<BackpackItem> contentItems;

    private void Awake()
    {
        viewport = scrollRect.viewport;
        content = scrollRect.content;
        gridLayoutGroup = content.GetComponent<GridLayoutGroup>();

        originalPaddingTop = gridLayoutGroup.padding.top;
        maxActualItemCount = GetMaxVirtualItemCount();

        itemPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var go = Instantiate(backpackItemPrefab);
                return go;
            },
            actionOnGet: (go) =>
            {
                go.SetActive(true);
            },
            actionOnRelease: (go) =>
            {
                GameObject gcRoot = GameObject.Find("GCRoot");
                if (gcRoot == null)
                {
                    gcRoot = new GameObject("GCRoot");
                }
                go.GetComponent<BackpackCell>().OnCellDeselected();
                go.transform.SetParent(gcRoot.transform, false);
                // go.SetActive(false);
            },
            collectionCheck: false,
            defaultCapacity: maxActualItemCount,
            maxSize: maxActualItemCount * 2
        );

        startIndex = 0;
        endIndex = content.childCount;
        scrollRect.onValueChanged.AddListener(OnValueChanged);

        // Do some test
        // SetContentItemsTest();
    }

    private int GetMaxVirtualItemCount()
    {
        int rowCount = (int)(viewport.rect.height / RowHeight) + 1;
        int itemCount = (rowCount + 5) * gridLayoutGroup.constraintCount;
        return itemCount;
    }

    public void OnValueChanged(Vector2 value)
    {
        UpdateContentView();
    }

    public void UpdateContentView()
    {
        float overflowHeight = -originalPaddingTop + content.anchoredPosition.y;
        int overflowRowCount = (int)(overflowHeight / RowHeight);
        overflowRowCount = Mathf.Max(0, overflowRowCount - 2);
        int overflowItemCount = overflowRowCount * ColumnCount;
        int nextStartIndex = overflowItemCount;
        int nextEndIndex = Mathf.Min(nextStartIndex + maxActualItemCount, contentItems.Count);

        if (nextStartIndex > startIndex || nextEndIndex > endIndex)
        {
            // Scroll down with bigger `itemCount`
            while (content.childCount > 0 && nextStartIndex > startIndex)
            {
                itemPool.Release(content.GetChild(0).gameObject);
                startIndex++;
            }
            if (nextStartIndex > startIndex)
            {
                startIndex = nextStartIndex;
                endIndex = nextStartIndex;
            }
            while(nextEndIndex > endIndex)
            {
                // GameObject item = Instantiate(backpackItemPrefab, content);
                GameObject item = itemPool.Get();
                item.transform.SetParent(content.transform, false);
                item.name = $"Item[{endIndex}]";

                BackpackCell cell = item.GetComponent<BackpackCell>();
                cell.SetBackpackPanel(backpackPanel);
                cell.Refresh(contentItems[endIndex], endIndex);
                endIndex++;
                if (cell.index == selectedIndex)
                {
                    cell.OnCellSelected();
                }
            }
        }else if(nextStartIndex < startIndex || nextEndIndex < endIndex)
        {
            // Scroll up with smaller `itemCount`
            int removeIndex = content.childCount - 1;
            while(content.childCount > 0 && nextEndIndex < endIndex)
            {
                itemPool.Release(content.GetChild(removeIndex--).gameObject);
                endIndex--;
            }
            if (nextEndIndex < endIndex)
            {
                endIndex = nextEndIndex;
                startIndex = nextEndIndex;
            }
            while (nextStartIndex < startIndex)
            {
                // GameObject item = Instantiate(backpackItemPrefab, content);
                GameObject item = itemPool.Get();
                item.transform.SetParent(content.transform, false);
                item.transform.SetSiblingIndex(0);
                item.name = $"Item[{--startIndex}]";

                BackpackCell cell = item.GetComponent<BackpackCell>();
                cell.SetBackpackPanel(backpackPanel);
                cell.Refresh(contentItems[startIndex], startIndex);
                if (cell.index == selectedIndex)
                {
                    cell.OnCellSelected();
                }
            }
        }

        float extraPaddingHeight = overflowRowCount * RowHeight;
        gridLayoutGroup.padding.top = originalPaddingTop + (int)extraPaddingHeight;
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    public void UpdateContentData()
    {
        for(int i = 0, j = startIndex; i < content.childCount; i++, j++)
        {
            content.GetChild(i).GetComponent<BackpackCell>().Refresh(contentItems[j], j);
        }
    }

    public void ReturnTop()
    {
        scrollRect.verticalScrollbar.value = 1.0f;
    }

    public void SetSelectedIndex(int index)
    {
        // Deselect if necessary
        if (selectedIndex >= 0)
        {
            int actualIndex = selectedIndex - startIndex;
            if (actualIndex >= 0 && actualIndex < content.childCount)
            {
                BackpackCell cell = content.GetChild(actualIndex).GetComponent<BackpackCell>();
                cell.OnCellDeselected();
            }
        }

        // Set selection
        selectedIndex = index;
        if (selectedIndex >= 0)
        {
            int actualIndex = selectedIndex - startIndex;
            if (actualIndex >= 0 && actualIndex < content.childCount)
            {
                BackpackCell cell = content.GetChild(actualIndex).GetComponent<BackpackCell>();
                cell.OnCellSelected();
            }
        }
    }

    public void SetContentItems(List<BackpackItem> items)
    {
        contentItems = items;

        // Set content height
        int totalRowCount = Math.DivRem(contentItems.Count, ColumnCount, out int rest);
        totalRowCount += rest > 0 ? 1 : 0;
        Vector2 sizeDelta = content.sizeDelta;
        sizeDelta.y = totalRowCount * RowHeight;
        content.sizeDelta = sizeDelta;
        UpdateContentView();
        UpdateContentData();
    }

    public void SetContentItemsTest()
    {
        SetContentItems(BackpackManager.Instance.GetSortedBackpackLocalData(ItemType.Prop));
        ReturnTop();
    }
}
