using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    string nextSceneName;
    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
