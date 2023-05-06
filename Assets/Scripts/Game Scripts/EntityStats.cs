using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour, IEntityHealth, IExperience
{
    [SerializeField] EntityType _entityType;

    public bool _canHealthChange = true;

    public int MaxHealth => _maxHealth;
    [SerializeField] private int _maxHealth;

    public int Health => _health;
    private int _health;

    [SerializeField] private int _experience;
    [SerializeField] private int _startLevel;
    [SerializeField] private int _maxLevel;

  
    private int _currentLevel;
    [SerializeField] private int[] _levelupThresholds;

    [SerializeField]private string[] _tags;

    CountdownTimer _xpPickupTimer;

    public delegate void EnemyHealthChangeSignature(int oldHealth, int newHealth, int maxHealth);
    public event EnemyHealthChangeSignature OnHealthChanged;

    public delegate void EntityDeathSignature(EntityStats entityStats);
    public event EntityDeathSignature OnDeath;

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


    private void Start()
    {
        
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
        if (_entityType != EntityType.Player)
        {
            var pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, -1); 
            var spawn = PoolManager.Spawn<FloatingDamageNumber>("FloatingDamageNumber", gameObject.transform, pos);
            spawn.Init(ammount);
        }
        
        if (_canHealthChange)
        {
            int newHealth = _health - ammount;
            newHealth = Mathf.Clamp(newHealth, 0, _maxHealth);

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

    public int GetExperienceValue() { return _experience; }
    public void ChangeExperienceValue(int change)
    {
        _experience += change;

        if (_currentLevel - 1 < _levelupThresholds.Length)
            if(_experience > _levelupThresholds[_currentLevel - 1])
            {
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
            XPSource xp = collider.GetComponent<XPSource>();

            if (xp != null)
            {
                ChangeExperienceValue(xp.Pickup());
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

    public EntityType GetEntityType() { return _entityType; }
}

public enum EntityType
{
    Player,
    Enemy
}
