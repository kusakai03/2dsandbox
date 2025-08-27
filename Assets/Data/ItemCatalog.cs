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
    public string type;
    public Weapon weaponData; //Neu item la weapon
    public int stackable; //0 hoac 1
}
[Serializable]
public class Weapon
{
    public string wType;
    public float wATK;
    public float wAttackSpeed;
    public float wMaxDurability;
}