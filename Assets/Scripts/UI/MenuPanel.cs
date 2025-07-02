using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : BasePanel
{
    public void OpenBackpack()
    {
        BasePanel panel = UIManager.Instance.OpenPanel(UIConst.BackpackPanel);
        panel.ShouldDestroyAfterClosed = true;
    }
}
