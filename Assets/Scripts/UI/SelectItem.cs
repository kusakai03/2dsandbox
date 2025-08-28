using System;
using UnityEngine;

public class SelectItem : MonoBehaviour
{
    [SerializeField] private int index;
    public Action<int> onSelectItem;
    public void OnClick()
    {
        onSelectItem?.Invoke(index);
    }
    public void SetIndex(int i)
    {
        index = i;
    }
}
