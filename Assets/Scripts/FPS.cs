using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI fpstxt; private float deltaTime = 0.0f;
    void Start()
    {
        InvokeRepeating(nameof(UpdateFps), 0, 1);
    }
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    private void UpdateFps()
    {
        float fps = 1.0f / deltaTime;
        fpstxt.text = $"FPS: {Mathf.Ceil(fps)}";
    }
}
