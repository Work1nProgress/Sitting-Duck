using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingDamageNumber : PoolObject
{
    [SerializeField] private float lifeTime;
    [SerializeField] private TMP_Text text;
    private float _damageAmount;

    [SerializeField]
    float OffsetX, OffsetY, ShakeAmount, StartOffsetXRange, StartOffsetY;

    float startOffsetX;

    Vector3 worldPos;

    public void Init(int damageAmount, Vector3 worldPosition)
    {
        worldPos = worldPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ControllerGame.Instance.MainUIContainer, Camera.main.WorldToScreenPoint(worldPos), null, out var point);

        var currentPos = ControllerGame.Instance.MainUIContainer.InverseTransformVector(point);
        startOffsetX = Random.Range(-StartOffsetXRange, StartOffsetXRange);
        //        Debug.Log(position);
        text.rectTransform.localPosition = new Vector3(currentPos.x + startOffsetX, currentPos.y + StartOffsetY, -1);
        text.alpha = 1;
        _damageAmount = damageAmount;
        text.text = _damageAmount.ToString();
        text.fontSize = 18f;
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
        transform.DOScale(new Vector3(2, 2, 2), .5f);
        text.DOFade(0, lifeTime).OnComplete(() => PoolManager.Despawn(this));
        DOVirtual.Float(0, 1, lifeTime, Animate);
    }

    void Animate(float value)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ControllerGame.Instance.MainUIContainer, Camera.main.WorldToScreenPoint(worldPos), null, out var point);

        var currentPos = ControllerGame.Instance.MainUIContainer.InverseTransformVector(point);

        text.rectTransform.localPosition = new Vector3(currentPos.x + startOffsetX, currentPos.y + StartOffsetY + DOVirtual.EasedValue(0, StartOffsetY, value,Ease.OutCubic), -1);

    }

    public void Init(string message)
    {
        text.fontSize = 36;
        transform.SetAsFirstSibling();
        text.alpha = 1;
        text.text = message;
        text.DOFade(0, lifeTime).OnComplete(() => PoolManager.Despawn(this));
    }

}
