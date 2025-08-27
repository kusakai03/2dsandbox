using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void Start()
    {
        Vector2Int cell = Vector2Int.FloorToInt(transform.position);
        GridManager.ins.SetWalkable(cell, false);
    }

    void OnDestroy()
    {
        Vector2Int cell = Vector2Int.FloorToInt(transform.position);
        GridManager.ins.SetWalkable(cell, true);
    }
}
