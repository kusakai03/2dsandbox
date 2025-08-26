using UnityEngine;

public class EntitySortOrder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr.sortingOrder = -(int)transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
