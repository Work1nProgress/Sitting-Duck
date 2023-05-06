using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XpBar : MonoBehaviour
{
    [SerializeField]
    Image bar;

    [SerializeField]
    Sprite[] barSprites;

    [SerializeField]
    TextMeshProUGUI text;
    private void Awake()
    {
        GameManager.Instance.OnSceneLoaded += Init;
    }

    private void Init()
    {
        GameManager.Instance.OnSceneLoaded -= Init;


        ControllerGame.Instance.PlayerEntity.OnXPChanged += OnXpChanged;
        ControllerGame.Instance.PlayerEntity.OnLevelUpSuccess += OnLevelChanged;



    }

    void OnXpChanged(int current, int max)
    {
        text.text = $"xp: {current}/{max}";
//        Debug.Log($"{(float)current / max} {Mathf.Min((float)current / max, 1f)} {Mathf.Min((float)current / max, 1f) * (barSprites.Length - 1)} {(int)(Mathf.Min((float)current / max, 1f) * (barSprites.Length - 1))}");
        bar.sprite = barSprites[(int)(Mathf.Min((float)current / max, 1f) * (barSprites.Length - 1))];
    }

    void OnLevelChanged(int currentLevel)
    {
        OnXpChanged(ControllerGame.Instance.PlayerEntity.GetExperienceValue, ControllerGame.Instance.PlayerEntity.GetMaxExperienceValue);
    }
}
