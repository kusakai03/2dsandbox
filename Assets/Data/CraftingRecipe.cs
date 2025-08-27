using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    public List<Craft> crafts;
}
[Serializable]
public class Craft
{
    public string productID; //ID san pham che tao
    public int craftStrength; //Suc manh che tao (ban che tao)
    public List<CraftingMaterial> materials; //Nguyen lieu che tao
    public float productAmount; //So luong san pham
}
[Serializable]
public class CraftingMaterial
{
    public string iID;
    public int amount;
}
