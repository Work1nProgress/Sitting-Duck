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

    public void Init(int damageAmount)
    {
        text.alpha = 1;
        _damageAmount = damageAmount;
        text.text = _damageAmount.ToString();
        gameObject.transform.SetParent(null, true);
        transform.rotation = Quaternion.identity;
        transform.DOScale(new Vector3(2, 2, 2), .5f);
        text.rectTransform.DOShakeAnchorPos(0.3f, ShakeAmount);
        transform.DOMoveX(transform.position.x + (Random.value - 0.5f) * OffsetX, lifeTime).SetEase(Ease.OutCirc);
        transform.DOMoveY(transform.position.y + OffsetY, lifeTime).SetEase(Ease.OutCubic);
        text.DOFade(0, lifeTime).OnComplete(() => PoolManager.Despawn(this));
    }

}
