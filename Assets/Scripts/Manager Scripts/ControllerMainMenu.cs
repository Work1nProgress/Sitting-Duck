using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMainMenu : ControllerLocal
{

    string mainScene = "GameScene";

    [SerializeField]
    TMPro.TextMeshProUGUI scoreText;
    public override void Init()
    {
        var score = PlayerPrefs.GetInt("kills_high", 0);

        if (score > 0) {
            scoreText.text = $"High Score: {score}";
        }
        base.Init();   
    }


    public void OnStartGameClick()
    {
        GameManager.Instance.LoadNewScene(mainScene);

    }
}
