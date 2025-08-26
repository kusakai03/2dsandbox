using System.Linq;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database db;
    [SerializeField] private ItemCatalog itemCatalog;
    private void Awake()
    {
        if (db == null)
        {
            db = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    public ItemInfo GetGameItemByid(string id) { return itemCatalog.items.Find(a => a.iID == id); }
    public bool IsItemInCatalog(string id) { return itemCatalog.items.Any(a => a.iID == id); }
}
