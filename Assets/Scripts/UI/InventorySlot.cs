using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;
    public Image icon;
    public TextMeshProUGUI quantityText;

    [SerializeField] private Transform canvasTransform;
    private GameObject dragIcon;
    private Image dragImage;
    private GameObject player;
    public void SetMaster(int index, GameObject player)
    {
        slotIndex = index;
        this.player = player;
    }

    public void SetItem(Sprite sprite, int quantity)
    {
        icon.sprite = sprite;
        icon.enabled = true;
        quantityText.text = quantity > 1 ? quantity.ToString() : "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (icon.sprite == null) return;

        // tạo drag icon
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvasTransform, false);
        dragIcon.transform.SetAsLastSibling(); // trên cùng
        dragImage = dragIcon.AddComponent<Image>();
        dragImage.sprite = icon.sprite;
        dragImage.raycastTarget = false; // không chặn raycast
        var cg = dragIcon.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var sourceSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        if (sourceSlot != null && sourceSlot != this)
        {
            player.GetComponent<PlayerData>().SwitchItemSlot(sourceSlot.slotIndex, slotIndex);
        }
    }
}
