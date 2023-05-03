using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingleton<GameManager>
{
    private string _currentScene;

 

    protected override void Awake()
    {
        base.Awake();
        _currentScene = SceneManager.GetActiveScene().name;    
    }

  

    public void LoadNewScene(string sceneName)
    {
        SceneManager.sceneLoaded += OnAfterSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    public void ResetCurrentScene()
    {
        Debug.Log($"load scene");
        LoadNewScene(_currentScene);
    }

    void OnAfterSceneLoaded(Scene s, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= OnAfterSceneLoaded;
        _currentScene = SceneManager.GetActiveScene().name;
        var controllerLocal = FindFirstObjectByType<ControllerLocal>();
        if (controllerLocal != null)
        {
            controllerLocal.Init();
        }
       


    }

   

    
}
