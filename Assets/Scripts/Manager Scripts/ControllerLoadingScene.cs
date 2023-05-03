using UnityEngine;

public class ControllerLoadingScene : MonoBehaviour
{


    string mainMenuScene = "MainMenuScene";

    void Start()
    {

        //TODO load save file

        GameManager.Instance.LoadNewScene(mainMenuScene);
    }
}
