using UnityEngine;

public class PlayerPickupRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Drop")
        {
            col.GetComponent<DropMagnet>().SetTarget(GetComponentInParent<PlayerData>().transform);
        }
    }
}
