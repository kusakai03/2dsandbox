using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ItemCatalog : ScriptableObject
{
    public List<ItemInfo> items;
}
[Serializable]
public class ItemInfo
{
    public string iID;
    public string iName;
    public Sprite iSpr;
    public int stackable; //0 hoac 1
}
