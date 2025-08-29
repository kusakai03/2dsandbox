using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager ins;
    private void Awake()
    {
        if (ins == null)
        {
            DontDestroyOnLoad(gameObject);
            ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void GoToScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }
}
