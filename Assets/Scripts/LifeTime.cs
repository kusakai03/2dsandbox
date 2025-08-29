using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private bool destroyOrDisable;
    private void OnEnable()
    {
        Invoke(nameof(DestroyObject), lifeTime);
    }
    private void OnDisable()
    {
        CancelInvoke(nameof(DestroyObject));
    }
    private void DestroyObject()
    {
        if (destroyOrDisable)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
