using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelUpPopup : MonoBehaviour
{


    [SerializeField]
    CanvasGroup cg;


    List<Upgrade> upgradeTypes;

    [SerializeField]
    UpgradeChoiceView[] choices;

    bool picked = true;

    private void Start()
    {
        foreach (var choiceView in choices)
        {
            choiceView.OnClickUpgrade += SelectUpgrade;
        }
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.alpha = 0;
    }


    public void Open(List<Upgrade> upgradeTypes)
    {
      
       this.upgradeTypes = upgradeTypes;
        OnBeforeShow();
    }

    void OnBeforeShow()
    {
        int i = 0;
        foreach (var choiceView in choices)
        {

            if (i < upgradeTypes.Count)
            {
                choiceView.gameObject.SetActive(true);
                choiceView.Init(upgradeTypes[i]);
            }
            else
            {
                choiceView.gameObject.SetActive(false);
            }

           
            
            i++;
        }
        cg.alpha = 0;
        Time.timeScale = 0;
        OnShowAnimate();
    }


    void OnShowAnimate()
    {
        cg.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => picked = false);

        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void SelectUpgrade(Upgrade upgrade)
    {
        if (picked)
        {
            return;
        }
        ControllerGame.Instance.Upgrade(upgrade.upgradeType, upgrade.Amount);
        picked = true;
        Hide();
    }

    void Hide() {
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => Time.timeScale = 1);
    }

 
}
