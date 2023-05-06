using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class UpgradeChoiceView : MonoBehaviour
{
    Upgrade upgrade;
    [SerializeField]
    Image icon;

    [SerializeField]
    TMP_Text label;

    public delegate void OnClickUpgradeSignature(Upgrade upgrade);
    public OnClickUpgradeSignature OnClickUpgrade;


    public void Init(Upgrade upgrade)
    {
        this.upgrade = upgrade;
        icon.sprite = upgrade.sprite;
        label.text = upgrade.Description;
    }

    public void OnClick()
    {
        OnClickUpgrade.Invoke(upgrade);
    }
}
