using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackpackCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BackpackPanel backpackPanel;

    private Transform UISelect;
    private Transform UIIconImage;
    private Transform UIIDText;
    private Transform UIQualityText;
    private Transform UINumberText;

    private static readonly Dictionary<uint, string> qualityMap = new()
    {
        {1, "普通" },
        {2, "精良" },
        {3, "优秀" },
        {4, "史诗" },
        {5, "传说" }
    };

    private static Sprite loadingSprite;

    private static string loadingSpritePath = "Images/MyIcons/Loading2";

    private BackpackItem item;

    public int index;

    private void Awake()
    {
        InitUIReferences();
    }

    private void InitUIReferences()
    {
        UISelect = transform.Find("Select");
        UIIconImage = transform.Find("Icon/IconImage");
        UIIDText = transform.Find("Icon/IDText");
        UIQualityText = transform.Find("Icon/QualityText");
        UINumberText = transform.Find("Icon/NumberText");

        if (loadingSprite == null)
        {
            loadingSprite = Resources.Load<Sprite>(loadingSpritePath);
        }
    }

    public void SetBackpackPanel(BackpackPanel backpackPanel)
    {
        this.backpackPanel = backpackPanel;
    }

    public void Refresh(BackpackItem newItem, int index)
    {
        this.index = index;

        UIIDText.GetComponent<TextMeshProUGUI>().text = $"ID: {newItem.uid}";
        UIQualityText.GetComponent<TextMeshProUGUI>().text = $"{qualityMap[newItem.quality]}";
        UINumberText.GetComponent<TextMeshProUGUI>().text = $"{newItem.amount}";

        // Load image resource async
        if (item == null || item.icon != newItem.icon)
        {
            item = newItem;
            UIIconImage.GetComponent<Image>().sprite = loadingSprite;
            UIIconImage.GetComponent<Animator>().SetBool("isLoading", true);
            StartCoroutine(LoadItemSpriteAsync(newItem.icon));
        }
        else
        {
            item = newItem;
        }
    }

    private IEnumerator LoadItemSpriteAsync(string spritePath)
    {
        ResourceRequest iconResourceRequest = Resources.LoadAsync<Sprite>(spritePath);
        yield return iconResourceRequest;

        if (item == null || spritePath != item.icon)
        {
            Resources.UnloadAsset(iconResourceRequest.asset);
        }
        else
        {
            UIIconImage.GetComponent<Animator>().SetBool("isLoading", false);
            UIIconImage.GetComponent<Image>().sprite = (Sprite)iconResourceRequest.asset;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (backpackPanel != null)
        {
            backpackPanel.OnCellSelected(this);
        }
    }

    public void OnCellSelected()
    {
        UISelect.gameObject.SetActive(true);
    }

    public void OnCellDeselected()
    {
        UISelect.gameObject.SetActive(false);
    }
}
