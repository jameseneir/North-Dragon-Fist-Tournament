using UnityEngine.SceneManagement;
using UnityEngine;

public class ReloadScene : MonoBehaviour
{
    [SerializeField]
    int currentScene;
    public void ReloadingScene()
    {
        SceneManager.LoadScene(currentScene);
    }
}
