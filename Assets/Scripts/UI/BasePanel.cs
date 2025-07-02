using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public bool ShouldDestroyAfterClosed = false;

    public virtual void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    public virtual void ClosePanel() 
    {
        gameObject.SetActive(false);
        if (ShouldDestroyAfterClosed)
        {
            Destroy(gameObject);
        }
    }
}
