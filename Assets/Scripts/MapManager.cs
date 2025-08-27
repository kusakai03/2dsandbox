using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager ins;
    [Header("Tilemap")]
    public Tilemap groundTilemap;

    [Header("Map Settings")]
    public int chunkSize = 32;
    public int viewDistance = 2;

    [Header("Noise Settings")]
    public float scale = 50f;
    public float tempScale = 50f;
    public int seed = 12345;

    [Header("Biomes")]
    public Biome[] biomes;

    [Header("Player")]
    public Transform player;

    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private System.Random prng;
    private float offsetX, offsetY, tempOffsetX, tempOffsetY;

    [Header("Tile Properties")]
    public TileDat[] tileDatas;

    private Dictionary<TileBase, float> tileSpeedLookup;

    void Awake()
    {
        if (ins == null) ins = this;
        tileSpeedLookup = new Dictionary<TileBase, float>();
        foreach (var td in tileDatas)
        {
            if (td != null && td.tile != null)
            {
                tileSpeedLookup[td.tile] = td.speedMultiplier;
            }
        }
    }

    public float GetSpeedMultiplier(Vector3 worldPos)
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(worldPos);
        TileBase tile = groundTilemap.GetTile(cellPos);
        if (tile != null && tileSpeedLookup.TryGetValue(tile, out float mult))
        {
            return mult;
        }
        return 1f;
    }
    void Start()
    {
        prng = new System.Random(seed);
        offsetX = prng.Next(-10000, 10000);
        offsetY = prng.Next(-10000, 10000);
        tempOffsetX = prng.Next(-10000, 10000);
        tempOffsetY = prng.Next(-10000, 10000);
    }

    void Update()
    {
        UpdateChunks();
    }

    void UpdateChunks()
    {
        if (player == null) return;
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.y / chunkSize)
        );

        HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();
        for (int y = -viewDistance; y <= viewDistance; y++)
        {
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);
                activeChunks.Add(chunkCoord);

                if (!chunks.ContainsKey(chunkCoord))
                {
                    Chunk newChunk = new Chunk(chunkCoord, chunkSize, scale, tempScale,
                                               offsetX, offsetY, tempOffsetX, tempOffsetY,
                                               biomes, groundTilemap);
                    chunks.Add(chunkCoord, newChunk);
                    newChunk.Draw();
                }
            }
        }
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var kvp in chunks)
        {
            if (!activeChunks.Contains(kvp.Key))
            {
                kvp.Value.Clear();
                chunksToRemove.Add(kvp.Key);
            }
        }

        foreach (var coord in chunksToRemove)
        {
            chunks.Remove(coord);
        }
    }
}
public class Chunk
{
    public Vector2Int coord;
    public int size;
    private float scale, tempScale;
    private float offsetX, offsetY, tempOffsetX, tempOffsetY;
    private Biome[] biomes;
    private Tilemap tilemap;
    private Transform objectParent;

    public Chunk(Vector2Int coord, int size, float scale, float tempScale,
                 float offsetX, float offsetY, float tempOffsetX, float tempOffsetY,
                 Biome[] biomes, Tilemap tilemap)
    {
        this.coord = coord;
        this.size = size;
        this.scale = scale;
        this.tempScale = tempScale;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.tempOffsetX = tempOffsetX;
        this.tempOffsetY = tempOffsetY;
        this.biomes = biomes;
        this.tilemap = tilemap;
        objectParent = new GameObject($"Chunk_{coord.x}_{coord.y}").transform;
    }

    public void Draw()
    {
        int startX = coord.x * size;
        int startY = coord.y * size;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int worldX = startX + x;
                int worldY = startY + y;

                float sampleX = (worldX + offsetX) / scale;
                float sampleY = (worldY + offsetY) / scale;
                float heightValue = Mathf.PerlinNoise(sampleX, sampleY);

                float tempSampleX = (worldX + tempOffsetX) / tempScale;
                float tempSampleY = (worldY + tempOffsetY) / tempScale;
                float tempValue = Mathf.PerlinNoise(tempSampleX, tempSampleY);

                foreach (var biome in biomes)
                {
                    if (biome.InRange(heightValue, tempValue))
                    {
                        tilemap.SetTile(new Vector3Int(worldX, worldY, 0), biome.tile);
                        GridManager.ins.SetWalkable(new Vector2Int(worldX, worldY), biome.type != BiomeType.Ocean);
                        if (biome.spawnableObjects != null && biome.spawnableObjects.Length > 0)
                        {
                            if (Random.value < biome.spawnChance)
                            {
                                GameObject prefab = biome.spawnableObjects[Random.Range(0, biome.spawnableObjects.Length)];
                                Vector3 pos = new Vector3(worldX + 0.5f, worldY + 0.5f, 0); // giữa ô tile
                                GameObject obj = Object.Instantiate(prefab, pos, Quaternion.identity, objectParent);
                            }
                        }

                        break;
                    }
                }
            }
        }
    }

    public void Clear()
    {
        int startX = coord.x * size;
        int startY = coord.y * size;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int worldX = startX + x;
                int worldY = startY + y;
                tilemap.SetTile(new Vector3Int(worldX, worldY, 0), null);
            }
        }
        if (objectParent != null)
        {
            Object.Destroy(objectParent.gameObject);
        }
    }
}

public enum BiomeType
{
    Ocean,
    River,
    Beach,
    Grassland,
    Desert,
    Snow,
    Mountain
}
[System.Serializable]
public class Biome
{
    public BiomeType type;
    public RuleTile tile;
    public Color color;

    [Header("Range")]
    [Range(0f, 1f)] public float minHeight;
    [Range(0f, 1f)] public float maxHeight;
    [Range(0f, 1f)] public float minTemperature;
    [Range(0f, 1f)] public float maxTemperature;

    [Header("Objects")]
    public GameObject[] spawnableObjects; // ví dụ cây, đá...
    [Range(0f, 1f)] public float spawnChance = 0.1f;

    public bool InRange(float height, float temperature)
    {
        return height >= minHeight && height <= maxHeight &&
               temperature >= minTemperature && temperature <= maxTemperature;
    }
}
[System.Serializable]
public class TileDat
{
    public RuleTile tile;
    public float speedMultiplier = 1f; // 1 = bình thường, <1 = chậm, >1 = nhanh
}