using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingDamageNumber : PoolObject
{
    [SerializeField] private float lifeTime;
    [SerializeField] private TMP_Text text;
    private float _damageAmount;

    [SerializeField]
    float OffsetX, OffsetY, ShakeAmount;

    public void Init(int damageAmount, Vector2 position)
    {
//        Debug.Log(position);
        text.rectTransform.localPosition = position;
        text.alpha = 1;
        _damageAmount = damageAmount;
        text.text = _damageAmount.ToString();
        transform.rotation = Quaternion.identity;
        transform.DOScale(new Vector3(2, 2, 2), .5f);
     //   text.rectTransform.DOShakeAnchorPos(0.3f, ShakeAmount);
     //   text.rectTransform.DOAnchorPosX(text.rectTransform.anchoredPosition.x + (Random.value - 0.5f) * OffsetX, lifeTime).SetEase(Ease.OutCirc);
     //   text.rectTransform.DOAnchorPosY(text.rectTransform.anchoredPosition.y + OffsetY, lifeTime).SetEase(Ease.OutCubic);
        text.DOFade(0, lifeTime).OnComplete(() => PoolManager.Despawn(this));
    }

    public void Init(string message, Vector2 position)
    {
        text.rectTransform.localPosition = position;
        text.alpha = 1;
        text.text = message;
        text.DOFade(0, lifeTime).OnComplete(() => PoolManager.Despawn(this));
    }

}
