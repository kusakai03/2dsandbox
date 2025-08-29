using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
    public Light2D torchLight;
    private void Start()
    {
        CheckTime();
        InvokeRepeating(nameof(CheckTime), 0f, 1f);
    }
    private void CheckTime()
    {
        if (WorldManager.ins.hour >= 18 || WorldManager.ins.hour < 5)
            torchLight.enabled = true;
        else
            torchLight.enabled = false;
    }
}
