using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackpackSwitchButton : MonoBehaviour
{
    private BackpackPanel backpackPanel;

    private Image buttonImage;
    [SerializeField] private Color buttonNormalColor;
    [SerializeField] private Color buttonSelectedColor;

    private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private Color textNormalColor;
    [SerializeField] private Color textSelectedColor;

    [SerializeField] private ItemType tabItemType;
    public ItemType TabItemType => tabItemType;

    private void Awake()
    {
        backpackPanel = transform.parent.parent.parent.GetComponent<BackpackPanel>();

        buttonImage = GetComponent<Image>();
        buttonImage.color = buttonNormalColor;

        textMeshProUGUI = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.color = textNormalColor;
    }

    public void OnClick()
    {
        backpackPanel.OnTabChanged(tabItemType);
    }

    public void ChangeToNormal()
    {
        buttonImage.color = buttonNormalColor;
        textMeshProUGUI.color = textNormalColor;
        transform.localScale = Vector3.one;
    }

    public void ChangeToSelected()
    {
        buttonImage.color = buttonSelectedColor;
        textMeshProUGUI.color = textSelectedColor;
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }
}
