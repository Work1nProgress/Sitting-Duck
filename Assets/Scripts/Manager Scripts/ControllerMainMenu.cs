using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMainMenu : ControllerLocal
{

    string mainScene = "UrbiEtUrbiTestScene";//"GameScene";
    public override void Init()
    {



        base.Init();   
    }


    public void OnStartGameClick()
    {
        GameManager.Instance.LoadNewScene(mainScene);

    }
}
