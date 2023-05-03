using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour, IEntityHealth, IExperience
{
    [SerializeField] EntityType _entityType;

    [SerializeField] private int _maxHealth;
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

    public delegate void EntityDeathSignature();
    public event EntityDeathSignature OnDeath;

    public delegate void EntityLevelChangeSignature(int level);
    public event EntityLevelChangeSignature OnLevelUpSuccess;
    public event EntityLevelChangeSignature OnLevelUpFail;

    private void Awake()
    {
        _currentLevel = Mathf.Clamp(_startLevel, 1, _maxLevel);
        _experience = Mathf.Abs(_experience);
        _health = _maxHealth;

        if(_entityType == EntityType.Player)
        {
            _xpPickupTimer = new CountdownTimer(0.125f, false, true);
            _xpPickupTimer.OnTimerExpired += PickupXPOrbs;
        }


        ControllerGame.Instance.AddEntityReference(this, _entityType);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (_xpPickupTimer != null)
            _xpPickupTimer.Update(Time.deltaTime);
    }

    public void Damage(int ammount)
    {
        int newHealth = _health - ammount;
        newHealth = Mathf.Clamp(newHealth, 0, _maxHealth);

        if (newHealth == 0)
        {
            if(OnDeath != null)
            OnDeath.Invoke();
            return;
        }

        if(OnHealthChanged != null)
        OnHealthChanged.Invoke(_health, newHealth, _maxHealth);
        _health = newHealth;
    }
    public void Heal(int ammount)
    {
        int newHealth = _health + ammount;
        newHealth = Mathf.Clamp(newHealth, 0, _maxHealth);

        if (newHealth == 0)
        {
            OnDeath.Invoke();
            return;
        }
        
        if(OnHealthChanged != null)
        OnHealthChanged.Invoke(_health, newHealth, _maxHealth);
        _health = newHealth;
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2);
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
