using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager ins;

    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    void Awake()
    {
        ins = this;
    }

    public void SetWalkable(Vector2Int pos, bool walkable)
    {
        if (!nodes.ContainsKey(pos))
            nodes[pos] = new Node(pos, walkable);
        else
            nodes[pos].walkable = walkable;
    }

    public Node GetNode(Vector2Int pos)
    {
        nodes.TryGetValue(pos, out Node node);
        return node;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        Vector2Int[] dirs = {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
        };
        foreach (var dir in dirs)
        {
            Vector2Int checkPos = node.pos + dir;
            if (nodes.TryGetValue(checkPos, out Node neighbour))
            {
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }
}

public class Node
{
    public Vector2Int pos;
    public bool walkable;
    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost => gCost + hCost;

    public Node(Vector2Int pos, bool walkable)
    {
        this.pos = pos;
        this.walkable = walkable;
    }
}
