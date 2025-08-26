using UnityEngine;
using System.IO;

[ExecuteAlways] // Cho phép xem ngay trong Editor khi chỉnh thông số
public class Perlin : MonoBehaviour
{
    [Header("Kích thước texture")]
    public int width = 256;
    public int height = 256;

    [Header("Noise cơ bản")]
    [Min(0.0001f)] public float scale = 50f;
    public int seed = 0;
    public Vector2 offset; // Kéo trôi noise

    [Header("Fractal (FBM)")]
    [Range(1, 10)] public int octaves = 4;
    [Range(0f, 1f)] public float persistence = 0.5f; // Giảm biên độ
    [Min(1f)] public float lacunarity = 2f;          // Tăng tần số

    [Header("Hiển thị")]
    public bool useGradient = false;
    public Gradient gradient; // Kéo thả để tô màu (nếu không dùng thì là grayscale)
    public FilterMode filterMode = FilterMode.Bilinear;
    public TextureWrapMode wrapMode = TextureWrapMode.Repeat; // Hữu ích nếu muốn tile

    [Header("Gán ra đâu?")]
    public SpriteRenderer targetSpriteRenderer; // Kéo component vào đây nếu muốn xem qua SpriteRenderer
    public bool regenerateEveryChange = true;   // Auto regen khi đổi Inspector

    private Texture2D _tex;
    private Color[] _colors;

    void OnEnable()
    {
        GenerateAndApply();
    }

    void OnValidate()
    {
        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);
        scale = Mathf.Max(0.0001f, scale);
        lacunarity = Mathf.Max(1f, lacunarity);
        if (regenerateEveryChange && enabled)
            GenerateAndApply();
    }

    /// <summary>
    /// Bấm phím Space trong Play Mode để random seed nhanh
    /// </summary>
    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            GenerateAndApply();
        }
#endif
    }

    [ContextMenu("Generate & Apply")]
    public void GenerateAndApply()
    {
        float[,] noiseMap = GenerateNoiseMap(width, height, scale, octaves, persistence, lacunarity, seed, offset);

        if (_tex == null || _tex.width != width || _tex.height != height)
        {
            _tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            _tex.filterMode = filterMode;
            _tex.wrapMode = wrapMode;
            _colors = new Color[width * height];
        }

        // Đổ màu
        int i = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++, i++)
            {
                float v = noiseMap[x, y]; // 0..1
                _colors[i] = useGradient ? gradient.Evaluate(v) : new Color(v, v, v, 1f);
            }

        _tex.SetPixels(_colors);
        _tex.Apply(false);

        // Gán ra SpriteRenderer nếu có
        if (targetSpriteRenderer != null)
        {
            // PPU  = 100 cho dễ nhìn; chỉnh tùy ý
            var sprite = Sprite.Create(_tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);
            targetSpriteRenderer.sprite = sprite;
        }
    }

    /// <summary>
    /// Tạo noise dạng FBM (nhiều octave)
    /// </summary>
    public static float[,] GenerateNoiseMap(
        int width, int height, float scale,
        int octaves, float persistence, float lacunarity,
        int seed, Vector2 offset)
    {
        float[,] map = new float[width, height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            // Offset lớn để seed tạo pattern khác biệt giữa các octave
            float ox = prng.Next(-100000, 100000) + offset.x;
            float oy = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(ox, oy);
        }

        float maxNoise = float.MinValue;
        float minNoise = float.MaxValue;

        // Tránh lặp lại phép chia
        float invScale = 1f / scale;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int o = 0; o < octaves; o++)
                {
                    float sampleX = (x * invScale * frequency) + octaveOffsets[o].x * invScale;
                    float sampleY = (y * invScale * frequency) + octaveOffsets[o].y * invScale;

                    // Mathf.PerlinNoise trả về 0..1; chuyển sang -1..1 để cộng chồng tự nhiên hơn
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoise) maxNoise = noiseHeight;
                if (noiseHeight < minNoise) minNoise = noiseHeight;

                map[x, y] = noiseHeight;
            }
        }

        // Chuẩn hóa map về 0..1
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                map[x, y] = Mathf.InverseLerp(minNoise, maxNoise, map[x, y]);
            }

        return map;
    }

    [ContextMenu("Save PNG to persistentDataPath")]
    public void SavePNG()
    {
        if (_tex == null) GenerateAndApply();
        byte[] png = _tex.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, $"perlin_{width}x{height}_seed{seed}.png");
        File.WriteAllBytes(path, png);
        Debug.Log("Saved to: " + path);
    }
}
