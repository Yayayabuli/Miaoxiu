using UnityEngine;
using UnityEngine.SceneManagement; 

public class Menu : MonoBehaviour
{
   
    public void StartGame()
    {
        
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("No more scenes to load!");
        }
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}