using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    [SerializeField]
    CanvasGroup cg;

    [SerializeField]
    TextMeshProUGUI score;
    [SerializeField]
    TextMeshProUGUI highscore;


    private void Start()
    {
      
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.alpha = 0;
    }


    public void Open(int score, int hightScore)
    {

        this.score.text = $"Your Score: {score}";
        this.highscore.text = $"Previous Highscore: {hightScore}";
        OnBeforeShow();
    }

    void OnBeforeShow()
    {
       
        cg.alpha = 0;
        DOVirtual.Float(1, 0, 1f, (x) => Time.timeScale = x).SetUpdate(true).OnComplete(OnShowAnimate);

    }


    void OnShowAnimate()
    {
        cg.DOFade(1, 0.5f).SetUpdate(true);

        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void Restart()
    {

        Time.timeScale = 1;
        GameManager.Instance.ResetCurrentScene();


    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.LoadNewScene("MainMenuScene");

    }

    void Hide()
    {
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => DOVirtual.Float(0, 1, 0.5f, (x) => Time.timeScale = x).SetUpdate(true));
    }
}
