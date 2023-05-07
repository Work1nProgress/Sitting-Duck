using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EntityStats : MonoBehaviour, IEntityHealth, IExperience
{
    [SerializeField] EntityType _entityType;
    [SerializeField] private GameObject _bloodVFX;

    public bool _canHealthChange = true;

    public int MaxHealth => _maxHealth;
    [SerializeField] private int _maxHealth;

    public int Health => _health;
    private int _health;

    [SerializeField] private int _experience;
    [SerializeField] private int _startLevel;
    [SerializeField] private int _maxLevel;
    [SerializeField] private float levelUpXPCoeficient, levelUpStartXP;
  
    private int _currentLevel;
    [SerializeField] private int[] _levelupThresholds;

    

    [SerializeField]private string[] _tags;

    CountdownTimer _xpPickupTimer;

    public delegate void EnemyHealthChangeSignature(int oldHealth, int newHealth, int maxHealth);
    public event EnemyHealthChangeSignature OnHealthChanged;

    public delegate void EntityDeathSignature(EntityStats entityStats);
    public event EntityDeathSignature OnDeath;

    public delegate void EntityXpChangeSignature(int current, int max);
    public event EntityXpChangeSignature OnXPChanged;

    public delegate void EntityLevelChangeSignature(int level);
    public event EntityLevelChangeSignature OnLevelUpSuccess;
    public event EntityLevelChangeSignature OnLevelUpFail;


    BulletCollider _collider;
    private void Awake()
    {
        _currentLevel = Mathf.Clamp(_startLevel, 1, _maxLevel);
        _experience = Mathf.Abs(_experience);
        _health = _maxHealth;


        _collider = GetComponent<BulletCollider>();

        if(_entityType == EntityType.Player)
        {
            _xpPickupTimer = new CountdownTimer(0.125f, false, true);
            _xpPickupTimer.OnTimerExpired += PickupXPOrbs;
            _levelupThresholds = new int[_maxLevel];
            for (int i = 0; i < _maxLevel; i++)
            {

                _levelupThresholds[i] = (int)(levelUpStartXP + i * levelUpXPCoeficient);
            }
        }


        ControllerGame.Instance.AddEntityReference(this, _entityType);
    }

    void OnEnable()
    {
        _collider.OnBulletHitEvent += OnBulletHit;
        _collider.OnTriggerExitEvent += OnChainSawExit;
    }

    void OnDisable()
    {
        _collider.OnBulletHitEvent -= OnBulletHit;
        _collider.OnTriggerExitEvent -= OnChainSawExit;
    }

    void OnBulletHit(BulletType bulletType, bool chainsaw) {
        if (!chainsaw)
        {
            ControllerGame.Instance.OnBulletHit(this, bulletType);
        }
        else
        {
            ControllerGame.Instance.AddChainsawDamage(this);
        }
    }

    void OnChainSawExit()
    {
        ControllerGame.Instance.OnChainSawExit(this);
    }

    private void Update()
    {
        if (_xpPickupTimer != null)
            _xpPickupTimer.Update(Time.deltaTime);
    }

    public void Damage(int ammount)
    {
        if (!_canHealthChange) return;
        
        
        if (_entityType != EntityType.Player)
        {
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(ControllerGame.Instance.MainUIContainer, Camera.main.WorldToScreenPoint(transform.position), null, out var point);

            //var currentPos = ControllerGame.Instance.MainUIContainer.InverseTransformVector(point);
            var spawn = PoolManager.Spawn<FloatingDamageNumber>("FloatingDamageNumber", ControllerGame.Instance.MainUIContainer);
            spawn.Init(ammount, transform.position);
        }

        if (_bloodVFX != null)
        {
            _bloodVFX.gameObject.SetActive(true);
            _bloodVFX.GetComponent<AutoDisableObject>().StartCountdown();
        }
        
        int newHealth = _health - ammount;
        newHealth = Mathf.Clamp(newHealth, 0, _maxHealth);
        
        if (_entityType == EntityType.Player)
        {
            GameFeelManager.Instance.PlayerTookDamageShakeTheCameraFor(0.3f);
            SoundManager.Instance.Play("heartbreak");
            _canHealthChange = false;
            StartCoroutine(CanTakeAgainDamageIn(newHealth > 0 ? 1f : 1000f));
        }

        if (newHealth == 0)
        {
            if (OnDeath != null)
                OnDeath.Invoke(this);
            return;
        }

        if (OnHealthChanged != null)
            OnHealthChanged.Invoke(_health, newHealth, _maxHealth);
        _health = newHealth;
    }
    public void Heal(int ammount)
    {
        if (_canHealthChange)
        {
            int newHealth = _health + ammount;
            newHealth = Mathf.Clamp(newHealth, 0, _maxHealth);

            if (newHealth == 0)
            {
                OnDeath.Invoke(this);
                return;
            }

            if (OnHealthChanged != null)
                OnHealthChanged.Invoke(_health, newHealth, _maxHealth);
            _health = newHealth;
        }
    }

    public int GetExperienceValue => _experience;
    public int GetMaxExperienceValue => _levelupThresholds[_currentLevel - 1];
    public void ChangeExperienceValue(int change)
    {
        _experience += change;
        OnXPChanged.Invoke(_experience, _levelupThresholds[Mathf.Min(_currentLevel - 1, _levelupThresholds.Length-1)]);
        if (_currentLevel - 1 < _levelupThresholds.Length)
            if(_experience > _levelupThresholds[_currentLevel - 1])
            {
                SoundManager.Instance.Play("levelup");
                _experience -= _levelupThresholds[_currentLevel - 1];
                IncreaseLevel();
            }
    }
    public int GetLevel() { return _currentLevel; }
    public void IncreaseLevel()
    {
        if(_currentLevel + 1 <= _maxLevel)
        {
            _currentLevel++;
            if (OnLevelUpSuccess != null)
            OnLevelUpSuccess.Invoke(_currentLevel);
        }
        else if (OnLevelUpFail != null)
                OnLevelUpFail.Invoke(_currentLevel);
    }

    public void PickupXPOrbs()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (Collider2D collider in colliders)
        {


            var pickup = collider.GetComponent<Pickup>();
            switch (pickup)
            {
                case XPSource xp:
                    SoundManager.Instance.Play("xporb");
                    ChangeExperienceValue(xp.GetExp);
                    xp.PickupObject();
                    break;


                case HeartSource heart:
                    SoundManager.Instance.Play("xporb");
                    Heal(1);
                    heart.PickupObject();
                    break;

            }
        }
    }

    public bool IsTaggedWith(string searchTag)
    {
        foreach(string tag in _tags)
        {
            if (tag == searchTag)
                return true;
        }

        return false;
    }

    public IEnumerator CanTakeAgainDamageIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _canHealthChange = true;
    }

    public EntityType GetEntityType() { return _entityType; }
}

public enum EntityType
{
    Player,
    Enemy
}
