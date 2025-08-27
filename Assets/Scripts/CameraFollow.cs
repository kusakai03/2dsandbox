using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Camera Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset;

    [Header("Mouse Influence")]
    public float mouseInfluence = 4f;
    public float edgeSize = 100f;

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;

        Vector3 mouseScreen = Input.mousePosition;

        float screenW = Screen.width;
        float screenH = Screen.height;

        Vector3 mouseOffset = Vector3.zero;

        if (mouseScreen.x <= edgeSize)
            mouseOffset.x = -1;
        else if (mouseScreen.x >= screenW - edgeSize)
            mouseOffset.x = 1;

        if (mouseScreen.y <= edgeSize)
            mouseOffset.y = -1;
        else if (mouseScreen.y >= screenH - edgeSize)
            mouseOffset.y = 1;

        if (mouseOffset != Vector3.zero)
        {
            mouseOffset = mouseOffset.normalized * mouseInfluence;
            desiredPosition += mouseOffset;
        }

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
