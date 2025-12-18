using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public int sceneIndex;
    public void LoadSceneOnClick()
    {
        SceneManager.LoadScene(sceneIndex);
    }
    
    public void OnButtonClick()
    {
        LoadSceneOnClick();
    }
}